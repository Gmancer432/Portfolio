// Written by Aidan Lethaby, September 2019

using Microsoft.VisualBasic;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// This class holds the event handlers and helper methods that contain the logic for the spreadsheet GUI.
    /// </summary>
    public partial class SpreadsheetControlView : Form
    {
        /// <summary>
        /// The spreadsheet that the current form represents.
        /// </summary>
        private AbstractSpreadsheet sheet;

        /// <summary>
        /// The current filepath this spreadsheet is assosiated with, if any.
        /// </summary>
        private string path;

        /// <summary>
        /// Sets up everything.
        /// 
        /// The backing spreadsheet, path, and some manually subscribed handlers.
        /// </summary>
        public SpreadsheetControlView()
        {
            InitializeComponent();
            sheet = new Spreadsheet(Validator, s => s = s.ToUpper(), "ps6");
            path = null;

            spreadsheetPanel.SelectionChanged += SelectionChangedHandler;
            Shown += ShownHandler;
        }

        //Write handlers here

        /// <summary>
        /// Handler for when the form if intially create.
        /// 
        /// Puts focus on the selectedCellContentTextBox so automatically selected cell A1 is ready to be edited.
        /// </summary>
        private void ShownHandler(object sender, EventArgs e)
        {
            selectedCellContentTextBox.Focus();
            GetSelectedCell();
        }

        /// <summary>
        /// Handler for when cell selection changes.
        /// </summary>
        private void SelectionChangedHandler(SpreadsheetPanel sp)
        {
            selectedCellContentTextBox.Focus();
            GetSelectedCell();
        }

        /// <summary>
        /// Handler for when the enter key is pressed while focus is on the selectedCellContentTextBox.
        /// </summary>
        private void SelectedCellContentTextBox_EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Set up for multithreading
                selectedCellContentTextBox.Enabled = false;
                saveMenuItem.Enabled = false;
                openMenuItem.Enabled = false;
                searchMenuItem.Enabled = false;
                spreadsheetPanel.GetSelection(out int col, out int row);
                Tuple<string, int, int> args = new Tuple<string, int, int>(selectedCellContentTextBox.Text, col, row);
                //Start multithreading
                setCellBackgroundWorker.RunWorkerAsync(args);
            }
        }

        /// <summary>
        /// Handle for when the background worker to set cells is called.
        /// </summary>
        private void SetCellBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Tuple<string, int, int> args = (Tuple<string, int, int>)e.Argument;
            SetSelectedCell(args.Item1, args.Item2, args.Item3);
        }

        /// <summary>
        /// Handler for when cell selection changes.
        /// </summary>
        private void SetCellBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ///Reenable fields after multithreading
            GetSelectedCell();
            selectedCellContentTextBox.Enabled = true;
            saveMenuItem.Enabled = true;
            openMenuItem.Enabled = true;
            searchMenuItem.Enabled = true;
        }

        /// <summary>
        /// Handler for when File>New is clicked.
        /// </summary>
        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetGUIApplicationContext.GetAppContext().RunForm(new SpreadsheetControlView());
        }

        /// <summary>
        /// Handler for when File>Open is clicked.
        /// </summary>
        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenExistingSpreadsheet();
        }

        /// <summary>
        /// Handler for when File>Save is clicked.
        /// </summary>
        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveToExistingSpreadsheet();
        }

        /// <summary>
        /// Handler for when File>Close is clicked.
        /// </summary>
        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (sheet.Changed == true)
            {
                if (MessageBox.Show("Closing will result in a loss of unsaved data. Do you want to continue?", "Unsaved data", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
            }
            Close();
        }

        /// <summary>
        /// Handler for when the forms "X" (close window) button is clicked.
        /// </summary>
        private void SpreadsheetControlView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sheet.Changed == true)
            {
                if (MessageBox.Show("Closing will result in a loss of unsaved data. Do you want to continue?", "Unsaved data", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// Handler for when Help>Spreadsheet>Creating a spreadsheet is clicked.
        /// </summary>
        private void CreatingASpreadsheetMenuItem_Click(object sender, EventArgs e)
        {
            ShowCreatingASpreadsheetHelp();
        }

        /// <summary>
        /// Handler for when Help>Spreadsheet>Saving a spreadsheet is clicked.
        /// </summary>
        private void SavingASpreadsheetMenuItem_Click(object sender, EventArgs e)
        {
            ShowSavingASpreadsheetHelp();
        }

        /// <summary>
        /// Handler for when Help>Spreadsheet>Opening a spreadsheet is clicked.
        /// </summary>
        private void OpeningASpreadsheetMenuItem_Click(object sender, EventArgs e)
        {
            ShowOpenASpreadsheetHelp();
        }

        /// <summary>
        /// Handler for when Help>Spreadsheet>Closing a spreadsheet is clicked.
        /// </summary>
        private void ClosingASpreadsheetMenuItem_Click(object sender, EventArgs e)
        {
            ShowCloseASpreadsheetHelp();
        }

        /// <summary>
        /// Handler for when Help>Cell>Editing a cell is clicked.
        /// </summary>
        private void EditingACellMenuItem_Click(object sender, EventArgs e)
        {
            ShowEditingACellHelp();
        }

        /// <summary>
        /// Handler for when Help>Cell>Selecting a cell is clicked.
        /// </summary>
        private void SelectingACellMenuItem_Click(object sender, EventArgs e)
        {
            ShowSelectingACellHelp();
        }

        /// <summary>
        /// Handler for when Help>Search for a value is clicked.
        /// </summary>
        private void SearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSearchForValueHelp();
        }

        /// <summary>
        /// Handler for when Help>Search for content is clicked.
        /// </summary>
        private void SearchForContentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowSearchForContentHelp();
        }

        /// <summary>
        /// Handler for when Search for value is clicked.
        /// </summary>
        private void SearchForValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForValue();
        }

        /// <summary>
        /// Handler for when Search for content is clicked.
        /// </summary>
        private void SearchForContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForContent();
        }

        //Write helpers here

        /// <summary>
        /// Get the name, value, and content of the selected cell to display on the GUI.
        /// </summary>
        private void GetSelectedCell()
        {
            spreadsheetPanel.GetSelection(out int col, out int row);
            string cellName = CoordsToCell(col, row);
            selectedCellNameTextBox.Text = cellName;

            if (sheet.GetCellValue(cellName) is FormulaError)
                selectedCellValueTextBox.Text = ((FormulaError)sheet.GetCellValue(cellName)).Reason;
            else if (sheet.GetCellValue(cellName) is double)
                selectedCellValueTextBox.Text = sheet.GetCellValue(cellName).ToString();
            else if (sheet.GetCellValue(cellName) is string)
                selectedCellValueTextBox.Text = (string)sheet.GetCellValue(cellName);

            if (sheet.GetCellContents(cellName) is Formula)
                selectedCellContentTextBox.Text = "=" + sheet.GetCellContents(cellName).ToString();
            else if (sheet.GetCellContents(cellName) is double)
                selectedCellContentTextBox.Text = sheet.GetCellContents(cellName).ToString();
            else if (sheet.GetCellContents(cellName) is string)
                selectedCellContentTextBox.Text = (string)sheet.GetCellContents(cellName);
        }

        /// <summary>
        /// Sets the selected cell and any dependent cells ounce a cells contents is changed.
        /// </summary>
        private void SetSelectedCell(string content, int col, int row)
        {
            try
            {
                ResetCells(sheet.SetContentsOfCell(CoordsToCell(col, row), content));
            }
            catch (CircularException)
            {
                MessageBox.Show("Circular exception! File trying to load from contained circular logic!", "Exception", 0, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Exception", 0, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Opens a specified file and populates the spreadsheet with the spreadsheet contained in that file.
        /// 
        /// Displays a message if opening the file would cause a loss of data or there is trouble reading the file chosen.
        /// </summary>
        private void OpenExistingSpreadsheet()
        {
            if (sheet.Changed == true)
            {
                if (MessageBox.Show("Opening a saved file will result in a loss of unsaved data. Do you want to continue?", "Unsaved data", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    path = openFileDialog.SafeFileName;

                try
                {
                    sheet = new Spreadsheet(path, Validator, s => s = s.ToUpper(), "ps6");
                }
                catch (CircularException)
                {
                    path = null;
                    MessageBox.Show("Circular exception! File trying to load from contained circular logic!", "Exception", 0, MessageBoxIcon.Error);
                }
                catch (Exception e)
                {
                    path = null;
                    MessageBox.Show(e.Message, "Exception", 0, MessageBoxIcon.Error);
                }

                ResetCells(sheet.GetNamesOfAllNonemptyCells());
                GetSelectedCell();
            }
        }

        /// <summary>
        /// Saves the current spreadsheet to the chosen file or newly created file.
        /// 
        /// Displays a message if saving would overwrite any existing file or there is trouble writing to the chosen file.
        /// </summary>
        private void SaveToExistingSpreadsheet()
        {
            if (path == null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        path = saveFileDialog.FileName;
                    if (!path.EndsWith(".sprd") && saveFileDialog.Filter == "sprd file (*.sprd)|*.sprd")
                        path += ".sprd";
                }

                if (File.Exists(path))
                {
                    if (MessageBox.Show("Saving will result in a overwritting of existing file's data. Do you want to continue?", "Overwritting data", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        return;
                }
                else
                {
                    using (File.Create(path)) { };
                }
            }
            try
            {
                sheet.Save(path);
            }
            catch (SpreadsheetReadWriteException e)
            {
                MessageBox.Show(e.Message, "Exception", 0, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Finds how many and which cells contain the specified value the user inputs.
        /// 
        /// Displays a prompt for input value and number and list of cells once computed.
        /// </summary>
        private void SearchForValue()
        {
            string input = Interaction.InputBox("Enter the value you would like to search for then press \"OK\"", "Search value", "", -1, -1);
            string containValue = "";
            int containCount = 0;

            if (input.Trim() == "")
                return;

            foreach (string s in sheet.GetNamesOfAllNonemptyCells())
            {
                if (!(sheet.GetCellValue(s) is FormulaError))
                {
                    if (sheet.GetCellValue(s) is double)
                    {
                        if (sheet.GetCellValue(s).ToString() == input)
                        {
                            containCount++;
                            containValue += s + " ";
                        }
                    }
                    if (sheet.GetCellValue(s) is string)
                    {
                        if ((string)sheet.GetCellValue(s) == input)
                        {
                            containCount++;
                            containValue += s + " ";
                        }

                    }
                }
            }
            if (containCount == 1)
                MessageBox.Show("1 cell contains \"" + input + "\": " + containValue, "Search value", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(containCount + " cells contain \"" + input + "\": " + containValue, "Search value", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Finds how many and which cells contain the specified content the user inputs.
        /// 
        /// Displays a prompt for input value and number and list of cells once computed.
        /// </summary>
        private void SearchForContent()
        {
            string input = Interaction.InputBox("Enter the content you would like to search for then press \"OK\"", "Search content", "", -1, -1);
            string containContent = "";
            int containCount = 0;
            Formula inputF = null;

            if (input.Trim() == "")
                return;
            if (input.StartsWith("="))
                inputF = new Formula(input.Substring(1));

            foreach (string s in sheet.GetNamesOfAllNonemptyCells())
            {
                if (sheet.GetCellContents(s) is Formula && inputF != null)
                {
                    if ((Formula)sheet.GetCellContents(s) == inputF)
                    {
                        containCount++;
                        containContent += s + " ";
                    }
                }
                else if (sheet.GetCellContents(s) is double && Double.TryParse(input, out double inputD))
                {
                    if ((double)sheet.GetCellContents(s) == inputD)
                    {
                        containCount++;
                        containContent += s + " ";
                    }
                }
                else if (sheet.GetCellContents(s) is string)
                {
                    if ((string)sheet.GetCellContents(s) == input)
                    {
                        containCount++;
                        containContent += s + " ";
                    }

                }
            }
            if (containCount == 1)
                MessageBox.Show("1 cell contains \"" + input + "\": " + containContent, "Search content", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(containCount + " cells contain \"" + input + "\": " + containContent, "Search content", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays help message for editing a cell.
        /// </summary>
        private void ShowEditingACellHelp()
        {
            MessageBox.Show("Edit the selected cell by typing the desired contents in the \"Content:\" text box and then pressing the \"Enter\" key. You can set the contents to regular text, a number, or a formula (type \"=\" and then the expression you would like to be evaluated).", "Editing a cell", 0, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays help message for selecting a cell.
        /// </summary>
        private void ShowSelectingACellHelp()
        {
            MessageBox.Show("Select a cell by clicking it on the grid.", "Selecting a cell", 0, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays help message for creating a spreadsheet.
        /// </summary>
        private void ShowCreatingASpreadsheetHelp()
        {
            MessageBox.Show("Create a new spreadsheet by clicking File > New.", "Creating a spreadsheet", 0, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays help message for saving a spreadsheet.
        /// </summary>
        private void ShowSavingASpreadsheetHelp()
        {
            MessageBox.Show("Save a spreadsheet by clicking File > Save. If the current spreadsheet was not loaded from an existing file, use the file explorer window to find a file to save to or enter a name to create a new file. If the file explorer is set to .sprd files, any new files created will be .sprd files. To save to another type of file, such as .txt, select \"All files\".", "Saving a spreadsheet", 0, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays help message for opening a spreadsheet.
        /// </summary>
        private void ShowOpenASpreadsheetHelp()
        {
            MessageBox.Show("Open an existing spreadsheet by clicking File > Open. Use the file explorer window to find a file. File finding is automatically limited to .sprd file. If you would like to use another type of file, such as .txt, then select \"All files\" in the file explorer.", "Opening a spreadsheet", 0, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays help message for closing a spreadsheet.
        /// </summary>
        private void ShowCloseASpreadsheetHelp()
        {
            MessageBox.Show("Close the current spreadsheet by clicking File > Close.", "Closing a spreadsheet", 0, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays help message for searching for a value.
        /// </summary>
        private void ShowSearchForValueHelp()
        {
            MessageBox.Show("You can search for a specific number or text to see whether it is contained in any cells (you cannot search for text resulting from formula errors). Click on Search>Search for value. When prompted, enter the desired search value into the text box and press \"OK\". A window will pop up displaying the number of cells that contain the value as well as those cells' names", "Searching for a value", 0, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Displays help message for searching for a value.
        /// </summary>
        private void ShowSearchForContentHelp()
        {
            MessageBox.Show("You can search for a specific number or text or formula to see whether it is contained in any cells. Click on Search>Search for content. When prompted, enter the desired search content into the text box and press \"OK\". A window will pop up displaying the number of cells that contain the content as well as those cells' names", "Searching for content", 0, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Updates the values displayed in the spreadsheetPanel.
        /// 
        /// Displays an exception if any exception occcurs during the update.
        /// </summary>
        /// <param name="enumerable"></param>
        private void ResetCells(IEnumerable<string> enumerable)
        {
            try
            {
                foreach (string s in enumerable)
                {
                    CellToCoords(s, out int c, out int r);

                    if (sheet.GetCellValue(s) is FormulaError)
                    {
                        spreadsheetPanel.SetValue(c, r, ((FormulaError)sheet.GetCellValue(s)).Reason);
                    }
                    else if (sheet.GetCellValue(s) is double)
                    {
                        spreadsheetPanel.SetValue(c, r, sheet.GetCellValue(s).ToString());
                    }
                    else if (sheet.GetCellValue(s) is string)
                    {
                        spreadsheetPanel.SetValue(c, r, (string)sheet.GetCellValue(s));
                    }
                }
            }
            catch (CircularException)
            {
                MessageBox.Show("Circular exception! Make sure your formula does not contain any circular logic!", "Exception", 0, MessageBoxIcon.Error);
            }
            catch (FormulaFormatException e)
            {
                MessageBox.Show(e.Message, "Exception", 0, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Converts coordinates to the proper cell name.
        /// </summary>
        private string CoordsToCell(int col, int row)
        {
            return "" + (char)(col + 65) + (row + 1);
        }

        /// <summary>
        /// Converts the cell name to the proper coordinates.
        /// </summary>
        private void CellToCoords(string cell, out int col, out int row)
        {
            col = (int)cell[0] - 65;
            row = int.Parse(cell.Substring(1)) - 1;
        }

        /// <summary>
        /// Defines the validator for all spreadsheets within this application.
        /// </summary>
        private bool Validator(string s)
        {
            if (s.Length == 2)
                return Regex.IsMatch(s, "^[a-zA-Z][\\d]$");
            else if (s.Length == 3)
                return Regex.IsMatch(s, "^[a-zA-Z][1-9]\\d$");
            return false;
        }
    }
}
