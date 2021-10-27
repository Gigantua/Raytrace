using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.UI
{
    class WaitableTimer : WaitHandle
    {
        [DllImport("kernel32.dll")]
        static extern SafeWaitHandle CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWaitableTimer(SafeWaitHandle hTimer, [In] ref long pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine, [MarshalAs(UnmanagedType.Bool)] bool fResume);

        Task<bool> WaitTask = null;
        RegisteredWaitHandle RegisteredHandle = null;

        public WaitableTimer(bool manualReset = true, string timerName = null)
        {
            this.SafeWaitHandle = CreateWaitableTimer(IntPtr.Zero, manualReset, timerName);

            
        }

        ~WaitableTimer()
        {
            this.Dispose(false);
            RegisteredHandle.Unregister(this);
        }

        void NativeSet(long dueTime)
        {
            if (!SetWaitableTimer(this.SafeWaitHandle, ref dueTime, 0, IntPtr.Zero, IntPtr.Zero, false))
            {
                throw new Win32Exception();
            }
        }

        void settime(TimeSpan timeout)
        {
            Int64 Time = -(long)(10 * timeout.TotalMilliseconds * 1000.0); // Convert to 100 nanosecond interval, negative value indicates relative time
            if (timeout <= TimeSpan.FromSeconds(0)) //negative wait = no wait
            {
                NativeSet(0);
            }
            else
            {
                NativeSet(Time);
            }
        }

        public bool Sleep(TimeSpan timeout)
        {
            settime(timeout);
            return this.WaitOne();
        }

        public Task<bool> SleepAsync(TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<bool>();
            RegisteredHandle = ThreadPool.RegisterWaitForSingleObject(
                this,
                (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                tcs,
                int.MaxValue,
                true);
            WaitTask = tcs.Task;

            settime(timeout);
            return WaitTask;
        }

    }

    class PreciseTimer
    {
        TimeSpan[] times;
        int timeindex = 0;
        WaitableTimer timerhandler;
        public TimeSpan TimeToWait { get; private set; }


        /// <summary>
        /// Do not use too many, Called directly by the system and can slowdown the system
        /// </summary>
        /// <param name="Milliseconds"></param>
        public PreciseTimer(TimeSpan time) : this(time,90)
        {
            
        }

        public static PreciseTimer StartNew(TimeSpan time)
        {
            return new PreciseTimer(time);
        }

        public static PreciseTimer FromFPS(double TargetFPS)
        {
            return new PreciseTimer(TimeSpan.FromMilliseconds(1000.0 / TargetFPS));
        }

        public PreciseTimer(TimeSpan time, int AverageSize)
        {
            if (AverageSize == 0) throw new ArgumentException(nameof(AverageSize) + " must be positive");

            times = new TimeSpan[AverageSize];
            this.TimeToWait = time;
            timerhandler = new WaitableTimer();
            Start();
        }

        void Start()
        {
            Stopwatch frametime = Stopwatch.StartNew();
            new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    TimerElapsedEvent?.Invoke(this, frametime.Elapsed); //invoke

                    TimeSpan DeltaTime = TimeToWait - frametime.Elapsed;
                    timerhandler.Sleep(DeltaTime);

                    times[timeindex] = frametime.Elapsed;
                    timeindex++;
                    if (timeindex == times.Length) timeindex = 0;
                    frametime.Restart();
                }
            }))
            { IsBackground = true }.Start();
            
        }


        TimeSpan calcaverage()
        {
            long sum = 0;
            int length = 0;
            long local = 0;

            for (int i=0;i<times.Length;local = times[i].Ticks,i++)
            {
                if (local!=0)
                {
                    sum += local;
                    length++;
                }
            }

            if (length == 0) return TimeSpan.FromMilliseconds(0);

            return new TimeSpan(sum / length);
        }

        void Wait(TimeSpan dT)
        {
            Stopwatch elapsed = Stopwatch.StartNew();
            while(dT > elapsed.Elapsed)
            {
                Thread.SpinWait(10000);
            }
        }

        /// <summary>
        /// Average of last 3 deltaTimes
        /// </summary>
        public TimeSpan Average => calcaverage();
        public event EventHandler<TimeSpan> TimerElapsedEvent;
    }
}
