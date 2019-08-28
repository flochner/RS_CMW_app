namespace CMWtests
{
    partial class OptionsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.numericUpDown_EPS = new System.Windows.Forms.NumericUpDown();
            this.labelStatisticsCount = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.checkBoxTempOverride = new System.Windows.Forms.CheckBox();
            this.checkBoxKB036 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_EPS)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown_EPS
            // 
            this.numericUpDown_EPS.Location = new System.Drawing.Point(135, 23);
            this.numericUpDown_EPS.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_EPS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_EPS.Name = "numericUpDown_EPS";
            this.numericUpDown_EPS.Size = new System.Drawing.Size(38, 20);
            this.numericUpDown_EPS.TabIndex = 0;
            this.numericUpDown_EPS.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // labelStatisticsCount
            // 
            this.labelStatisticsCount.AutoSize = true;
            this.labelStatisticsCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatisticsCount.Location = new System.Drawing.Point(12, 23);
            this.labelStatisticsCount.Name = "labelStatisticsCount";
            this.labelStatisticsCount.Size = new System.Drawing.Size(117, 15);
            this.labelStatisticsCount.TabIndex = 1;
            this.labelStatisticsCount.Text = "EPS Statistics Count";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(143, 196);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(224, 196);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReset.Location = new System.Drawing.Point(12, 196);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // checkBoxTempOverride
            // 
            this.checkBoxTempOverride.AutoSize = true;
            this.checkBoxTempOverride.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxTempOverride.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.checkBoxTempOverride.Location = new System.Drawing.Point(25, 49);
            this.checkBoxTempOverride.Name = "checkBoxTempOverride";
            this.checkBoxTempOverride.Size = new System.Drawing.Size(124, 19);
            this.checkBoxTempOverride.TabIndex = 2;
            this.checkBoxTempOverride.Text = "Override WarmUp";
            this.checkBoxTempOverride.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.checkBoxTempOverride.UseVisualStyleBackColor = true;
            // 
            // checkBoxKB036
            // 
            this.checkBoxKB036.AutoSize = true;
            this.checkBoxKB036.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxKB036.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.checkBoxKB036.Location = new System.Drawing.Point(66, 74);
            this.checkBoxKB036.Name = "checkBoxKB036";
            this.checkBoxKB036.Size = new System.Drawing.Size(83, 19);
            this.checkBoxKB036.TabIndex = 3;
            this.checkBoxKB036.Text = "hasKB036";
            this.checkBoxKB036.UseVisualStyleBackColor = true;
            this.checkBoxKB036.Visible = false;
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(311, 231);
            this.Controls.Add(this.checkBoxKB036);
            this.Controls.Add(this.checkBoxTempOverride);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelStatisticsCount);
            this.Controls.Add(this.numericUpDown_EPS);
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_EPS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown_EPS;
        private System.Windows.Forms.Label labelStatisticsCount;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox checkBoxTempOverride;
        private System.Windows.Forms.CheckBox checkBoxKB036;
    }
}