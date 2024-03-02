namespace Practical.AI.SupervisedLearning.NeuralNetworks.HandwrittenDigitRecognition
{
    partial class HandwrittenRecognitionGui
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.paintBox = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cleanBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.trainBtn = new System.Windows.Forms.Button();
            this.classBox = new System.Windows.Forms.TextBox();
            this.classifyBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paintBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.paintBox);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(223, 143);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Drawing";
            // 
            // paintBox
            // 
            this.paintBox.BackColor = System.Drawing.Color.Black;
            this.paintBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paintBox.Location = new System.Drawing.Point(91, 58);
            this.paintBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.paintBox.Name = "paintBox";
            this.paintBox.Size = new System.Drawing.Size(39, 36);
            this.paintBox.TabIndex = 0;
            this.paintBox.TabStop = false;
            this.paintBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintBoxPaint);
            this.paintBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PaintBoxMouseDown);
            this.paintBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PaintBoxMouseMove);
            this.paintBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PaintBoxMouseUp);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cleanBtn);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.trainBtn);
            this.groupBox2.Controls.Add(this.classBox);
            this.groupBox2.Controls.Add(this.classifyBtn);
            this.groupBox2.Location = new System.Drawing.Point(247, 15);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(212, 143);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Neural Network";
            // 
            // cleanBtn
            // 
            this.cleanBtn.Location = new System.Drawing.Point(0, 71);
            this.cleanBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cleanBtn.Name = "cleanBtn";
            this.cleanBtn.Size = new System.Drawing.Size(203, 28);
            this.cleanBtn.TabIndex = 4;
            this.cleanBtn.Text = "Clean";
            this.cleanBtn.UseVisualStyleBackColor = true;
            this.cleanBtn.Click += new System.EventHandler(this.CleanBtnClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Classification";
            // 
            // trainBtn
            // 
            this.trainBtn.Location = new System.Drawing.Point(109, 107);
            this.trainBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.trainBtn.Name = "trainBtn";
            this.trainBtn.Size = new System.Drawing.Size(93, 27);
            this.trainBtn.TabIndex = 2;
            this.trainBtn.Text = "Train";
            this.trainBtn.UseVisualStyleBackColor = true;
            this.trainBtn.Click += new System.EventHandler(this.TrainBtnClick);
            // 
            // classBox
            // 
            this.classBox.Location = new System.Drawing.Point(107, 37);
            this.classBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.classBox.Name = "classBox";
            this.classBox.Size = new System.Drawing.Size(96, 22);
            this.classBox.TabIndex = 1;
            // 
            // classifyBtn
            // 
            this.classifyBtn.Location = new System.Drawing.Point(8, 107);
            this.classifyBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.classifyBtn.Name = "classifyBtn";
            this.classifyBtn.Size = new System.Drawing.Size(91, 28);
            this.classifyBtn.TabIndex = 0;
            this.classifyBtn.Text = "Classify";
            this.classifyBtn.UseVisualStyleBackColor = true;
            this.classifyBtn.Click += new System.EventHandler(this.ClassifyBtnClick);
            // 
            // HandwrittenRecognitionGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 170);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "HandwrittenRecognitionGui";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HandwrittenRecognitionGui";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.paintBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox paintBox;
        private System.Windows.Forms.TextBox classBox;
        private System.Windows.Forms.Button classifyBtn;
        private System.Windows.Forms.Button trainBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cleanBtn;
    }
}