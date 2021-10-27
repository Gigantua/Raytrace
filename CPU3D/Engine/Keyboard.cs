using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPU3D.Engine
{
    class Keyboard : Inputdevice
    {
        
        public Keyboard(Form form)
        {
            form.PreviewKeyDown += Form_PreviewKeyDown;
            form.KeyUp += Form_KeyUp;
            form.LostFocus += Form_LostFocus;
        }

        

        private void Form_LostFocus(object sender, EventArgs e)
        {
            Reset();
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            int k = (int)e.KeyCode;
            base.KeyUp(k);
        }

        private void Form_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int k = (int)e.KeyCode;
            base.KeyDown(k);
        }

        public bool IsDown(Keys k, out float dT)
        {
            dT = (float)(DateTime.Now - new DateTime(base.KeyChangeTime[(int)k])).TotalSeconds;
            return base.IsDown((int)k);
        }

        public bool IsUp(Keys k)
        {
            return !base.IsDown((int)k);
        }

        public bool StateChanged(Keys k)
        {
            return base.StateChanged((int)k);
        }

        /// <summary>
        /// Was down is now up, use frame() in loop
        /// </summary>
        public bool ChangedUp(Keys k)
        {
            return base.ChangedUp((int)k);
        }
        /// <summary>
        /// Was up is now down, use frame() in loop
        /// </summary>
        public bool ChangedDown(Keys k)
        {
            return base.ChangedDown((int)k);
        }

    }
}
