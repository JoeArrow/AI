namespace LinRegOnnxRunner
{
    partial class AIPredForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            numX = new NumericUpDown();
            btnPredict = new Button();
            txtY = new TextBox();
            ((System.ComponentModel.ISupportInitialize)numX).BeginInit();
            SuspendLayout();
            // 
            // numX
            // 
            numX.Location = new Point(57, 40);
            numX.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numX.Name = "numX";
            numX.Size = new Size(120, 23);
            numX.TabIndex = 0;
            // 
            // btnPredict
            // 
            btnPredict.Location = new Point(80, 69);
            btnPredict.Name = "btnPredict";
            btnPredict.Size = new Size(75, 23);
            btnPredict.TabIndex = 1;
            btnPredict.Text = "Predict";
            btnPredict.UseVisualStyleBackColor = true;
            btnPredict.Click += btnPredict_Click;
            // 
            // txtY
            // 
            txtY.Location = new Point(67, 98);
            txtY.Name = "txtY";
            txtY.ReadOnly = true;
            txtY.Size = new Size(100, 23);
            txtY.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(227, 154);
            Controls.Add(txtY);
            Controls.Add(btnPredict);
            Controls.Add(numX);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AI Pred";
            ((System.ComponentModel.ISupportInitialize)numX).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown numX;
        private Button btnPredict;
        private TextBox txtY;
    }
}
