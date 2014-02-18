namespace Go2It
{
    partial class StartUpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartUpForm));
            this.btnBrowseProject = new System.Windows.Forms.Button();
            this.lstRecentProjects = new System.Windows.Forms.ListBox();
            this.rbOpenExistingProject = new System.Windows.Forms.RadioButton();
            this.rbEmptyProject = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.bsRecentFiles = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bsRecentFiles)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBrowseProject
            // 
            this.btnBrowseProject.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnBrowseProject.Location = new System.Drawing.Point(195, 3);
            this.btnBrowseProject.MaximumSize = new System.Drawing.Size(75, 23);
            this.btnBrowseProject.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnBrowseProject.Name = "btnBrowseProject";
            this.btnBrowseProject.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseProject.TabIndex = 0;
            this.btnBrowseProject.Text = "Browse";
            this.btnBrowseProject.UseVisualStyleBackColor = true;
            this.btnBrowseProject.Click += new System.EventHandler(this.btnBrowseProject_Click);
            // 
            // lstRecentProjects
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.lstRecentProjects, 3);
            this.lstRecentProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRecentProjects.FormattingEnabled = true;
            this.lstRecentProjects.Location = new System.Drawing.Point(3, 28);
            this.lstRecentProjects.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.lstRecentProjects.MinimumSize = new System.Drawing.Size(200, 50);
            this.lstRecentProjects.Name = "lstRecentProjects";
            this.lstRecentProjects.Size = new System.Drawing.Size(267, 183);
            this.lstRecentProjects.TabIndex = 1;
            this.lstRecentProjects.Click += new System.EventHandler(this.lstRecentProjects_Click);
            // 
            // rbOpenExistingProject
            // 
            this.rbOpenExistingProject.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.rbOpenExistingProject, 2);
            this.rbOpenExistingProject.Dock = System.Windows.Forms.DockStyle.Left;
            this.rbOpenExistingProject.Location = new System.Drawing.Point(3, 3);
            this.rbOpenExistingProject.Name = "rbOpenExistingProject";
            this.rbOpenExistingProject.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.rbOpenExistingProject.Size = new System.Drawing.Size(130, 22);
            this.rbOpenExistingProject.TabIndex = 2;
            this.rbOpenExistingProject.Text = "Open Existing Project";
            this.rbOpenExistingProject.UseVisualStyleBackColor = true;
            // 
            // rbEmptyProject
            // 
            this.rbEmptyProject.AutoSize = true;
            this.rbEmptyProject.Checked = true;
            this.tableLayoutPanel1.SetColumnSpan(this.rbEmptyProject, 3);
            this.rbEmptyProject.Dock = System.Windows.Forms.DockStyle.Left;
            this.rbEmptyProject.Location = new System.Drawing.Point(3, 214);
            this.rbEmptyProject.MinimumSize = new System.Drawing.Size(149, 17);
            this.rbEmptyProject.Name = "rbEmptyProject";
            this.rbEmptyProject.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.rbEmptyProject.Size = new System.Drawing.Size(149, 19);
            this.rbEmptyProject.TabIndex = 3;
            this.rbEmptyProject.TabStop = true;
            this.rbEmptyProject.Text = "Create Empty Project";
            this.rbEmptyProject.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOK.Location = new System.Drawing.Point(195, 239);
            this.btnOK.MaximumSize = new System.Drawing.Size(75, 28);
            this.btnOK.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.tableLayoutPanel1.Controls.Add(this.rbOpenExistingProject, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.rbEmptyProject, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lstRecentProjects, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnBrowseProject, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnOK, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(273, 269);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Location = new System.Drawing.Point(110, 239);
            this.btnCancel.MaximumSize = new System.Drawing.Size(75, 28);
            this.btnCancel.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // StartUpForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(273, 269);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(241, 176);
            this.Name = "StartUpForm";
            this.Text = "Go2It Project Loader";
            this.Load += new System.EventHandler(this.StartUpForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bsRecentFiles)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBrowseProject;
        private System.Windows.Forms.ListBox lstRecentProjects;
        private System.Windows.Forms.RadioButton rbOpenExistingProject;
        private System.Windows.Forms.RadioButton rbEmptyProject;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.BindingSource bsRecentFiles;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnCancel;
    }
}