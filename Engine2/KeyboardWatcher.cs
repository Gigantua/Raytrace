using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engine2
{
    class Keyboard
    {
        Dictionary<Keys, bool> keys = new Dictionary<Keys, bool>();

        public Keyboard(Form form)
        {
            form.KeyPreview = true;
            form.PreviewKeyDown += Form_PreviewKeyDown;
            form.KeyUp += Form_KeyUp;
        }

        public Keys[] AllPressed()
        {
            return keys.Where(x => x.Value == true).Select(x => x.Key).ToArray();
        }

        public bool IsDown(Keys key)
        {
            if (key == Keys.Control)
            {
                
            }


            bool state;

            if (keys.TryGetValue(key, out state) == true)
            {
                return state;
            }
            return false;
        }

        private void Form_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            keys[e.KeyCode] = true;
        }
        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            keys[e.KeyCode] = false;
        }
    }
}
