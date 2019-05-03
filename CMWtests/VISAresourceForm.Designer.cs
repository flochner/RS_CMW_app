using System.Windows.Forms;

namespace CMWtests
{
    partial class VISAresourceForm
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
            this.listBoxResources = new System.Windows.Forms.ListBox();
            this.BtnSelect = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxResources
            // 
            this.listBoxResources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxResources.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxResources.ItemHeight = 15;
            this.listBoxResources.Location = new System.Drawing.Point(12, 10);
            this.listBoxResources.MinimumSize = new System.Drawing.Size(0, 19);
            this.listBoxResources.Name = "listBoxResources";
            this.listBoxResources.Size = new System.Drawing.Size(319, 19);
            this.listBoxResources.Sorted = true;
            this.listBoxResources.TabIndex = 1;
            this.listBoxResources.SelectedIndexChanged += new System.EventHandler(this.listBoxResources_SelectedIndexChanged);
            this.listBoxResources.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxResources_DoubleClick);
            // 
            // BtnSelect
            // 
            this.BtnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnSelect.Location = new System.Drawing.Point(171, 44);
            this.BtnSelect.Name = "BtnSelect";
            this.BtnSelect.Size = new System.Drawing.Size(80, 23);
            this.BtnSelect.TabIndex = 9;
            this.BtnSelect.Text = "Select";
            this.BtnSelect.UseVisualStyleBackColor = true;
            this.BtnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(96, 14);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(150, 15);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "No VISA Resources found.";
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(257, 44);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 11;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // VISAresourceForm
            // 
            this.AcceptButton = this.BtnSelect;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(344, 79);
            this.ControlBox = false;
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.BtnSelect);
            this.Controls.Add(this.listBoxResources);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(360, 118);
            this.Name = "VISAresourceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select VISA Resource";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ListBox listBoxResources;
        private System.Windows.Forms.Button BtnSelect;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Button BtnCancel;
    }
}