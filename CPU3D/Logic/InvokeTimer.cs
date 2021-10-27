using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace CPU3D.Logic
{
    public class InvokeTimer
    {
        Stopwatch dT = Stopwatch.StartNew();
        TimeSpan timeout;

        public InvokeTimer(TimeSpan timeout)
        {
            this.timeout = timeout;
        }

        public void Invoke(Action t, Control control)
        {
            if (dT.Elapsed > timeout)
            {
                dT.Restart();
                if (control != null && control.InvokeRequired) try { control.Invoke(t); } catch { }
                else t();
            }
        }

        public async void AsyncInvoke(Action t, Control control)
        {
            await Task.Run(() => Invoke(t, control));
        }

    }
}
