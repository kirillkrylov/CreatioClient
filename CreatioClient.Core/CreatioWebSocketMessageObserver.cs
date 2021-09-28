using CreatioClient.Core.Models.Dto;
using System;

namespace CreatioClient.Core
{
    /// <inheritdoc cref="IFilteredObserver{WebSocketMessage}"/>
    public abstract class CreatioWebSocketMessageObserver : IFilteredObserver<WebSocketMessage>
    {
        /// <inheritdoc cref="IFilteredObserver{WebSocketMessage}.OnCompleted"/>
        public abstract void OnCompleted();

        /// <inheritdoc cref="IFilteredObserver{WebSocketMessage}.OnError(System.Exception)"/>
        public abstract void OnError(System.Exception error);

        /// <inheritdoc cref="IFilteredObserver{WebSocketMessage}}.OnNext(T)"/>
        public abstract void OnNext(WebSocketMessage value);

        public abstract Func<WebSocketMessage, bool> MessageFilter { get; }
        public abstract Func<System.Exception, bool> ExceptionFilter { get; }
    }
}
