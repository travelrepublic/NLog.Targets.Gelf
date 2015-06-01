namespace TestApp
{
    partial class Form1
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
            this.buttonTrace = new System.Windows.Forms.Button();
            this.buttonDebug = new System.Windows.Forms.Button();
            this.buttonInfo = new System.Windows.Forms.Button();
            this.buttonWarn = new System.Windows.Forms.Button();
            this.buttonError = new System.Windows.Forms.Button();
            this.buttonFatal = new System.Windows.Forms.Button();
            this.buttonRandomMessage = new System.Windows.Forms.Button();
            this.tbxRandomMessageSize = new System.Windows.Forms.TextBox();
            this.lblRandomMessageSize = new System.Windows.Forms.Label();
            this.buttonCustomMessage = new System.Windows.Forms.Button();
            this.tbxCustomMessage = new System.Windows.Forms.TextBox();
            this.buttonAggregateError = new System.Windows.Forms.Button();
            this.buttonInnerError = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonTrace
            // 
            this.buttonTrace.Location = new System.Drawing.Point(34, 26);
            this.buttonTrace.Name = "buttonTrace";
            this.buttonTrace.Size = new System.Drawing.Size(212, 38);
            this.buttonTrace.TabIndex = 0;
            this.buttonTrace.Text = "Trace Event";
            this.buttonTrace.UseVisualStyleBackColor = true;
            this.buttonTrace.Click += new System.EventHandler(this.ButtonLog_Click);
            // 
            // buttonDebug
            // 
            this.buttonDebug.Location = new System.Drawing.Point(34, 70);
            this.buttonDebug.Name = "buttonDebug";
            this.buttonDebug.Size = new System.Drawing.Size(212, 38);
            this.buttonDebug.TabIndex = 1;
            this.buttonDebug.Text = "Debug Event";
            this.buttonDebug.UseVisualStyleBackColor = true;
            this.buttonDebug.Click += new System.EventHandler(this.ButtonLog_Click);
            // 
            // buttonInfo
            // 
            this.buttonInfo.Location = new System.Drawing.Point(34, 114);
            this.buttonInfo.Name = "buttonInfo";
            this.buttonInfo.Size = new System.Drawing.Size(212, 38);
            this.buttonInfo.TabIndex = 2;
            this.buttonInfo.Text = "Info Event";
            this.buttonInfo.UseVisualStyleBackColor = true;
            this.buttonInfo.Click += new System.EventHandler(this.ButtonLog_Click);
            // 
            // buttonWarn
            // 
            this.buttonWarn.Location = new System.Drawing.Point(34, 158);
            this.buttonWarn.Name = "buttonWarn";
            this.buttonWarn.Size = new System.Drawing.Size(212, 38);
            this.buttonWarn.TabIndex = 3;
            this.buttonWarn.Text = "Warn Event";
            this.buttonWarn.UseVisualStyleBackColor = true;
            this.buttonWarn.Click += new System.EventHandler(this.ButtonLog_Click);
            // 
            // buttonAggregateError
            // 
            this.buttonAggregateError.Location = new System.Drawing.Point(34, 205);
            this.buttonAggregateError.Name = "buttonAggregateError";
            this.buttonAggregateError.Size = new System.Drawing.Size(212, 38);
            this.buttonAggregateError.TabIndex = 4;
            this.buttonAggregateError.Text = "Aggregate Error Event";
            this.buttonAggregateError.UseVisualStyleBackColor = true;
            this.buttonAggregateError.Click += new System.EventHandler(this.ButtonLog_Click);
            // 
            // buttonInnerError
            // 
            this.buttonInnerError.Location = new System.Drawing.Point(34, 248);
            this.buttonInnerError.Name = "buttonInnerError";
            this.buttonInnerError.Size = new System.Drawing.Size(212, 38);
            this.buttonInnerError.TabIndex = 5;
            this.buttonInnerError.Text = "Inner Error Event";
            this.buttonInnerError.UseVisualStyleBackColor = true;
            this.buttonInnerError.Click += new System.EventHandler(this.ButtonLog_Click);
            // 
            // buttonError
            // 
            this.buttonError.Location = new System.Drawing.Point(34, 291);
            this.buttonError.Name = "buttonError";
            this.buttonError.Size = new System.Drawing.Size(212, 38);
            this.buttonError.TabIndex = 6;
            this.buttonError.Text = "Error Event";
            this.buttonError.UseVisualStyleBackColor = true;
            this.buttonError.Click += new System.EventHandler(this.ButtonLog_Click);
            // 
            // buttonFatal
            // 
            this.buttonFatal.Location = new System.Drawing.Point(34, 335);
            this.buttonFatal.Name = "buttonFatal";
            this.buttonFatal.Size = new System.Drawing.Size(212, 38);
            this.buttonFatal.TabIndex = 7;
            this.buttonFatal.Text = "Fatal Event";
            this.buttonFatal.UseVisualStyleBackColor = true;
            this.buttonFatal.Click += new System.EventHandler(this.ButtonLog_Click);
            // 
            // buttonRandomMessage
            // 
            this.buttonRandomMessage.Location = new System.Drawing.Point(34, 379);
            this.buttonRandomMessage.Name = "buttonRandomMessage";
            this.buttonRandomMessage.Size = new System.Drawing.Size(212, 38);
            this.buttonRandomMessage.TabIndex = 8;
            this.buttonRandomMessage.Text = "Send Random Message";
            this.buttonRandomMessage.UseVisualStyleBackColor = true;
            this.buttonRandomMessage.Click += new System.EventHandler(this.ButtonLog_Click);
            // 
            // tbxRandomMessageSize
            // 
            this.tbxRandomMessageSize.Location = new System.Drawing.Point(153, 423);
            this.tbxRandomMessageSize.MaxLength = 7;
            this.tbxRandomMessageSize.Name = "tbxRandomMessageSize";
            this.tbxRandomMessageSize.Size = new System.Drawing.Size(93, 20);
            this.tbxRandomMessageSize.TabIndex = 9;
            // 
            // lblRandomMessageSize
            // 
            this.lblRandomMessageSize.AutoSize = true;
            this.lblRandomMessageSize.Location = new System.Drawing.Point(31, 426);
            this.lblRandomMessageSize.Name = "lblRandomMessageSize";
            this.lblRandomMessageSize.Size = new System.Drawing.Size(116, 13);
            this.lblRandomMessageSize.TabIndex = 10;
            this.lblRandomMessageSize.Text = "Random Message Size";
            // 
            // buttonCustomMessage
            // 
            this.buttonCustomMessage.Location = new System.Drawing.Point(34, 449);
            this.buttonCustomMessage.Name = "buttonCustomMessage";
            this.buttonCustomMessage.Size = new System.Drawing.Size(212, 38);
            this.buttonCustomMessage.TabIndex = 11;
            this.buttonCustomMessage.Text = "Send Custom Message";
            this.buttonCustomMessage.UseVisualStyleBackColor = true;
            this.buttonCustomMessage.Click += new System.EventHandler(this.ButtonLog_Click);
            // 
            // tbxCustomMessage
            // 
            this.tbxCustomMessage.Location = new System.Drawing.Point(34, 492);
            this.tbxCustomMessage.Multiline = true;
            this.tbxCustomMessage.Name = "tbxCustomMessage";
            this.tbxCustomMessage.Size = new System.Drawing.Size(212, 148);
            this.tbxCustomMessage.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 652);
            this.Controls.Add(this.buttonInnerError);
            this.Controls.Add(this.buttonAggregateError);
            this.Controls.Add(this.tbxCustomMessage);
            this.Controls.Add(this.buttonCustomMessage);
            this.Controls.Add(this.lblRandomMessageSize);
            this.Controls.Add(this.tbxRandomMessageSize);
            this.Controls.Add(this.buttonRandomMessage);
            this.Controls.Add(this.buttonFatal);
            this.Controls.Add(this.buttonError);
            this.Controls.Add(this.buttonWarn);
            this.Controls.Add(this.buttonInfo);
            this.Controls.Add(this.buttonDebug);
            this.Controls.Add(this.buttonTrace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Syslog Target Demo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonTrace;
        private System.Windows.Forms.Button buttonDebug;
        private System.Windows.Forms.Button buttonInfo;
        private System.Windows.Forms.Button buttonWarn;
        private System.Windows.Forms.Button buttonError;
        private System.Windows.Forms.Button buttonFatal;
        private System.Windows.Forms.Button buttonRandomMessage;
        private System.Windows.Forms.TextBox tbxRandomMessageSize;
        private System.Windows.Forms.Label lblRandomMessageSize;
        private System.Windows.Forms.Button buttonCustomMessage;
        private System.Windows.Forms.TextBox tbxCustomMessage;
        private System.Windows.Forms.Button buttonAggregateError;
        private System.Windows.Forms.Button buttonInnerError;
    }
}