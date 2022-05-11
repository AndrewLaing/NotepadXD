using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (findTextBox.Text.Length < 1)
            {
                findNextButton.Enabled = false;
                replaceButton.Enabled = false;
                replaceAllButton.Enabled = false;
            }
            else
            {
                findNextButton.Enabled = true;
                replaceButton.Enabled = true;
                replaceAllButton.Enabled = true;
            }
        }
    }
}
