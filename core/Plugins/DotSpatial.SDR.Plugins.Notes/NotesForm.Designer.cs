namespace DotSpatial.SDR.Plugins.Notes
{
    partial class NotesForm
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.layoutTable = new System.Windows.Forms.TableLayoutPanel();
            this.notesTable = new System.Windows.Forms.TableLayoutPanel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.layoutTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(228, 33);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.btnSave.MaximumSize = new System.Drawing.Size(75, 23);
            this.btnSave.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(147, 33);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.btnCancel.MaximumSize = new System.Drawing.Size(75, 23);
            this.btnCancel.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // layoutTable
            // 
            this.layoutTable.AutoSize = true;
            this.layoutTable.ColumnCount = 3;
            this.layoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutTable.Controls.Add(this.btnCancel, 1, 1);
            this.layoutTable.Controls.Add(this.btnSave, 2, 1);
            this.layoutTable.Controls.Add(this.notesTable, 0, 0);
            this.layoutTable.Controls.Add(this.btnDelete, 0, 1);
            this.layoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutTable.Location = new System.Drawing.Point(0, 0);
            this.layoutTable.Name = "layoutTable";
            this.layoutTable.RowCount = 2;
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutTable.Size = new System.Drawing.Size(306, 59);
            this.layoutTable.TabIndex = 0;
            // 
            // notesTable
            // 
            this.notesTable.AutoSize = true;
            this.notesTable.ColumnCount = 2;
            this.layoutTable.SetColumnSpan(this.notesTable, 3);
            this.notesTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.notesTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.notesTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notesTable.Location = new System.Drawing.Point(3, 3);
            this.notesTable.Name = "notesTable";
            this.notesTable.RowCount = 1;
            this.notesTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.notesTable.Size = new System.Drawing.Size(300, 19);
            this.notesTable.TabIndex = 3;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(3, 33);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.btnDelete.MaximumSize = new System.Drawing.Size(75, 23);
            this.btnDelete.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // NotesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(306, 59);
            this.Controls.Add(this.layoutTable);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NotesForm";
            this.Text = "Notes";
            this.layoutTable.ResumeLayout(false);
            this.layoutTable.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel layoutTable;
        private System.Windows.Forms.TableLayoutPanel notesTable;
        private System.Windows.Forms.Button btnDelete;

    }
}