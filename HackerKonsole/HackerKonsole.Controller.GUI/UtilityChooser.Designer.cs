namespace HackerKonsole.Controller.GUI
{
    partial class UtilityChooser
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
            this.shellChooser = new System.Windows.Forms.Button();
            this.fileChooser = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // shellChooser
            // 
            this.shellChooser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.shellChooser.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.shellChooser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.shellChooser.Location = new System.Drawing.Point(13, 12);
            this.shellChooser.Name = "shellChooser";
            this.shellChooser.Size = new System.Drawing.Size(302, 45);
            this.shellChooser.TabIndex = 0;
            this.shellChooser.Text = "Shell";
            this.shellChooser.UseVisualStyleBackColor = false;
            this.shellChooser.Click += new System.EventHandler(this.button1_Click);
            // 
            // fileChooser
            // 
            this.fileChooser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fileChooser.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.fileChooser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.fileChooser.Location = new System.Drawing.Point(13, 73);
            this.fileChooser.Name = "fileChooser";
            this.fileChooser.Size = new System.Drawing.Size(302, 45);
            this.fileChooser.TabIndex = 1;
            this.fileChooser.Text = "File Transfer";
            this.fileChooser.UseVisualStyleBackColor = false;
            // 
            // UtilityChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(327, 131);
            this.Controls.Add(this.fileChooser);
            this.Controls.Add(this.shellChooser);
            this.Name = "UtilityChooser";
            this.Text = "UtilityChooser";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button shellChooser;
        private System.Windows.Forms.Button fileChooser;
    }
}