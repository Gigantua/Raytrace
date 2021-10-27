using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPU3D.Logic
{
    public class TimeMeasure
    {
        public List<TimeMeasure> TimedEvents = new List<TimeMeasure>();
        public string Name { get; }
        public TimeSpan Duration => TimeSpan.FromTicks(ticks);

        public long ticks = 0;

        public TimeMeasure Run(string Name, Action<TimeMeasure> t)
        {
            TimeMeasure timer = new TimeMeasure(Name, t);
            TimedEvents.Add(timer);
            return timer;
        }

        public TimeMeasure StartThreadTimer(string Name)
        {
            var threadtimer = new TimeMeasure(Name);
            TimedEvents.Add(threadtimer);
            return threadtimer;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        public T MeasureThread<T>(Func<TimeMeasure, T> f)
        {
            long t1;
            long t2;
            QueryPerformanceCounter(out t1);
            T val = f(this);
            QueryPerformanceCounter(out t2);
            Interlocked.Add(ref ticks, t2 - t1);
            return val;
        }


        public T Run<T>(string Name, Func<TimeMeasure,T> f)
        {
            TimeMeasure timer = new TimeMeasure(Name);
            TimedEvents.Add(timer);
            return timer.MeasureTime(f);
        }

        T MeasureTime<T>(Func<TimeMeasure,T> f)
        {
            Stopwatch dT = Stopwatch.StartNew();
            T result = f(this);
            this.ticks = dT.Elapsed.Ticks;
            return result;
        }

        public TimeMeasure(string Name)
        {
            this.Name = Name;
        }

        public TimeMeasure(string Name, Action<TimeMeasure> t)
        {
            this.Name = Name;
            Stopwatch dT = Stopwatch.StartNew();
            t(this);
            this.ticks = dT.Elapsed.Ticks;
        }


        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            if (ticks == 0)
            {
                ticks = TimedEvents.Sum(x => x.ticks);
            }

            tostring(str, 0);
            return str.ToString();
        }

        void Intend(StringBuilder builder, int intend)
        {
            for(int i=0;i<intend;i++)
            {
                builder.Append("\t");
            }
        }

        void tostring(StringBuilder builder, int intend)
        {
            Intend(builder, intend);
            

            builder.AppendLine($"{Name}: {Duration.TotalMilliseconds.ToString("0.00000")}ms");
            for (int i = 0; i < TimedEvents.Count; i++)
            {
                TimedEvents[i].tostring(builder, intend+1);
            }
        }

        public void Clear()
        {
            TimedEvents.Clear();
            ticks = 0;
        }
    }
}
