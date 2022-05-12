using System;
using System.Windows.Forms;

namespace NotepadXD
{
    public partial class ReplaceForm : Form
    {
        public ReplaceForm()
        {
            InitializeComponent();
        }

        public void set_findTextBox_Text(string selected_text)
        {
            findTextBox.Text = selected_text;
        }

        public string get_findTextBox_Text()
        {
            return findTextBox.Text;
        }

        public string get_replaceTextBox_Text()
        {
            return replaceTextBox.Text;
        }

        public bool get_matchCase_checked()
        {
            return matchCaseCheckBox.Checked;
        }

        public bool get_wrapAround_checked()
        {
            return wrapCheckBox.Checked;
        }

        private void ReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void findTextBox_TextChanged(object sender, EventArgs e)
        {
            bool has_text = findTextBox.Text.Length > 0;

            findNextButton.Enabled = has_text;
            replaceButton.Enabled = has_text;
            replaceAllButton.Enabled = has_text;
        }
    }
}
