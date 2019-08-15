namespace CMWtests
{
    partial class TempGauge
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxGauge = new System.Windows.Forms.PictureBox();
            this.pictureBoxSlider = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelTemp = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGauge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxGauge
            // 
            this.pictureBoxGauge.BackColor = System.Drawing.SystemColors.HighlightText;
            this.pictureBoxGauge.Image = global::CMWtests.Properties.Resources.Gauge;
            this.pictureBoxGauge.Location = new System.Drawing.Point(13, 17);
            this.pictureBoxGauge.Name = "pictureBoxGauge";
            this.pictureBoxGauge.Size = new System.Drawing.Size(160, 9);
            this.pictureBoxGauge.TabIndex = 6;
            this.pictureBoxGauge.TabStop = false;
            // 
            // pictureBoxSlider
            // 
            this.pictureBoxSlider.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.pictureBoxSlider.Location = new System.Drawing.Point(52, 13);
            this.pictureBoxSlider.Name = "pictureBoxSlider";
            this.pictureBoxSlider.Size = new System.Drawing.Size(2, 17);
            this.pictureBoxSlider.TabIndex = 7;
            this.pictureBoxSlider.TabStop = false;
            this.pictureBoxSlider.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "25";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "40";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(124, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "55";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(164, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "65";
            // 
            // labelTemp
            // 
            this.labelTemp.AutoSize = true;
            this.labelTemp.ForeColor = System.Drawing.Color.Black;
            this.labelTemp.Location = new System.Drawing.Point(44, 30);
            this.labelTemp.Name = "labelTemp";
            this.labelTemp.Size = new System.Drawing.Size(19, 13);
            this.labelTemp.TabIndex = 5;
            this.labelTemp.Text = "35";
            this.labelTemp.Visible = false;
            // 
            // TempGauge
            // 
            this.Controls.Add(this.labelTemp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxSlider);
            this.Controls.Add(this.pictureBoxGauge);
            this.Enabled = false;
            this.Name = "TempGauge";
            this.Size = new System.Drawing.Size(186, 41);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGauge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxGauge;
        private System.Windows.Forms.PictureBox pictureBoxSlider;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label labelTemp;
    }
}
