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
            this.listBoxResources.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxResources.FormattingEnabled = true;
            this.listBoxResources.ItemHeight = 15;
            this.listBoxResources.Location = new System.Drawing.Point(12, 10);
            this.listBoxResources.Name = "listBoxResources";
            this.listBoxResources.Size = new System.Drawing.Size(256, 19);
            this.listBoxResources.TabIndex = 8;
            this.listBoxResources.MouseDoubleClick += new MouseEventHandler(ListBoxResources_DoubleClick);
            // 
            // BtnSelect
            // 
            this.BtnSelect.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BtnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnSelect.Location = new System.Drawing.Point(108, 42);
            this.BtnSelect.Name = "BtnSelect";
            this.BtnSelect.Size = new System.Drawing.Size(80, 23);
            this.BtnSelect.TabIndex = 9;
            this.BtnSelect.Text = "Select";
            this.BtnSelect.UseVisualStyleBackColor = true;
            this.BtnSelect.Click += new System.EventHandler(this.BtnSelect_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(56, 13);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(167, 16);
            this.Label1.TabIndex = 10;
            this.Label1.Text = "No VISA Resources found.";
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(194, 42);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 11;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // VISAresource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(281, 77);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.BtnSelect);
            this.Controls.Add(this.listBoxResources);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "VISAresource";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select VISA Resource";
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