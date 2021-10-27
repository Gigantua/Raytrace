using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engine2
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        public event EventHandler PropertyChanged;

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked) return;
            radioButton2.Checked = !radioButton2.Checked;

            radioButton1.Checked = !radioButton2.Checked;
            radioButton3.Checked = !radioButton2.Checked;

            PropertyChanged?.Invoke(this, e);
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            if (radioButton3.Checked) return;
            radioButton3.Checked = !radioButton3.Checked;

            radioButton1.Checked = !radioButton3.Checked;
            radioButton2.Checked = !radioButton3.Checked;

            PropertyChanged?.Invoke(this, e);
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) return;
            radioButton1.Checked = !radioButton1.Checked;

            radioButton2.Checked = !radioButton1.Checked;
            radioButton3.Checked = !radioButton1.Checked;

            PropertyChanged?.Invoke(this, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
