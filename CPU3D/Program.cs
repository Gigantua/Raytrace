using CPU3D.Draw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPU3D
{
    static class Program
    {

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //RayTracer.RayTracerForm.Main2();
            Application.Run(new Form1());
        }
    }
}
