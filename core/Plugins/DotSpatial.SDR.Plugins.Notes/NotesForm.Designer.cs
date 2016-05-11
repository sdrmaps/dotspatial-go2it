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
            this.layoutTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(192, 164);
            this.btnSave.MaximumSize = new System.Drawing.Size(75, 23);
            this.btnSave.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(111, 164);
            this.btnCancel.MaximumSize = new System.Drawing.Size(75, 23);
            this.btnCancel.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // layoutTable
            // 
            this.layoutTable.ColumnCount = 2;
            this.layoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.layoutTable.Controls.Add(this.btnCancel, 0, 1);
            this.layoutTable.Controls.Add(this.btnSave, 1, 1);
            this.layoutTable.Controls.Add(this.notesTable, 0, 0);
            this.layoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutTable.Location = new System.Drawing.Point(0, 0);
            this.layoutTable.Name = "layoutTable";
            this.layoutTable.RowCount = 2;
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layoutTable.Size = new System.Drawing.Size(270, 190);
            this.layoutTable.TabIndex = 0;
            // 
            // notesTable
            // 
            this.notesTable.ColumnCount = 2;
            this.layoutTable.SetColumnSpan(this.notesTable, 2);
            this.notesTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.notesTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.notesTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.notesTable.Location = new System.Drawing.Point(3, 3);
            this.notesTable.Name = "notesTable";
            this.notesTable.RowCount = 1;
            this.notesTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.notesTable.Size = new System.Drawing.Size(264, 155);
            this.notesTable.TabIndex = 3;
            // 
            // NotesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 190);
            this.Controls.Add(this.layoutTable);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NotesForm";
            this.Text = "Notes";
            this.layoutTable.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel layoutTable;
        private System.Windows.Forms.TableLayoutPanel notesTable;

    }
}