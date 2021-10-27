namespace CPU3D
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.neuToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.öffnenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.speichernToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Record = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.ausschneidenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.kopierenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.einfügenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.hilfeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.SaveVideo = new System.Windows.Forms.SaveFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SaveImage = new System.Windows.Forms.SaveFileDialog();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.neuToolStripButton,
            this.öffnenToolStripButton,
            this.speichernToolStripButton,
            this.Record,
            this.toolStripSeparator,
            this.ausschneidenToolStripButton,
            this.kopierenToolStripButton,
            this.einfügenToolStripButton,
            this.toolStripSeparator1,
            this.hilfeToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(711, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // neuToolStripButton
            // 
            this.neuToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.neuToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("neuToolStripButton.Image")));
            this.neuToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.neuToolStripButton.Name = "neuToolStripButton";
            this.neuToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.neuToolStripButton.Text = "&Neu";
            // 
            // öffnenToolStripButton
            // 
            this.öffnenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.öffnenToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("öffnenToolStripButton.Image")));
            this.öffnenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.öffnenToolStripButton.Name = "öffnenToolStripButton";
            this.öffnenToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.öffnenToolStripButton.Text = "Ö&ffnen";
            // 
            // speichernToolStripButton
            // 
            this.speichernToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.speichernToolStripButton.Image = global::CPU3D.Properties.Resources.camera_icon;
            this.speichernToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.speichernToolStripButton.Name = "speichernToolStripButton";
            this.speichernToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.speichernToolStripButton.Text = "&Speichern";
            this.speichernToolStripButton.Click += new System.EventHandler(this.speichernToolStripButton_Click);
            // 
            // Record
            // 
            this.Record.Image = global::CPU3D.Properties.Resources.if_player_record_48792;
            this.Record.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Record.Name = "Record";
            this.Record.Size = new System.Drawing.Size(46, 22);
            this.Record.Text = "Rec";
            this.Record.Click += new System.EventHandler(this.Record_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // ausschneidenToolStripButton
            // 
            this.ausschneidenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ausschneidenToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ausschneidenToolStripButton.Image")));
            this.ausschneidenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ausschneidenToolStripButton.Name = "ausschneidenToolStripButton";
            this.ausschneidenToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.ausschneidenToolStripButton.Text = "&Ausschneiden";
            // 
            // kopierenToolStripButton
            // 
            this.kopierenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.kopierenToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("kopierenToolStripButton.Image")));
            this.kopierenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.kopierenToolStripButton.Name = "kopierenToolStripButton";
            this.kopierenToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.kopierenToolStripButton.Text = "&Kopieren";
            // 
            // einfügenToolStripButton
            // 
            this.einfügenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.einfügenToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("einfügenToolStripButton.Image")));
            this.einfügenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.einfügenToolStripButton.Name = "einfügenToolStripButton";
            this.einfügenToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.einfügenToolStripButton.Text = "&Einfügen";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // hilfeToolStripButton
            // 
            this.hilfeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hilfeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("hilfeToolStripButton.Image")));
            this.hilfeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hilfeToolStripButton.Name = "hilfeToolStripButton";
            this.hilfeToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.hilfeToolStripButton.Text = "Hi&lfe";
            // 
            // SaveVideo
            // 
            this.SaveVideo.Filter = "Video File (.mkv)|*.mkv|Bitmap Image (.bmp)|*.bmp";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SaveImage
            // 
            this.SaveImage.Filter = "Bitmap Image (.bmp)|*.bmp";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 519);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton neuToolStripButton;
        private System.Windows.Forms.ToolStripButton öffnenToolStripButton;
        private System.Windows.Forms.ToolStripButton speichernToolStripButton;
        private System.Windows.Forms.ToolStripButton Record;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton ausschneidenToolStripButton;
        private System.Windows.Forms.ToolStripButton kopierenToolStripButton;
        private System.Windows.Forms.ToolStripButton einfügenToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton hilfeToolStripButton;
        private System.Windows.Forms.SaveFileDialog SaveVideo;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.SaveFileDialog SaveImage;
    }
}

