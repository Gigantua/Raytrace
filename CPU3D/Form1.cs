using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CPU3D.Platform.Windows;
using CPU3D.Draw;
using CPU3D.Logic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CPU3D.Engine;
using System.Drawing;
using CPU3D.RayTrace;

namespace CPU3D
{
    public partial class Form1 : Form
    {
        GameEngine engine;

        public Form1()
        {
            InitializeComponent();
            this.Width = 1024;
            this.Height = 768;

            engine = new GameEngine(this);

            engine.MouseLocked += (sender, e) => toolStrip1.Visible = false;
            engine.MouseUnlocked += (sender, e) => toolStrip1.Visible = true;

            toolStrip1.Visible = false;
        }

        private void Record_Click(object sender, EventArgs e)
        {
            if (engine.IsRecording) { engine.StopRecord(); return; }

            if (SaveVideo.ShowDialog() != DialogResult.OK) return;
            engine.StartRecord(SaveVideo.FileName);
        }

        bool rec = true;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (engine.IsRecording)
            {
                if (rec)
                {
                    Record.Image = CPU3D.Properties.Resources.if_player_Norecord_48792;
                    rec = false;
                }
                else
                {
                    Record.Image = CPU3D.Properties.Resources.if_player_record_48792;
                    rec = true;
                }
            }
            else Record.Image = CPU3D.Properties.Resources.if_player_record_48792;
        }

        private void speichernToolStripButton_Click(object sender, EventArgs e)
        {
            if (SaveImage.ShowDialog() != DialogResult.OK) return;

            engine.SaveImage(SaveImage.FileName);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
