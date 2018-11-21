namespace Until_Biome
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.elevationbutton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savefileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forCastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCastResultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(23, 46);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(460, 460);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // elevationbutton
            // 
            this.elevationbutton.Location = new System.Drawing.Point(38, 36);
            this.elevationbutton.Name = "elevationbutton";
            this.elevationbutton.Size = new System.Drawing.Size(121, 40);
            this.elevationbutton.TabIndex = 1;
            this.elevationbutton.Text = "Biome";
            this.elevationbutton.UseVisualStyleBackColor = true;
            this.elevationbutton.Click += new System.EventHandler(this.elevationbutton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.forCastToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(722, 27);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openfileToolStripMenuItem,
            this.savefileToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(61, 23);
            this.menuToolStripMenuItem.Text = "menu";
            // 
            // openfileToolStripMenuItem
            // 
            this.openfileToolStripMenuItem.Name = "openfileToolStripMenuItem";
            this.openfileToolStripMenuItem.Size = new System.Drawing.Size(140, 26);
            this.openfileToolStripMenuItem.Text = "openfile";
            this.openfileToolStripMenuItem.Click += new System.EventHandler(this.openfileToolStripMenuItem_Click);
            // 
            // savefileToolStripMenuItem
            // 
            this.savefileToolStripMenuItem.Name = "savefileToolStripMenuItem";
            this.savefileToolStripMenuItem.Size = new System.Drawing.Size(140, 26);
            this.savefileToolStripMenuItem.Text = "savefile";
            this.savefileToolStripMenuItem.Click += new System.EventHandler(this.savefileToolStripMenuItem_Click);
            // 
            // forCastToolStripMenuItem
            // 
            this.forCastToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveCastResultToolStripMenuItem});
            this.forCastToolStripMenuItem.Name = "forCastToolStripMenuItem";
            this.forCastToolStripMenuItem.Size = new System.Drawing.Size(74, 23);
            this.forCastToolStripMenuItem.Text = "ForCast";
            // 
            // saveCastResultToolStripMenuItem
            // 
            this.saveCastResultToolStripMenuItem.Name = "saveCastResultToolStripMenuItem";
            this.saveCastResultToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.saveCastResultToolStripMenuItem.Text = "SaveCastResult";
            this.saveCastResultToolStripMenuItem.Click += new System.EventHandler(this.saveCastResultToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.elevationbutton);
            this.panel1.Location = new System.Drawing.Point(509, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(201, 340);
            this.panel1.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(38, 260);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 38);
            this.button2.TabIndex = 3;
            this.button2.Text = "Show_World";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(38, 156);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 40);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cast_to_World";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Voronoi Map (*.json) | *.json";
            // 
            // saveFileDialog2
            // 
            this.saveFileDialog2.Filter = "Voronoi Map (*.json) | *.json";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 566);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button elevationbutton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openfileToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem savefileToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem forCastToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCastResultToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog2;
        private System.Windows.Forms.Button button2;
    }
}

