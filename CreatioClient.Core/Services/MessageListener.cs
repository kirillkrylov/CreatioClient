using CreatioClient.Core.Models.Domain;
using CreatioClient.Core.Models.Dto;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CreatioClient.Core.Services
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(System.Exception exception)
        {
            Exception = exception;
        }
        public System.Exception Exception { get; private set; }
    }

    internal interface IMessageListener
    {
        event EventHandler<EventArgs> NewMessage;
        event EventHandler<ErrorEventArgs> Error;
        Task StartReceiving(CancellationToken ct);
    }

    internal class MessageListener : IMessageListener
    {
        private readonly CookieContainer _cookies;
        private readonly IConfiguration _configuration;
        private readonly ILogin _login;
        private readonly ConcurrentQueue<WebSocketMessage> _queue;
        private ClientWebSocket _wss;
        
        public event EventHandler<EventArgs> NewMessage = delegate { };
        public event EventHandler<ErrorEventArgs> Error = delegate { };


        public MessageListener(CookieContainer cookies, IConfiguration configuration, ILogin login, ConcurrentQueue<WebSocketMessage> queue)
        {
            _cookies = cookies;
            _configuration = configuration;
            _login = login;
            _queue = queue;
        }
        private Uri SocketUri
        {
            get
            {
                string relativePart = (_configuration.IsNetCore) ? "Nui/ViewModule.aspx.ashx" : "0/Nui/ViewModule.aspx.ashx";

                return (_configuration.AppUri.Scheme == "https") ?
                    new Uri($"wss://{_configuration.AppUri.Authority}/{relativePart}") :
                    new Uri($"ws://{_configuration.AppUri.Authority}/{relativePart}");
            }
        }
        public async Task StartReceiving(CancellationToken ct)
        {
            if (_login.CurrentState != Login.State.LoggedIn)
            {
                var x = await _login.Execute();
            }
            
            Thread t = new Thread(async () =>
            {
                await Run(CancellationToken.None);
            });
            t.Start();
        }

        private async Task Run(CancellationToken ct)
        {
            while(!ct.IsCancellationRequested)
            {
                await CreateSocket();
                if (_wss.State != WebSocketState.Open)
                {
                    await Connect(ct);
                }
                await Receive(ct);
            }
        }

        private async Task CreateSocket()
        {
            if (_login.CurrentState != Login.State.LoggedIn)
            {
                await _login.Execute();
            }


            if (_wss == null)
            {
                _wss = new ClientWebSocket();
                _wss.Options.Cookies = _cookies;
                string value = _cookies.GetCookies(_configuration.AppUri).Cast<Cookie>()
                    .FirstOrDefault(c => c.Name == "BPMCSRF").Value;

                _wss.Options.SetRequestHeader("BPMCSRF", value);
            }
        }

        private async Task Connect(CancellationToken ct)
        {
            await _wss.ConnectAsync(SocketUri, ct);
        }

        private async Task Receive(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && _wss.State == WebSocketState.Open)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int increment = 128;
                    byte[] buffer = new byte[increment];
                    WebSocketReceiveResult result = default;
                    int bytesUsed = 0;

                    do
                    {
                        ArraySegment<byte> bufferSegment = new ArraySegment<byte>(buffer, bytesUsed, increment);
                        try
                        {
                            result = await _wss.ReceiveAsync(bufferSegment,ct);
                        }
                        catch (WebSocketException ex)
                        {
                            Error?.Invoke(this, new ErrorEventArgs(ex));
                        }

                        if(result is object)
                        {
                            ms.Write(bufferSegment.Array, bytesUsed, result.Count);
                            bytesUsed += result.Count;
                        }

                        if (!result.EndOfMessage)
                        {
                            Array.Resize(ref buffer, buffer.Length+ increment);
                        }

                    }
                    while (result is object && result is object && !result.EndOfMessage && !ct.IsCancellationRequested);
                    
                    if (result is object && result.MessageType == WebSocketMessageType.Text)
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            char[] buf = new char[bytesUsed];
                            await reader.ReadBlockAsync(buf, 0, bytesUsed);
                            string txt = new string(buf);

                            if (!string.IsNullOrEmpty(txt) && !string.IsNullOrWhiteSpace(txt))
                            {
                                WebSocketMessage obj = JsonSerializer.Deserialize<WebSocketMessage>(txt);

                                _queue.Enqueue(obj);
                                NewMessage?.Invoke(this, new EventArgs());
                            }
                        }
                    }
                }
            }

            _wss.Dispose();
            _wss = null;
        }
    }
}
