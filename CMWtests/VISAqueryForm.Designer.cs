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
            this.btnQueryVISA = new System.Windows.Forms.Button();
            this.btnWriteVISA = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxStringToWrite = new System.Windows.Forms.TextBox();
            this.btnConnectNew = new System.Windows.Forms.Button();
            this.labelResource = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.textBoxResponse = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnQueryVISA
            // 
            this.btnQueryVISA.Location = new System.Drawing.Point(93, 90);
            this.btnQueryVISA.Name = "btnQueryVISA";
            this.btnQueryVISA.Size = new System.Drawing.Size(75, 23);
            this.btnQueryVISA.TabIndex = 12;
            this.btnQueryVISA.Text = "Query";
            this.btnQueryVISA.UseVisualStyleBackColor = true;
            this.btnQueryVISA.Click += new System.EventHandler(this.btnQueryVISA_Click);
            // 
            // btnWriteVISA
            // 
            this.btnWriteVISA.Location = new System.Drawing.Point(12, 90);
            this.btnWriteVISA.Name = "btnWriteVISA";
            this.btnWriteVISA.Size = new System.Drawing.Size(75, 23);
            this.btnWriteVISA.TabIndex = 11;
            this.btnWriteVISA.Text = "Write";
            this.btnWriteVISA.UseVisualStyleBackColor = true;
            this.btnWriteVISA.Click += new System.EventHandler(this.btnWriteVISA_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "String to write";
            // 
            // textBoxStringToWrite
            // 
            this.textBoxStringToWrite.AllowDrop = true;
            this.textBoxStringToWrite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStringToWrite.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxStringToWrite.Location = new System.Drawing.Point(12, 64);
            this.textBoxStringToWrite.Name = "textBoxStringToWrite";
            this.textBoxStringToWrite.Size = new System.Drawing.Size(287, 20);
            this.textBoxStringToWrite.TabIndex = 9;
            // 
            // btnConnectNew
            // 
            this.btnConnectNew.Location = new System.Drawing.Point(12, 12);
            this.btnConnectNew.Name = "btnConnectNew";
            this.btnConnectNew.Size = new System.Drawing.Size(75, 23);
            this.btnConnectNew.TabIndex = 14;
            this.btnConnectNew.Text = "Connect...";
            this.btnConnectNew.UseVisualStyleBackColor = true;
            this.btnConnectNew.Click += new System.EventHandler(this.btnConnectNew_Click);
            // 
            // labelResource
            // 
            this.labelResource.AutoSize = true;
            this.labelResource.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResource.Location = new System.Drawing.Point(93, 15);
            this.labelResource.Name = "labelResource";
            this.labelResource.Size = new System.Drawing.Size(130, 15);
            this.labelResource.TabIndex = 15;
            this.labelResource.Text = "No Resource Selected";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(224, 288);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // textBoxResponse
            // 
            this.textBoxResponse.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResponse.Location = new System.Drawing.Point(12, 120);
            this.textBoxResponse.Multiline = true;
            this.textBoxResponse.Name = "textBoxResponse";
            this.textBoxResponse.Size = new System.Drawing.Size(287, 154);
            this.textBoxResponse.TabIndex = 17;
            // 
            // VISAqueryForm
            // 
            this.AcceptButton = this.btnQueryVISA;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(311, 323);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.textBoxResponse);
            this.Controls.Add(this.labelResource);
            this.Controls.Add(this.btnConnectNew);
            this.Controls.Add(this.btnQueryVISA);
            this.Controls.Add(this.btnWriteVISA);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxStringToWrite);
            this.Name = "VISAqueryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VISAquery";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnQueryVISA;
        private System.Windows.Forms.Button btnWriteVISA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxStringToWrite;
        private System.Windows.Forms.Button btnConnectNew;
        private System.Windows.Forms.Label labelResource;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox textBoxResponse;
    }
}