using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CreatioClient.Core;
using CreatioClient.Core.Models.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CreatioClient.Console
{
    internal class Program
    {
        static async Task Main()
        {
            var app = new App();
            //await app.CallConfService();
            //await app.ExecuteGet();
            //await app.ExecutePost();
            await app.UseJson();
            await app.UseProtoBuf();
            //var app = new App("http://k_krylov:7040", "Supervisor", "Supervisor"); ;
            //var r = BenchmarkRunner.Run<App>();
            System.Console.WriteLine("Enter any key to exit");
            System.Console.ReadLine();
        }
    }

    [MemoryDiagnoser]
    public class App
    {
        private readonly Client _client;
        private readonly IDisposable _syncMsgLoggerSubscription;
        private readonly IDisposable _syncRealoadApplicationSubscription;
        SyncMsgLoggerObserver _syncMsgLoggerObserver = new SyncMsgLoggerObserver();
        ReloadApplicationObserver _reloadApplicationObserver = new ReloadApplicationObserver();
        public App()
        {
            _client = new Client(
                "http://k_krylov:7040", "Supervisor", "Supervisor");


            _syncMsgLoggerSubscription = _client.SubscribeToWebSocketMessages(_syncMsgLoggerObserver);
            _syncRealoadApplicationSubscription = _client.SubscribeToWebSocketMessages(_reloadApplicationObserver);
        }
        
        //public async Task CallConfService()
        //{
        //    var r = await _client.CallConfigurationServiceAsync("DemoService", "GetJson", "GET" );
        //    System.Console.WriteLine(r);
        //}

        //public async Task ExecuteGet()
        //{
        //    string r = _client.ExecuteGetRequest("http://k_krylov:7040/Login/NuiLogin.aspx?ReturnUrl=%2f");
        //    System.Console.WriteLine(r);
        //}

        //public async Task ExecutePost()
        //{
        //    var r = await _client.ExecutePostRequestAsync("http://k_krylov:7040/0/DataService/json/SyncReply/SelectQuery", "{\"rootSchemaName\":\"Survey\"");
        //    string str = await r.Content.ReadAsStringAsync();
        //    System.Console.WriteLine(str);
        //}

        [Benchmark]
        public async Task UseProtoBuf()
        {
            
            DTO.Proto photo = await _client.CallConfigurationServiceDeserializedAsync<DTO.Proto>(
              "DemoService", "GetStream", "GET",Core.SerializedWith.ProtobufNet);

            var f = photo.Files.FirstOrDefault();
            System.Console.WriteLine($"ProtoBuf: {f.FileName} of({f.Data.Length:N0}) bytes");

        }

        [Benchmark]
        public async Task UseJson()
        {
            DTO.Json photo = await _client.CallConfigurationServiceDeserializedAsync<DTO.Json>(
                "DemoService", "GetJson", "GET", Core.SerializedWith.Microsoft);

            var f = photo.Files.FirstOrDefault();
            System.Console.WriteLine($"Json    : {f.FileName} of({f.Data.Length:N0}) bytes");
        }
    }

    public class SyncMsgLoggerObserver : CreatioWebSocketMessageObserver
    {
        public override Func<WebSocketMessage, bool> MessageFilter => (message) =>
        {
            return message.MessageHeader.Sender == "SyncMsgLogger";
        };        
        
        public override Func<Exception, bool> ExceptionFilter => (exception) =>
        {
            return false;
        };
        
        public override void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public override void OnError(Exception error)
        {
            System.Console.WriteLine(error.Message);
        }

        public override void OnNext(WebSocketMessage value)
        {
            System.Console.BackgroundColor = ConsoleColor.DarkGreen;
            System.Console.WriteLine($"Message from: {value.MessageHeader.Sender}{Environment.NewLine}{value.MessageBody}");
            System.Console.ResetColor();
        }
    }
    
    public class ReloadApplicationObserver : CreatioWebSocketMessageObserver
    {
        public override Func<WebSocketMessage, bool> MessageFilter => (message) =>
        {
            return message.MessageHeader.Sender == "ReloadApplication";
        };        
        
        public override Func<Exception, bool> ExceptionFilter => (exception) =>
        {
            return true;
        };
        
        public override void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public override void OnError(Exception error)
        {
            System.Console.BackgroundColor = ConsoleColor.Red;
            System.Console.WriteLine(error.Message);
            System.Console.ResetColor();
        }

        public override void OnNext(WebSocketMessage value)
        {
            System.Console.WriteLine($"Message from: {value.MessageHeader.Sender}{Environment.NewLine}{value.MessageBody}");
        }
    }
}
