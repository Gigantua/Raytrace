using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU3D.Logic
{
    public class Xorshift
    {
        private long[] seed;

        public Xorshift()
        {
            long t = DateTime.Now.ToBinary();
            seed = new long[] { t, t, t, t };
            for (var i = Environment.TickCount % 1000; i != 0; i--)
            {
                Next();
            }
        }

        public Xorshift(long seed)
        {
            SetSeed(seed);
        }

        public Xorshift(long[] seed)
        {
            SetSeed(seed);
        }

        public Xorshift SetSeed(long seed)
        {
            this.seed = new long[] { seed, seed, seed, seed };
            return this;
        }

        public Xorshift SetSeed(long[] seed)
        {
            if (seed.Length < 4)
            {
                return SetSeed(seed[0]);
            }
            this.seed = seed;
            return this;
        }

        public long Next()
        {
            long t = seed[0] ^ (seed[0] << 11);
            seed[0] = seed[1];
            seed[1] = seed[2];
            seed[2] = seed[3];
            seed[3] = (seed[3] ^ (seed[3] >> 19)) ^ (t ^ (t >> 8));
            return seed[3];
        }

        public long NextLong()
        {
            long l = this.Next();
            if (this.NextBoolean())
            {
                l = ~l + 1L;
            }
            return l;
        }

        public int NextInt()
        {
            return (int)this.Next();
        }

        public (float, float, float) NextValueType()
        {
            return (NextFloat(), NextFloat(), NextFloat());
        }

        public int NextInt(int i)
        {
            double r = this.NextDouble();
            r *= i;
            return (int)Math.Floor((decimal)r);
        }

        public float NextFloat()
        {
            return (float)this.Next() / long.MaxValue;
        }

        public double NextDouble()
        {
            return (double)this.Next() / long.MaxValue;
        }

        private long temp;
        private char cnt = Char.MinValue;

        public bool NextBoolean()
        {
            if (cnt == 0)
            {
                temp = this.Next();
                cnt = '?'; // 63
            }
            bool b = (temp | 1L) == 0;
            temp >>= 1;
            cnt -= (char)1;
            return b;
        }
        public void NextBytes(byte[] b)
        {
            int length = b.Length;
            for (int i = 0; i < length; i++)
            {
                long n = this.Next();
                for (int j = 0; j < 7; j++)
                {
                    if (i < length)
                    {
                        b[i] = (byte)(n >> (j << 3));
                        i++;
                    }
                }
            }
        }

        private double nextGaussian_;
        private bool hasNextGaussian;

        public double NextGaussian()
        {
            if (hasNextGaussian)
            {
                hasNextGaussian = false;
                return nextGaussian_;
            }
            double x = Math.Sqrt(-2D * Math.Log(this.NextDouble()));
            double y = 2D * Math.PI * this.NextDouble();
            double z = x * Math.Sin(y);
            nextGaussian_ = x * Math.Cos(y);
            hasNextGaussian = true;
            return z;
        }

    }
}