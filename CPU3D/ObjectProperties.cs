using CPU3D.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPU3D
{
    public partial class ObjectProperties : Form
    {
        public static bool DialogueOpen = false;
        Object3D obj;
        Material r;
        TextBox focusedControl;

        public ObjectProperties(Object3D obj)
        {
            InitializeComponent();
            this.Text = "Properties of " + obj.GetType().Name;
            this.FormClosed += (sender, e) =>
            {
                DialogueOpen = false;
            };
            this.obj = obj;
            this.r = obj.Material;
            
            InitBox(Abso, 0, 0.1, () => r.Optics_Absorptionrate, (x) => r.Optics_Absorptionrate = x);
            InitBox(Ref, 1, 2.5, () => r.Optics_RefractionIndex, (x) => r.Optics_RefractionIndex = x);
            InitBox(Reflectbox, 0, 1, () => r.Reflective_Percentage, (x) => r.Reflective_Percentage = x);
            InitBool(checkBox1, () => r.HasShadow, (x) => r.HasShadow = x);
            InitBool(checkBox2, () => r.HasColoredShadow, (x) => r.HasColoredShadow = x);
            InitColor(button1, () => r.Color, (x) => r.Color = x);
        }

        void InitBool(CheckBox box, Func<bool> Getter, Action<bool> Setter)
        {
            box.Checked = Getter();
            box.CheckedChanged += (sender, e) =>
            {
                Setter(box.Checked);
            };
        }

        
        void InitColor(Button b, Func<Draw.Color> Getter, Action<Draw.Color> Setter)
        {
            var col = Getter();
            System.Drawing.Color c = System.Drawing.Color.FromArgb((byte)(col.R * 255.0), (byte)(col.G * 255.0), (byte)(col.B * 255.0));
            b.FlatStyle = FlatStyle.Flat;
            b.BackColor = c;

            b.Click += (sender, e) =>
            {
                var currentc = Getter();
                System.Drawing.Color setting = System.Drawing.Color.FromArgb((byte)(currentc.R * 255.0), (byte)(currentc.G * 255.0), (byte)(currentc.B * 255.0));
                colorDialog1.Color = setting;
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    Color k = colorDialog1.Color;
                    b.BackColor = k;
                    Setter(Draw.Colors.Convertfloat((k.R, k.G, k.B)));
                }
            };
        }

        void InitBox(TextBox box,double min, double max, Func<float> Getter, Action<float> Setter)
        {
            box.Tag = (min, max);
            box.Text = Getter().ToString(CultureInfo.InvariantCulture);
            box.TextChanged += (sender, e) =>
            {
                if (CheckDouble(box, out double val))
                {
                    Setter((float)val);
                }
            };
            box.GotFocus += (sender, e) => focusedControl = box;
        }

        public void ShowSingle()
        {
            if (DialogueOpen) return;
            else
            {
                DialogueOpen = true;
                Thread T = new Thread(() =>
                {
                    this.ShowDialog();
                });
                T.IsBackground = true;
                T.SetApartmentState(ApartmentState.STA);
                T.Start();
            }
        }

        bool CheckDouble(TextBox box, out double value)
        {
            if (double.TryParse(box.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                errorProvider1.SetError(box, null);
                return true;
            }
            else
            {
                errorProvider1.SetError(box, "Please Enter a valid number");
                return false;
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            (double min, double max) = (ValueTuple<double, double>)focusedControl.Tag;
            
            double val = min + trackBar1.Value * (max - min) / trackBar1.Maximum;
            focusedControl.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
