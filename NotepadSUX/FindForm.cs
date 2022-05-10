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
    public partial class FindForm : Form
    {
        private bool searchDownwards = true;

        public FindForm()
        {
            InitializeComponent();
        }

        public string get_textBox1_Text()
        {
            return textBox1.Text;
        }

        public bool get_matchCase_enabled()
        {
            return matchCaseCheckBox.Enabled;
        }

        public bool get_wrapAround_enabled()
        {
            return wrapCheckBox.Enabled;
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

        private void FindForm_Deactivate(object sender, EventArgs e)
        {
            Hide();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Length < 1)
            {
                findNextButton.Enabled = false;
            }
            else
            {
                findNextButton.Enabled = true;
            }
        }

        private void upRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            searchDownwards = downRadioButton.Checked == true;
        }

        private void downRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            searchDownwards = downRadioButton.Checked == true;
        }
    }
}
