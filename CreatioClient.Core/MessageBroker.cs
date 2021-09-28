using CreatioClient.Core.Models.Dto;
using CreatioClient.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CreatioClient.Core
{
    internal interface IMessageBroker: IFilteredObservable<WebSocketMessage>
    {

    }
        
    internal class MessageBroker : IMessageBroker
    {
        #region Fields
        private readonly IMessageListener _listener;
        private readonly ConcurrentQueue<WebSocketMessage> _queue;
        private readonly List<WeakReference<IFilteredObserver<WebSocketMessage>>> _subsribers = new List<WeakReference<IFilteredObserver<WebSocketMessage>>>();
        #endregion

        #region Class : Unsubscriber
        private class Unsubscriber : IDisposable
        {
            private readonly List<WeakReference<IFilteredObserver<WebSocketMessage>>> _subscribers;
            private readonly WeakReference<IFilteredObserver<WebSocketMessage>> _subscriber;
            public Unsubscriber(List<WeakReference<IFilteredObserver<WebSocketMessage>>> subscribers,
                WeakReference<IFilteredObserver<WebSocketMessage>> subscriber
                )
            {
                _subscribers = subscribers;
                _subscriber = subscriber;
            }
            
            public void Dispose()
            {
                _subscribers.Remove(_subscriber);
            }
        }
        #endregion

        #region Constructor
        public MessageBroker(IMessageListener listener, ConcurrentQueue<WebSocketMessage> queue)
        {
            _listener = listener;
            _queue = queue;
            _listener.NewMessage += OnNewMessage;
            _listener.Error += OnError;
            Task.Run(async () => await _listener.StartReceiving(CancellationToken.None)).Wait();
        }
        #endregion

        #region IFilteredObservable<WebSocketMessage>
        /// <inheritdoc cref="IFilteredObservable{T}.Subscribe(IFilteredObserver{T})"/>
        public IDisposable Subscribe(IFilteredObserver<WebSocketMessage> observer)
        {
            var w = new WeakReference<IFilteredObserver<WebSocketMessage>>(observer);
            _subsribers.Add(w);
            return new Unsubscriber(_subsribers, w);
        }
        #endregion

        #region EventHandler
        /// <summary>Handler to process new NewMessage event.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data.</param>
        /// <remarks> Event is raised by implementation <see cref="IMessageListener"/>, 
        /// default implementation <see cref="MessageListener" />
        /// </remarks>
        private void OnNewMessage(object sender, EventArgs e)
        {
            if(_queue.TryDequeue(out  WebSocketMessage message))
            {
                for (int i = 0; i < _subsribers.Count; i++)
                {
                    if(_subsribers[i].TryGetTarget(out IFilteredObserver<WebSocketMessage> target))
                    {
                        if (target.MessageFilter.Invoke(message))
                        {
                            target.OnNext(message);
                        }
                    }
                    else
                    {
                        _subsribers.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>Handler to process new Error event.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks> Event is raised by implementation <see cref="IMessageListener"/>, 
        /// default implementation <see cref="MessageListener" />
        /// </remarks>
        private void OnError(object sender, ErrorEventArgs e)
        {
            for (int i = 0; i < _subsribers.Count; i++)
            {
                if (_subsribers[i].TryGetTarget(out IFilteredObserver<WebSocketMessage> target))
                {

                    if (target.ExceptionFilter.Invoke(e.Exception))
                    {
                        target.OnError(e.Exception);
                    }
                }
                else
                {
                    _subsribers.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}
