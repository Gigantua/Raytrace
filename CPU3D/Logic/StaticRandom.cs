using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPU3D.Logic
{
    public static class Rand
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Xorshift> random = new ThreadLocal<Xorshift>(() => new Xorshift(Interlocked.Increment(ref seed)));

        public static Xorshift Val
        {
            get => random.Value;
        }
    }
}
