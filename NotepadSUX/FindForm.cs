using System;
using System.Windows.Forms;

namespace NotepadXD
{
    public partial class FindForm : Form
    {
        private bool searchDownwards = true;

        public FindForm()
        {
            InitializeComponent();
        }

        public void set_textBox1_Text(string selected_text)
        {
            textBox1.Text = selected_text;
        }

        public string get_textBox1_Text()
        {
            return textBox1.Text;
        }

        public bool get_matchCase_checked()
        {
            return matchCaseCheckBox.Checked;
        }

        public bool get_wrapAround_checked()
        {
            return wrapCheckBox.Checked;
        }

        public bool get_searchDownwards()
        {
            return searchDownwards;
        }

        private void FindForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bool has_text = textBox1.Text.Length > 0;
            findNextButton.Enabled = has_text;
        }

        private void upRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            searchDownwards = downRadioButton.Checked;
        }

        private void downRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            searchDownwards = downRadioButton.Checked;
        }
    }
}
