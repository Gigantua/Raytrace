using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPU3D.Engine
{
    public class Inputdevice
    {
        bool[] KeyStates = new bool[256];
        sbyte[] KeyChanges = new sbyte[256];
        protected long[] KeyChangeTime = new long[256];

        public Inputdevice()
        {
            for (int i = 0; i < 256; i++) KeyChangeTime[i] = DateTime.Now.Ticks;
        }


        public void Frame()
        {
            lock (KeyStates)
            {
                for (int i = 0; i < 256; i++) KeyChanges[i] = 0; //Take all changes
                for (int i = 0; i < 256; i++) KeyChangeTime[i] = DateTime.Now.Ticks;
            }
        }
        protected void Reset()
        {
            lock (KeyStates)
            {
                for (int i = 0; i < 256; i++)
                {
                    KeyStates[i] = false;
                    KeyChanges[i] = 0;
                    KeyChangeTime[i] = DateTime.Now.Ticks;
                }
            }
        }

        protected void KeyDown(int k)
        {
            if (KeyStates[k] == true) return; //Already down
            KeyChanges[k] = -1;
            KeyStates[k] = true;
            KeyChangeTime[k] = DateTime.Now.Ticks;
        }

        protected void KeyUp(int k)
        {
            if (KeyStates[k] == false) return; //Already up
            KeyChanges[k] = 1;
            KeyStates[k] = false;
            KeyChangeTime[k] = DateTime.Now.Ticks;
        }

        protected bool IsDown(int k)
        {
            return KeyStates[k];
        }

        protected bool StateChanged(int k)
        {
            return KeyChanges[k] != 0;
        }

        /// <summary>
        /// Was down is now up, use frame() in loop
        /// </summary>
        protected bool ChangedUp(int k)
        {
            return KeyChanges[k] == 1;
        }
        /// <summary>
        /// Was up is now down, use frame() in loop
        /// </summary>
        protected bool ChangedDown(int k)
        {
            return KeyChanges[k] == -1;
        }

        public override string ToString()
        {
            lock (KeyStates)
            {
                StringBuilder b = new StringBuilder();

                for (int i = 0; i < 256; i++)
                {
                    if (KeyStates[i])
                    {
                        b.Append($"|{((Keys)i).ToString()}");
                    }
                }
                b.Append("|");

                return b.ToString();
            }
        }
    }
}
