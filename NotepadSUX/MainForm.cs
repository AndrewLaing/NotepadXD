using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NotepadXD
{
    public partial class MainForm : Form
    {
        private const String DEFAULT_FILENAME = "Untitled";
        private const String DEFAULT_APPNAME = "NotepadXD";
        private const float MIN_TEXTBOX_FONTSIZE = 8;
        private const int MAX_STACK_SIZE = 1000;
        private const int MIN_ZOOM_FACTOR = 10;  // 10%
        private const int MAX_ZOOM_FACTOR = 500; // 500%

        private AboutForm aboutform;
        private FindForm findForm;
        private ReplaceForm replaceForm;
        private String current_filename;
        private bool new_file_opened = false;
        private bool textbox1_text_has_changed = false;

        private float current_textbox_fontsize;

        private string stringToPrint;

        private Stack<Func<object>> undoStack = new Stack<Func<object>>();
        private Stack<Func<object>> redoStack = new Stack<Func<object>>();

        public MainForm()
        {
            InitializeComponent();
            InitialisePrinting();
            current_filename = DEFAULT_FILENAME;
            UpdateMainFormText();
            current_textbox_fontsize = textBox1.Font.Size;
            InitialiseSecondaryForms();
        }

        #region "Helper functions"

        private void InitialisePrinting()
        {
            pageSetupDialog1.Document = printDocument1;
            printDialog1.Document = printDocument1;
            printPreviewDialog1.Document = printDocument1;
        }

        private void InitialiseSecondaryForms()
        {
            aboutform = new AboutForm();
            findForm = new FindForm();
            findForm.findNextButton.Click += new System.EventHandler(this.findForm_findButton_Click);
            replaceForm = new ReplaceForm();
            replaceForm.findNextButton.Click += new System.EventHandler(this.replaceForm_findNextButton_Click);
            replaceForm.replaceButton.Click += new System.EventHandler(this.replaceForm_replaceButton_Click);
            replaceForm.replaceAllButton.Click += new System.EventHandler(this.replaceForm_replaceAllButton_Click);
        }

        private void ClearStacks()
        {
            undoStack.Clear();
            redoStack.Clear();
        }

        private bool ContinueWorkingOnCurrentDocument(object sender, EventArgs e)
        {
            if (textbox1_text_has_changed)
            {
                DialogResult save_result = ShowSaveChangesDialog();

                if (save_result == DialogResult.Yes)
                {
                    saveAsToolStripMenuItem_Click(sender, e);
                }
                return save_result == DialogResult.Cancel;
            }
            return false;
        }

        private void DoCannotFindSearchTermAction(string search_term)
        {
            String msg = "Cannot find \"" + search_term + "\"";
            String caption = DEFAULT_APPNAME;
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(msg, caption, buttons, MessageBoxIcon.Information);
        }

        private void DoNewFileOpened()
        {
            new_file_opened = true;
            textbox1_text_has_changed = false;
            UpdateMainFormText();
            ClearStacks();
        }

        private void RemoveItemsFromEndOfStack(Stack<Func<object>> toResize, int newSize)
        {
            Stack<Func<object>> resize = new Stack<Func<object>>();
            for (int i = 0; i < newSize; i++)
            {
                resize.Push(toResize.Pop());
            }
            toResize.Clear();
            for (int i = 0; i < newSize; i++)
            {
                toResize.Push(resize.Pop());
            }
        }

        private DialogResult ShowSaveChangesDialog()
        {
            String msg = "Do you want to save changes to " + System.IO.Path.GetFileName(current_filename);
            String caption = DEFAULT_APPNAME;
            MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;

            return MessageBox.Show(msg, caption, buttons);
        }

        private void UpdateCaretPositionStatusLabel()
        {
            int line = textBox1.GetLineFromCharIndex(textBox1.SelectionStart);
            int column = textBox1.SelectionStart - textBox1.GetFirstCharIndexFromLine(line);
            caretPositionStatusLabel.Text = "Ln " + (line + 1) + ", Col " + (column + 1);
        }

        private void UpdateMainFormText()
        {
            if(!textbox1_text_has_changed)
            {
                this.Text = System.IO.Path.GetFileName(current_filename);
            }
            else
            {
                this.Text = "*" + System.IO.Path.GetFileName(current_filename);
            }
            this.Text += " - " + DEFAULT_APPNAME;
        }

        #endregion


        #region "MainForm Event Handlers"

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ContinueWorkingOnCurrentDocument(sender, e))
            {
                e.Cancel = true;
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            UpdateCaretPositionStatusLabel();
        }

        #endregion


        #region "MenuStrip Event Handlers"

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ContinueWorkingOnCurrentDocument(sender, e) == false)
            {
                float current_zoom = textBox1.ZoomFactor;  // richtextbox zoom resets on cleared!!

                current_filename = DEFAULT_FILENAME;
                textBox1.Text = "";
                DoNewFileOpened();

                textBox1.ZoomFactor = current_zoom;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ContinueWorkingOnCurrentDocument(sender, e))
            {
                return;
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                current_filename = openFileDialog1.FileName;
                textBox1.Text = System.IO.File.ReadAllText(current_filename);
                DoNewFileOpened();
                undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(current_filename == DEFAULT_FILENAME)
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                System.IO.File.WriteAllText(current_filename, textBox1.Text);
                textbox1_text_has_changed = false;
                UpdateMainFormText();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = current_filename;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                current_filename = saveFileDialog1.FileName;
                System.IO.File.WriteAllText(current_filename, textBox1.Text);
                textbox1_text_has_changed = false;
                UpdateMainFormText();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ContinueWorkingOnCurrentDocument(sender, e) == false)
            {
                textbox1_text_has_changed = false;
                this.Close();
                Application.Exit();
            }
        }

        private void editToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            bool textSelected = textBox1.SelectionLength > 0;
            cutToolStripMenuItem.Enabled = textSelected;
            copyToolStripMenuItem.Enabled = textSelected;
            deleteToolStripMenuItem.Enabled = textSelected;
            googleSearchToolStripMenuItem.Enabled = textSelected;
            undoToolStripMenuItem.Enabled = undoStack.Count > 0;
            redoToolStripMenuItem.Enabled = redoStack.Count > 0;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                float current_zoom = textBox1.ZoomFactor;

                if(undoStack.Count >= MAX_STACK_SIZE)
                {
                    RemoveItemsFromEndOfStack(undoStack, MAX_STACK_SIZE / 2);
                }
                undoToolStripMenuItem.Enabled = false;
                redoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
                undoStack.Pop()();
                undoToolStripMenuItem.Enabled = true;

                textBox1.ZoomFactor = current_zoom;
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                float current_zoom = textBox1.ZoomFactor;

                redoToolStripMenuItem.Enabled = false;
                undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
                redoStack.Pop()();
                redoToolStripMenuItem.Enabled = true;

                textBox1.ZoomFactor = current_zoom;
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(textBox1.SelectedText != "")
            {
                undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
                textBox1.Cut();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.SelectedText != "")
            {
                textBox1.Copy();
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
            textBox1.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int start = textBox1.SelectionStart;
            int length = textBox1.SelectionLength;
            if(length < 1)
            {
                if(textBox1.Text.Length > start)
                {
                    length = 1;   // Implement delete ahead
                }
                
            }
            undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
            textBox1.Text = textBox1.Text.Remove(start, length);
            textBox1.SelectionStart = start;
            textBox1.SelectionLength = 0;
        }

        private void googleSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.SelectionLength > 0)
            {
                string target = "http://google.com/search?q=";
                target += textBox1.SelectedText;
                System.Diagnostics.Process.Start(target);
            }
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.SelectionLength > 0)
            { 
                findForm.set_textBox1_Text(textBox1.SelectedText);
            }
            findForm.Show();
        }

        private void findNextMatchAndHighlight(string search_term, bool wrap_around, bool match_case)
        {
            int start_pos = textBox1.SelectionStart + textBox1.SelectionLength;
            int idx;

            if (match_case)
            {
                idx = textBox1.Text.IndexOf(search_term, start_pos, StringComparison.Ordinal);
            }
            else
            {
                idx = textBox1.Text.IndexOf(search_term, start_pos, StringComparison.OrdinalIgnoreCase);
            }

            if (idx < 0 && wrap_around)
            {
                if (match_case)
                {
                    idx = textBox1.Text.IndexOf(search_term, 0, StringComparison.Ordinal);
                }
                else
                {
                    idx = textBox1.Text.IndexOf(search_term, 0, StringComparison.OrdinalIgnoreCase);
                }
            }

            if (idx < 0)
            {
                DoCannotFindSearchTermAction(search_term);
            }
            else
            {
                textBox1.SelectionStart = idx;
                textBox1.SelectionLength = search_term.Length;
                textBox1.ScrollToCaret();
            }
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string search_term = findForm.get_textBox1_Text();

            if(search_term.Length < 1)
            {
                findToolStripMenuItem_Click(sender, e);
            }
            else
            {
                bool wrap_around = findForm.get_wrapAround_checked();
                bool match_case = findForm.get_matchCase_checked();
                findNextMatchAndHighlight(search_term, wrap_around, match_case);
            }
        }

        private void findPreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string search_term = findForm.get_textBox1_Text();

            if (search_term.Length < 1)
            {
                findToolStripMenuItem_Click(sender, e);
            }
            else
            {
                int idx;
                bool wrap_around = findForm.get_wrapAround_checked();
                bool match_case = findForm.get_matchCase_checked();
                int start_pos = textBox1.SelectionStart;

                if (match_case)
                {
                    idx = textBox1.Text.LastIndexOf(search_term, start_pos, StringComparison.Ordinal);
                }
                else
                {
                    idx = textBox1.Text.LastIndexOf(search_term, start_pos, StringComparison.OrdinalIgnoreCase);
                }

                if (idx < 0 && wrap_around)
                {
                    if (match_case)
                    {
                        idx = textBox1.Text.LastIndexOf(search_term, textBox1.Text.Length, StringComparison.Ordinal);
                    }
                    else
                    {
                        idx = textBox1.Text.LastIndexOf(search_term, textBox1.Text.Length, StringComparison.OrdinalIgnoreCase);
                    }
                }

                if (idx < 0)
                {
                    DoCannotFindSearchTermAction(search_term);
                }
                else
                {
                    textBox1.SelectionStart = idx;
                    textBox1.SelectionLength = search_term.Length;
                    textBox1.ScrollToCaret();
                }
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textBox1.SelectionLength > 0)
            {
                replaceForm.set_findTextBox_Text(textBox1.SelectedText);
            }
            replaceForm.Show();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void timeDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString("H:mm MM/dd/yy");
            undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
            textBox1.SelectedText = date;
        }

        private void wordWrapToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            float current_zoom = textBox1.ZoomFactor;

            textBox1.WordWrap = wordWrapToolStripMenuItem.Checked;
            UpdateCaretPositionStatusLabel();

            textBox1.ZoomFactor = current_zoom;
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = textBox1.Font;

            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Font = fontDialog1.Font;
                current_textbox_fontsize = textBox1.Font.Size;
            }
        }

        private void UpdateZoomStatusLabel()
        {
            float factor = textBox1.ZoomFactor * 100;
            zoomStatusLabel.Text = factor + "%";
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int temp = (int)(textBox1.ZoomFactor * 100 + 10);
            if (temp > MAX_ZOOM_FACTOR)
            {
                return;
            }
            textBox1.ZoomFactor = (float)Math.Round(temp/100f,1);  // correct floating point error
            UpdateZoomStatusLabel();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int temp = (int)(textBox1.ZoomFactor * 100 - 10);
            if(temp < MIN_ZOOM_FACTOR)
            {
                return;
            }
            textBox1.ZoomFactor = (float)Math.Round(temp / 100f, 1);
            UpdateZoomStatusLabel();
        }

        private void restoreDefaultZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.ZoomFactor = 1f;
            UpdateZoomStatusLabel();
        }

        private void viewStatusBarToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (viewStatusBarToolStripMenuItem.Checked)
            {
                statusStrip1.Show();
            }
            else
            {
                statusStrip1.Hide();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutform.Show();
        }

        #endregion


        #region "textBox1 Event Handlers"

        private void textBox1_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            UpdateZoomStatusLabel();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateCaretPositionStatusLabel();

            if (new_file_opened)   //Dont show '*' in title bar when new/existing file opened
            {
                new_file_opened = false;
                return;
            }

            if(!textbox1_text_has_changed)
            {
                textbox1_text_has_changed = true;
                UpdateMainFormText();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            redoStack.Clear();
            undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            UpdateCaretPositionStatusLabel();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateCaretPositionStatusLabel();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateCaretPositionStatusLabel();
        }

        #endregion


        #region "contextMenu Event Handlers"

        private void contextMenuStrip1_Opened(object sender, EventArgs e)
        {
            bool textSelected = textBox1.SelectionLength > 0;
            cutToolStripMenuItem1.Enabled = textSelected;
            copyToolStripMenuItem1.Enabled = textSelected;
            deleteToolStripMenuItem1.Enabled = textSelected;
            googleSearchToolStripMenuItem1.Enabled = textSelected;
            undoToolStripMenuItem1.Enabled = undoStack.Count > 0;
            redoToolStripMenuItem1.Enabled = redoStack.Count > 0;
        }

        #endregion


        #region "findForm Event Handlers"

        protected void findForm_findButton_Click(object sender, EventArgs e)
        {
            if(findForm.get_searchDownwards())
            {
                findNextToolStripMenuItem_Click(sender, e);
            }
            else
            {
                findPreviousToolStripMenuItem_Click(sender, e);
            }
        }

        #endregion


        #region "replaceForm Event Handlers"

        protected void replaceForm_findNextButton_Click(object sender, EventArgs e)
        {
            string search_term = replaceForm.get_findTextBox_Text();
            bool wrap_around = replaceForm.get_wrapAround_checked();
            bool match_case = replaceForm.get_matchCase_checked();
            findNextMatchAndHighlight(search_term, wrap_around, match_case);
        }

        protected void replaceForm_replaceButton_Click(object sender, EventArgs e)
        {
            if(textBox1.SelectedText == replaceForm.get_findTextBox_Text())
            {
                undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
                textBox1.SelectedText = replaceForm.get_replaceTextBox_Text();
            }
            replaceForm_findNextButton_Click(sender, e);
        }

        protected void replaceForm_replaceAllButton_Click(object sender, EventArgs e)
        {
            string search_term = replaceForm.get_findTextBox_Text();
            string replace_term = replaceForm.get_replaceTextBox_Text();

            if (replaceForm.get_matchCase_checked())
            {
                if(textBox1.Text.IndexOf(search_term, 0, StringComparison.Ordinal) >= 0)
                {
                    undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
                    textBox1.Text = textBox1.Text.Replace(search_term, replace_term);
                    textBox1.SelectionStart = 0;
                    textBox1.SelectionLength = 0;
                }
            }
            else
            {
                if(textBox1.Text.IndexOf(search_term, 0, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart, textBox1.SelectionLength));
                    textBox1.Text = Regex.Replace(textBox1.Text, search_term, replace_term,
                                                  RegexOptions.IgnoreCase);
                    textBox1.SelectionStart = 0;
                    textBox1.SelectionLength = 0;
                }
            }
        }

        #endregion


        #region "Printing Event Handlers"

        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            stringToPrint = textBox1.Text;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int charactersOnPage = 0;
            int linesPerPage = 0;

            e.Graphics.MeasureString(stringToPrint, textBox1.Font,
                e.MarginBounds.Size, StringFormat.GenericTypographic,
                out charactersOnPage, out linesPerPage);

            // Draws the string within the bounds of the page
            e.Graphics.DrawString(stringToPrint, textBox1.Font, Brushes.Black,
                e.MarginBounds, StringFormat.GenericTypographic);

            stringToPrint = stringToPrint.Substring(charactersOnPage);
            e.HasMorePages = (stringToPrint.Length > 0);
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stringToPrint = textBox1.Text;
            pageSetupDialog1.PageSettings = new System.Drawing.Printing.PageSettings();
            pageSetupDialog1.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
            pageSetupDialog1.ShowDialog();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stringToPrint = textBox1.Text;
            printPreviewDialog1.ShowDialog();
        }

        #endregion
    }

    public static class Extensions
    {
        /// <summary>
        /// Used to store the text and cursor position from a textBox in the undo/redo stacks
        /// </summary>
        public static Func<RichTextBox> Text(this RichTextBox textBox, string text, int selectionStart, int selectionLength)
        {
            return () =>
            {
                textBox.Text = text;
                textBox.SelectionStart = selectionStart;
                textBox.SelectionLength = selectionLength;
                return textBox;
            };
        }
    }
}
