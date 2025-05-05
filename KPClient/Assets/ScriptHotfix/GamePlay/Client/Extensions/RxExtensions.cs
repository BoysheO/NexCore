using System;
using Cysharp.Threading.Tasks;

namespace Hotfix.Extensions
{
    public static class RxExtensions
    {
        public static IDisposable SubscribeAsButton<T>(this IObservable<T> observable, Func<UniTask> onNext)
        {
            return observable.Subscribe(new UniTaskObserver<T>(onNext));
        }

        private class UniTaskObserver<T> : IObserver<T>
        {
            private readonly Func<UniTask> _func;
            private bool _isRunning = false;
            private bool _isDead;

            public UniTaskObserver(Func<UniTask> func)
            {
                _func = func;
            }

            public void OnCompleted()
            {
                if (_isDead) return;
                _isDead = true;
            }

            public void OnError(Exception error)
            {
                if (_isDead) return;
                _isDead = true;
            }

            public void OnNext(T value)
            {
                if (_isDead) return;
                if (_isRunning) return;
                _isRunning = true;
                Run().Forget();
            }

            private async UniTask Run()
            {
                try
                {
                    await _func();
                }
                finally
                {
                    _isRunning = false;
                }
            }
        }

        // /// <summary>
        // /// --+--+--x
        // /// ---_---_x
        // /// --(+_)-(+_)-x
        // /// </summary>
        // public static IObservable<T> ScanT<T>(this IObservable<T> observable,IObservable<T> observable2)
        // {
        //     
        // }
        //
        // private class Scan2
        // {
        //     
        // }
    }
}