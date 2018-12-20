namespace CMWtests
{
    partial class VISAqueryForm
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
            this.components = new System.ComponentModel.Container();
            this.BtnQueryVISA = new System.Windows.Forms.Button();
            this.BtnWriteVISA = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.TextBoxStringToWrite = new System.Windows.Forms.TextBox();
            this.BtnConnectNew = new System.Windows.Forms.Button();
            this.LabelResource = new System.Windows.Forms.Label();
            this.BtnClose = new System.Windows.Forms.Button();
            this.TextBoxResponse = new System.Windows.Forms.TextBox();
            this.ContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnQueryVISA
            // 
            this.BtnQueryVISA.Location = new System.Drawing.Point(93, 90);
            this.BtnQueryVISA.Name = "BtnQueryVISA";
            this.BtnQueryVISA.Size = new System.Drawing.Size(75, 23);
            this.BtnQueryVISA.TabIndex = 12;
            this.BtnQueryVISA.Text = "Query";
            this.BtnQueryVISA.UseVisualStyleBackColor = true;
            this.BtnQueryVISA.Click += new System.EventHandler(this.BtnQueryVISA_Click);
            // 
            // BtnWriteVISA
            // 
            this.BtnWriteVISA.Location = new System.Drawing.Point(12, 90);
            this.BtnWriteVISA.Name = "BtnWriteVISA";
            this.BtnWriteVISA.Size = new System.Drawing.Size(75, 23);
            this.BtnWriteVISA.TabIndex = 11;
            this.BtnWriteVISA.Text = "Write";
            this.BtnWriteVISA.UseVisualStyleBackColor = true;
            this.BtnWriteVISA.Click += new System.EventHandler(this.BtnWriteVISA_Click);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(9, 48);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(71, 13);
            this.Label2.TabIndex = 10;
            this.Label2.Text = "String to write";
            // 
            // TextBoxStringToWrite
            // 
            this.TextBoxStringToWrite.AllowDrop = true;
            this.TextBoxStringToWrite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxStringToWrite.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TextBoxStringToWrite.Location = new System.Drawing.Point(12, 64);
            this.TextBoxStringToWrite.Name = "TextBoxStringToWrite";
            this.TextBoxStringToWrite.Size = new System.Drawing.Size(302, 20);
            this.TextBoxStringToWrite.TabIndex = 9;
            // 
            // BtnConnectNew
            // 
            this.BtnConnectNew.Location = new System.Drawing.Point(12, 12);
            this.BtnConnectNew.Name = "BtnConnectNew";
            this.BtnConnectNew.Size = new System.Drawing.Size(75, 23);
            this.BtnConnectNew.TabIndex = 14;
            this.BtnConnectNew.Text = "Connect...";
            this.BtnConnectNew.UseVisualStyleBackColor = true;
            this.BtnConnectNew.Click += new System.EventHandler(this.BtnConnectNew_Click);
            // 
            // LabelResource
            // 
            this.LabelResource.AutoSize = true;
            this.LabelResource.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelResource.Location = new System.Drawing.Point(93, 15);
            this.LabelResource.Name = "LabelResource";
            this.LabelResource.Size = new System.Drawing.Size(130, 15);
            this.LabelResource.TabIndex = 15;
            this.LabelResource.Text = "No Resource Selected";
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnClose.Location = new System.Drawing.Point(239, 325);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 16;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // TextBoxResponse
            // 
            this.TextBoxResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxResponse.ContextMenuStrip = this.ContextMenuStrip1;
            this.TextBoxResponse.Location = new System.Drawing.Point(12, 120);
            this.TextBoxResponse.Multiline = true;
            this.TextBoxResponse.Name = "TextBoxResponse";
            this.TextBoxResponse.Size = new System.Drawing.Size(302, 199);
            this.TextBoxResponse.TabIndex = 17;
            // 
            // ContextMenuStrip1
            // 
            this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CopyToolStripMenuItem});
            this.ContextMenuStrip1.Name = "ContextMenuStrip1";
            this.ContextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ContextMenuStrip1.ShowImageMargin = false;
            this.ContextMenuStrip1.Size = new System.Drawing.Size(78, 26);
            this.ContextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1_Opening);
            // 
            // CopyToolStripMenuItem
            // 
            this.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem";
            this.CopyToolStripMenuItem.Size = new System.Drawing.Size(77, 22);
            this.CopyToolStripMenuItem.Text = "&Copy";
            this.CopyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // VISAqueryForm
            // 
            this.AcceptButton = this.BtnQueryVISA;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnClose;
            this.ClientSize = new System.Drawing.Size(326, 360);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.TextBoxResponse);
            this.Controls.Add(this.LabelResource);
            this.Controls.Add(this.BtnConnectNew);
            this.Controls.Add(this.BtnQueryVISA);
            this.Controls.Add(this.BtnWriteVISA);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.TextBoxStringToWrite);
            this.Name = "VISAqueryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VISAquery";
            this.Load += new System.EventHandler(this.VISAquery_Load);
            this.ContextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnQueryVISA;
        private System.Windows.Forms.Button BtnWriteVISA;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.TextBox TextBoxStringToWrite;
        private System.Windows.Forms.Button BtnConnectNew;
        private System.Windows.Forms.Label LabelResource;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.TextBox TextBoxResponse;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem CopyToolStripMenuItem;
    }
}