using System;
using System.Collections.Generic;
using System.Text;

namespace CreatioClient.Core
{
    ///<inheritdoc cref="IObservable{T}" />
    internal interface IFilteredObservable<out T>
    {
        ///<inheritdoc cref="IObservable{T}.Subscribe(IObserver{T})"/>
        IDisposable Subscribe(IFilteredObserver<T> observer);
    }

    /// <summary>
    /// Provides a mechanism for receiving push-based notifications.
    /// </summary>
    /// <typeparam name="T">The object that provides notification information.</typeparam>
    public interface IFilteredObserver<in T>

    {
        /// <summary>
        /// Encapsulates filter to for messages to receive
        /// </summary>
        Func<T, bool> MessageFilter { get; }

        /// <summary>
        /// Encapsulates filter for exception to receive
        /// </summary>
        Func<System.Exception, bool> ExceptionFilter { get; }

        /// <inheritdoc cref="IObserver{T}.OnCompleted"/>
        void OnCompleted();

        /// <inheritdoc cref="IObserver{T}.OnError(System.Exception)"/>
        void OnError(System.Exception error);

        /// <inheritdoc cref="IObserver{T}.OnNext(T)"/>
        void OnNext(T value);
    }
}
