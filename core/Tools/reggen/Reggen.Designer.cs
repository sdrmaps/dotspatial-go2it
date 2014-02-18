namespace reggen
{
    partial class Reggen
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
            this.txtIdent = new System.Windows.Forms.TextBox();
            this.txtReg = new System.Windows.Forms.TextBox();
            this.txtDays = new System.Windows.Forms.NumericUpDown();
            this.btnGen = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblExpire = new System.Windows.Forms.Label();
            this.lblDays = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtDays)).BeginInit();
            this.SuspendLayout();
            // 
            // txtIdent
            // 
            this.txtIdent.Location = new System.Drawing.Point(96, 11);
            this.txtIdent.Name = "txtIdent";
            this.txtIdent.Size = new System.Drawing.Size(341, 20);
            this.txtIdent.TabIndex = 0;
            // 
            // txtReg
            // 
            this.txtReg.Location = new System.Drawing.Point(96, 37);
            this.txtReg.Name = "txtReg";
            this.txtReg.ReadOnly = true;
            this.txtReg.Size = new System.Drawing.Size(341, 20);
            this.txtReg.TabIndex = 1;
            // 
            // txtDays
            // 
            this.txtDays.Location = new System.Drawing.Point(96, 63);
            this.txtDays.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.txtDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtDays.Name = "txtDays";
            this.txtDays.Size = new System.Drawing.Size(65, 20);
            this.txtDays.TabIndex = 2;
            this.txtDays.Value = new decimal(new int[] {
            785,
            0,
            0,
            0});
            // 
            // btnGen
            // 
            this.btnGen.Location = new System.Drawing.Point(362, 63);
            this.btnGen.Name = "btnGen";
            this.btnGen.Size = new System.Drawing.Size(75, 23);
            this.btnGen.TabIndex = 3;
            this.btnGen.Text = "Generate";
            this.btnGen.UseVisualStyleBackColor = true;
            this.btnGen.Click += new System.EventHandler(this.btnGen_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Identification:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Registration:";
            // 
            // lblExpire
            // 
            this.lblExpire.AutoSize = true;
            this.lblExpire.Location = new System.Drawing.Point(7, 65);
            this.lblExpire.Name = "lblExpire";
            this.lblExpire.Size = new System.Drawing.Size(83, 13);
            this.lblExpire.TabIndex = 6;
            this.lblExpire.Text = "Expiration Days:";
            // 
            // lblDays
            // 
            this.lblDays.AutoSize = true;
            this.lblDays.Location = new System.Drawing.Point(167, 65);
            this.lblDays.Name = "lblDays";
            this.lblDays.Size = new System.Drawing.Size(112, 13);
            this.lblDays.TabIndex = 7;
            this.lblDays.Text = "( 785 = No Expiration )";
            // 
            // Reggen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 98);
            this.Controls.Add(this.lblDays);
            this.Controls.Add(this.lblExpire);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGen);
            this.Controls.Add(this.txtDays);
            this.Controls.Add(this.txtReg);
            this.Controls.Add(this.txtIdent);
            this.Name = "Reggen";
            this.Text = "SDR Registration Generator";
            ((System.ComponentModel.ISupportInitialize)(this.txtDays)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtIdent;
        private System.Windows.Forms.TextBox txtReg;
        private System.Windows.Forms.NumericUpDown txtDays;
        private System.Windows.Forms.Button btnGen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblExpire;
        private System.Windows.Forms.Label lblDays;
    }
}

