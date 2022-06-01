namespace SpreadsheetGUI
{
    partial class SpreadsheetControlView
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
            this.spreadsheetPanel = new SS.SpreadsheetPanel();
            this.selectedCellNameLabel = new System.Windows.Forms.Label();
            this.selectedCellNameTextBox = new System.Windows.Forms.TextBox();
            this.selectedCellValueLabel = new System.Windows.Forms.Label();
            this.selectedCellValueTextBox = new System.Windows.Forms.TextBox();
            this.selectedCellContentLabel = new System.Windows.Forms.Label();
            this.selectedCellContentTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spreadsheetHelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creatingASpreadsheetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savingASpreadsheetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openingASpreadsheetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closingASpreadsheetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellHelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectingACellMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editingACellMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchForValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchForContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setCellBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.searchForContentToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // spreadsheetPanel
            // 
            this.spreadsheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetPanel.Location = new System.Drawing.Point(1, 97);
            this.spreadsheetPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spreadsheetPanel.Name = "spreadsheetPanel";
            this.spreadsheetPanel.Size = new System.Drawing.Size(643, 379);
            this.spreadsheetPanel.TabIndex = 0;
            // 
            // selectedCellNameLabel
            // 
            this.selectedCellNameLabel.AutoSize = true;
            this.selectedCellNameLabel.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedCellNameLabel.Location = new System.Drawing.Point(-4, 37);
            this.selectedCellNameLabel.Name = "selectedCellNameLabel";
            this.selectedCellNameLabel.Size = new System.Drawing.Size(40, 21);
            this.selectedCellNameLabel.TabIndex = 1;
            this.selectedCellNameLabel.Text = "Cell:";
            // 
            // selectedCellNameTextBox
            // 
            this.selectedCellNameTextBox.Enabled = false;
            this.selectedCellNameTextBox.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedCellNameTextBox.Location = new System.Drawing.Point(1, 58);
            this.selectedCellNameTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.selectedCellNameTextBox.Name = "selectedCellNameTextBox";
            this.selectedCellNameTextBox.Size = new System.Drawing.Size(38, 26);
            this.selectedCellNameTextBox.TabIndex = 2;
            // 
            // selectedCellValueLabel
            // 
            this.selectedCellValueLabel.AutoSize = true;
            this.selectedCellValueLabel.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedCellValueLabel.Location = new System.Drawing.Point(40, 37);
            this.selectedCellValueLabel.Name = "selectedCellValueLabel";
            this.selectedCellValueLabel.Size = new System.Drawing.Size(53, 21);
            this.selectedCellValueLabel.TabIndex = 3;
            this.selectedCellValueLabel.Text = "Value:";
            // 
            // selectedCellValueTextBox
            // 
            this.selectedCellValueTextBox.Enabled = false;
            this.selectedCellValueTextBox.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedCellValueTextBox.Location = new System.Drawing.Point(44, 58);
            this.selectedCellValueTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.selectedCellValueTextBox.Name = "selectedCellValueTextBox";
            this.selectedCellValueTextBox.Size = new System.Drawing.Size(211, 26);
            this.selectedCellValueTextBox.TabIndex = 4;
            // 
            // selectedCellContentLabel
            // 
            this.selectedCellContentLabel.AutoSize = true;
            this.selectedCellContentLabel.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedCellContentLabel.Location = new System.Drawing.Point(256, 37);
            this.selectedCellContentLabel.Name = "selectedCellContentLabel";
            this.selectedCellContentLabel.Size = new System.Drawing.Size(71, 21);
            this.selectedCellContentLabel.TabIndex = 5;
            this.selectedCellContentLabel.Text = "Content:";
            // 
            // selectedCellContentTextBox
            // 
            this.selectedCellContentTextBox.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedCellContentTextBox.Location = new System.Drawing.Point(260, 58);
            this.selectedCellContentTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.selectedCellContentTextBox.Name = "selectedCellContentTextBox";
            this.selectedCellContentTextBox.Size = new System.Drawing.Size(211, 26);
            this.selectedCellContentTextBox.TabIndex = 6;
            this.selectedCellContentTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SelectedCellContentTextBox_EnterPressed);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.menuStrip.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.helpMenuItem,
            this.searchMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(643, 29);
            this.menuStrip.TabIndex = 7;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.saveMenuItem,
            this.openMenuItem,
            this.closeMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(48, 25);
            this.fileMenuItem.Text = "File";
            // 
            // newMenuItem
            // 
            this.newMenuItem.Name = "newMenuItem";
            this.newMenuItem.Size = new System.Drawing.Size(131, 26);
            this.newMenuItem.Text = "New";
            this.newMenuItem.Click += new System.EventHandler(this.NewMenuItem_Click);
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.Size = new System.Drawing.Size(131, 26);
            this.saveMenuItem.Text = "Save";
            this.saveMenuItem.Click += new System.EventHandler(this.SaveMenuItem_Click);
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.Size = new System.Drawing.Size(131, 26);
            this.openMenuItem.Text = "Open";
            this.openMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Name = "closeMenuItem";
            this.closeMenuItem.Size = new System.Drawing.Size(131, 26);
            this.closeMenuItem.Text = "Close";
            this.closeMenuItem.Click += new System.EventHandler(this.CloseMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spreadsheetHelpMenuItem,
            this.cellHelpMenuItem,
            this.searchToolStripMenuItem,
            this.searchForContentToolStripMenuItem1});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(56, 25);
            this.helpMenuItem.Text = "Help";
            // 
            // spreadsheetHelpMenuItem
            // 
            this.spreadsheetHelpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.creatingASpreadsheetMenuItem,
            this.savingASpreadsheetMenuItem,
            this.openingASpreadsheetMenuItem,
            this.closingASpreadsheetMenuItem});
            this.spreadsheetHelpMenuItem.Name = "spreadsheetHelpMenuItem";
            this.spreadsheetHelpMenuItem.Size = new System.Drawing.Size(224, 26);
            this.spreadsheetHelpMenuItem.Text = "Spreadsheet";
            // 
            // creatingASpreadsheetMenuItem
            // 
            this.creatingASpreadsheetMenuItem.Name = "creatingASpreadsheetMenuItem";
            this.creatingASpreadsheetMenuItem.Size = new System.Drawing.Size(253, 26);
            this.creatingASpreadsheetMenuItem.Text = "Creating a spreadsheet";
            this.creatingASpreadsheetMenuItem.Click += new System.EventHandler(this.CreatingASpreadsheetMenuItem_Click);
            // 
            // savingASpreadsheetMenuItem
            // 
            this.savingASpreadsheetMenuItem.Name = "savingASpreadsheetMenuItem";
            this.savingASpreadsheetMenuItem.Size = new System.Drawing.Size(253, 26);
            this.savingASpreadsheetMenuItem.Text = "Saving a spreadsheet";
            this.savingASpreadsheetMenuItem.Click += new System.EventHandler(this.SavingASpreadsheetMenuItem_Click);
            // 
            // openingASpreadsheetMenuItem
            // 
            this.openingASpreadsheetMenuItem.Name = "openingASpreadsheetMenuItem";
            this.openingASpreadsheetMenuItem.Size = new System.Drawing.Size(253, 26);
            this.openingASpreadsheetMenuItem.Text = "Opening a spreadsheet";
            this.openingASpreadsheetMenuItem.Click += new System.EventHandler(this.OpeningASpreadsheetMenuItem_Click);
            // 
            // closingASpreadsheetMenuItem
            // 
            this.closingASpreadsheetMenuItem.Name = "closingASpreadsheetMenuItem";
            this.closingASpreadsheetMenuItem.Size = new System.Drawing.Size(253, 26);
            this.closingASpreadsheetMenuItem.Text = "Closing a spreadsheet";
            this.closingASpreadsheetMenuItem.Click += new System.EventHandler(this.ClosingASpreadsheetMenuItem_Click);
            // 
            // cellHelpMenuItem
            // 
            this.cellHelpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectingACellMenuItem,
            this.editingACellMenuItem});
            this.cellHelpMenuItem.Name = "cellHelpMenuItem";
            this.cellHelpMenuItem.Size = new System.Drawing.Size(224, 26);
            this.cellHelpMenuItem.Text = "Cells";
            // 
            // selectingACellMenuItem
            // 
            this.selectingACellMenuItem.Name = "selectingACellMenuItem";
            this.selectingACellMenuItem.Size = new System.Drawing.Size(194, 26);
            this.selectingACellMenuItem.Text = "Selecting a cell";
            this.selectingACellMenuItem.Click += new System.EventHandler(this.SelectingACellMenuItem_Click);
            // 
            // editingACellMenuItem
            // 
            this.editingACellMenuItem.Name = "editingACellMenuItem";
            this.editingACellMenuItem.Size = new System.Drawing.Size(194, 26);
            this.editingACellMenuItem.Text = "Editing a cell";
            this.editingACellMenuItem.Click += new System.EventHandler(this.EditingACellMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.searchToolStripMenuItem.Text = "Search for a value";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.SearchToolStripMenuItem_Click);
            // 
            // searchMenuItem
            // 
            this.searchMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchForValueToolStripMenuItem,
            this.searchForContentToolStripMenuItem});
            this.searchMenuItem.Name = "searchMenuItem";
            this.searchMenuItem.Size = new System.Drawing.Size(70, 25);
            this.searchMenuItem.Text = "Search";
            // 
            // searchForValueToolStripMenuItem
            // 
            this.searchForValueToolStripMenuItem.Name = "searchForValueToolStripMenuItem";
            this.searchForValueToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.searchForValueToolStripMenuItem.Text = "Search for value";
            this.searchForValueToolStripMenuItem.Click += new System.EventHandler(this.SearchForValueToolStripMenuItem_Click);
            // 
            // searchForContentToolStripMenuItem
            // 
            this.searchForContentToolStripMenuItem.Name = "searchForContentToolStripMenuItem";
            this.searchForContentToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.searchForContentToolStripMenuItem.Text = "Search for content";
            this.searchForContentToolStripMenuItem.Click += new System.EventHandler(this.SearchForContentToolStripMenuItem_Click);
            // 
            // setCellBackgroundWorker
            // 
            this.setCellBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.SetCellBackgroundWorker_DoWork);
            this.setCellBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.SetCellBackgroundWorker_RunWorkerCompleted);
            // 
            // searchForContentToolStripMenuItem1
            // 
            this.searchForContentToolStripMenuItem1.Name = "searchForContentToolStripMenuItem1";
            this.searchForContentToolStripMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.searchForContentToolStripMenuItem1.Text = "Search for content";
            this.searchForContentToolStripMenuItem1.Click += new System.EventHandler(this.SearchForContentToolStripMenuItem1_Click);
            // 
            // SpreadsheetControlView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 477);
            this.Controls.Add(this.selectedCellContentTextBox);
            this.Controls.Add(this.selectedCellContentLabel);
            this.Controls.Add(this.selectedCellValueTextBox);
            this.Controls.Add(this.selectedCellValueLabel);
            this.Controls.Add(this.selectedCellNameTextBox);
            this.Controls.Add(this.selectedCellNameLabel);
            this.Controls.Add(this.spreadsheetPanel);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(518, 429);
            this.Name = "SpreadsheetControlView";
            this.Text = "Spreadsheet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpreadsheetControlView_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel spreadsheetPanel;
        private System.Windows.Forms.Label selectedCellNameLabel;
        private System.Windows.Forms.TextBox selectedCellNameTextBox;
        private System.Windows.Forms.Label selectedCellValueLabel;
        private System.Windows.Forms.TextBox selectedCellValueTextBox;
        private System.Windows.Forms.Label selectedCellContentLabel;
        private System.Windows.Forms.TextBox selectedCellContentTextBox;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cellHelpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creatingASpreadsheetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savingASpreadsheetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openingASpreadsheetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closingASpreadsheetMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectingACellMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editingACellMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spreadsheetHelpMenuItem;
        private System.ComponentModel.BackgroundWorker setCellBackgroundWorker;
        private System.Windows.Forms.ToolStripMenuItem searchMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchForValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchForContentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchForContentToolStripMenuItem1;
    }
}

