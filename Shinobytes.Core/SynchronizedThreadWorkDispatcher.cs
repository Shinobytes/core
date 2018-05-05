/*******************************************************************\
* Copyright (c) 2016 Shinobytes, Gothenburg, Sweden.                *
* Any usage of the content of this file, in part or whole, without  *
* a written agreement from Shinobytes, will be considered a         *
* violation against international copyright law.                    *
\*******************************************************************/

using System;
using System.Collections.Generic;
using System.Threading;

namespace Shinobytes.Core
{
    public class SynchronizedThreadWorkDispatcher : IThreadWorkDispatcher, IDisposable
    {
        private static readonly object queueLock = new object();
        private readonly IThreadWorkDispatcherSettings settings;
        private readonly Queue<Action> workerQueue;
        private readonly Thread[] workerThreads;
        private readonly CancellationTokenSource cancellationTokenSource;
        private bool disposed;

        public SynchronizedThreadWorkDispatcher(IThreadWorkDispatcherSettings settings)
        {
            this.settings = settings;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.workerQueue = new Queue<Action>();
            this.workerThreads = new Thread[settings.MaximumConcurrentThreads];
            this.CreateWorkerThreads();
        }

        private void CreateWorkerThreads()
        {
            for (var i = 0; i < settings.MaximumConcurrentThreads; i++)
            {
                workerThreads[i] = new Thread(Run);
                workerThreads[i].Start();
            }
        }

        private void Run()
        {
            do
            {
                var action = Next();
                if (action != null)
                {
                    action.Invoke();
                }
                else
                {
                    Thread.Sleep(settings.MsWaitTime);
                }
            } while (!cancellationTokenSource.IsCancellationRequested);
        }

        private Action Next()
        {
            lock (queueLock)
            {
                return workerQueue.Count > 0 ? workerQueue.Dequeue() : null;
            }
        }

        public void Dispatch(Action work)
        {
            lock (queueLock)
            {
                workerQueue.Enqueue(work);
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            cancellationTokenSource.Cancel();
        }
    }
}