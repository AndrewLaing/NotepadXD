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

        private AboutForm aboutform;
        private FindForm findForm;
        private ReplaceForm replaceForm;
        private String current_filename;
        private bool new_file_opened = false;
        private bool textbox1_text_has_changed = false;

        private float current_textbox_fontsize;

        private Stack<Func<object>> undoStack = new Stack<Func<object>>();
        private Stack<Func<object>> redoStack = new Stack<Func<object>>();

        public MainForm()
        {
            InitializeComponent();
            current_filename = DEFAULT_FILENAME;
            UpdateMainFormText();
            current_textbox_fontsize = textBox1.Font.Size;
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

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ContinueWorkingOnCurrentDocument(sender, e) == false)
            {
                current_filename = DEFAULT_FILENAME;
                textBox1.Text = "";
                DoNewFileOpened();
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
                undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart));
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

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                if(undoStack.Count >= MAX_STACK_SIZE)
                {
                    RemoveItemsFromEndOfStack(undoStack, MAX_STACK_SIZE / 2);
                }
                undoToolStripMenuItem.Enabled = false;
                redoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart));
                undoStack.Pop()();
                undoToolStripMenuItem.Enabled = true;
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                redoToolStripMenuItem.Enabled = false;
                undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart));
                redoStack.Pop()();
                redoToolStripMenuItem.Enabled = true;
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(textBox1.SelectedText != "")
            {
                undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart));
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
            undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart));
            textBox1.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int start = textBox1.SelectionStart;
            int length = textBox1.SelectionLength;
            textBox1.Text = textBox1.Text.Remove(start, length);
            textBox1.SelectionStart = start;
            textBox1.SelectionLength = 0;
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
            undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart));
            textBox1.Paste(date);
        }

        private void wordWrapToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.WordWrap = wordWrapToolStripMenuItem.Checked;
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

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float newFontSize = (textBox1.Font.Size + 1);
            textBox1.Font = new Font(textBox1.Font.FontFamily, newFontSize);
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float newFontSize = (textBox1.Font.Size - 1);
            if(newFontSize > MIN_TEXTBOX_FONTSIZE)
            {
                textBox1.Font = new Font(textBox1.Font.FontFamily, newFontSize);
            }
        }

        private void restoreDefaultZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Font = new Font(textBox1.Font.FontFamily, (current_textbox_fontsize));
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutform.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ContinueWorkingOnCurrentDocument(sender, e))
            {
                e.Cancel = true;
                return;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
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
            undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart));
        }

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
                undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart));
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
                    undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart));
                    textBox1.Text = textBox1.Text.Replace(search_term, replace_term);
                }
                else
                {
                    return;
                }
            }
            else
            {
                if(textBox1.Text.IndexOf(search_term, 0, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    undoStack.Push(textBox1.Text(textBox1.Text, textBox1.SelectionStart));
                    textBox1.Text = Regex.Replace(textBox1.Text, search_term, replace_term,
                                                  RegexOptions.IgnoreCase);
                }
                else
                {
                    return;
                }
            }

            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
        }
    }

    public static class Extensions
    {
        // Used to create an object for the undo/redo stacks, saving the current text
        // in the textbox and cursor position
        public static Func<TextBox> Text(this TextBox textBox, string text, int sel)
        {
            return () =>
            {
                textBox.Text = text;
                textBox.SelectionStart = sel;
                return textBox;
            };
        }
    }
}
