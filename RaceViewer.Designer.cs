namespace MarkovDecisionProcess
{
    partial class RaceViewer
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
            this.TrackPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.TrackPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // TrackPictureBox
            // 
            this.TrackPictureBox.Location = new System.Drawing.Point(12, 12);
            this.TrackPictureBox.Name = "TrackPictureBox";
            this.TrackPictureBox.Size = new System.Drawing.Size(331, 314);
            this.TrackPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.TrackPictureBox.TabIndex = 0;
            this.TrackPictureBox.TabStop = false;
            this.TrackPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.TrackPictureBox_Paint);
            // 
            // RaceViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 338);
            this.Controls.Add(this.TrackPictureBox);
            this.Name = "RaceViewer";
            this.Text = "RaceViewer";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RaceViewer_Paint);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RaceViewer_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.TrackPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox TrackPictureBox;

    }
}