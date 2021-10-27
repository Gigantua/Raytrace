using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPU3D.Logic
{
    class Preworker<T> : BlockingCollection<T>
    {
        List<Task> backgroundTasks = new List<Task>();
        CancellationTokenSource cts = new CancellationTokenSource();
        Func<T> PreworkerMethod;
        int ThreadCount;

        public Preworker(Func<T> PreworkerMethod, int Preworksize) : this(PreworkerMethod, Preworksize, 1)
        {

        }

        public Preworker(Func<T> PreworkerMethod, int Preworksize, int Threads) : base(new ConcurrentQueue<T>(), Preworksize)
        {
            this.PreworkerMethod = PreworkerMethod;
            this.ThreadCount = Threads;
        }

        public void Start()
        {
            for(int i=0;i< ThreadCount;i++)
            {
                Task background = Task.Factory.StartNew(() =>
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        T Element = PreworkerMethod();
                        base.Add(Element);
                    }
                    base.CompleteAdding();
                }, TaskCreationOptions.LongRunning);
                backgroundTasks.Add(background);
            }
        }

        public void Stop()
        {
            cts.Cancel();
            backgroundTasks.Clear();
        }

    }
}
