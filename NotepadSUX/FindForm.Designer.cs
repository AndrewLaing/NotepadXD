namespace NotepadXD
{
    partial class FindForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindForm));
            this.findNextButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.directionGroupBox = new System.Windows.Forms.GroupBox();
            this.downRadioButton = new System.Windows.Forms.RadioButton();
            this.upRadioButton = new System.Windows.Forms.RadioButton();
            this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.wrapCheckBox = new System.Windows.Forms.CheckBox();
            this.directionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // findNextButton
            // 
            this.findNextButton.Enabled = false;
            this.findNextButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.findNextButton.Location = new System.Drawing.Point(328, 16);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(91, 24);
            this.findNextButton.TabIndex = 1;
            this.findNextButton.Text = "Find Next";
            this.findNextButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(328, 48);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(91, 24);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Find what:";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.HideSelection = false;
            this.textBox1.Location = new System.Drawing.Point(88, 16);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(224, 22);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // directionGroupBox
            // 
            this.directionGroupBox.Controls.Add(this.downRadioButton);
            this.directionGroupBox.Controls.Add(this.upRadioButton);
            this.directionGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.directionGroupBox.Location = new System.Drawing.Point(176, 48);
            this.directionGroupBox.Name = "directionGroupBox";
            this.directionGroupBox.Size = new System.Drawing.Size(136, 40);
            this.directionGroupBox.TabIndex = 4;
            this.directionGroupBox.TabStop = false;
            this.directionGroupBox.Text = "Direction";
            // 
            // downRadioButton
            // 
            this.downRadioButton.AutoSize = true;
            this.downRadioButton.Checked = true;
            this.downRadioButton.Location = new System.Drawing.Point(70, 16);
            this.downRadioButton.Name = "downRadioButton";
            this.downRadioButton.Size = new System.Drawing.Size(53, 17);
            this.downRadioButton.TabIndex = 4;
            this.downRadioButton.TabStop = true;
            this.downRadioButton.Text = "Down";
            this.downRadioButton.UseVisualStyleBackColor = true;
            this.downRadioButton.CheckedChanged += new System.EventHandler(this.downRadioButton_CheckedChanged);
            // 
            // upRadioButton
            // 
            this.upRadioButton.AutoSize = true;
            this.upRadioButton.Location = new System.Drawing.Point(14, 16);
            this.upRadioButton.Name = "upRadioButton";
            this.upRadioButton.Size = new System.Drawing.Size(39, 17);
            this.upRadioButton.TabIndex = 3;
            this.upRadioButton.Text = "Up";
            this.upRadioButton.UseVisualStyleBackColor = true;
            this.upRadioButton.CheckedChanged += new System.EventHandler(this.upRadioButton_CheckedChanged);
            // 
            // matchCaseCheckBox
            // 
            this.matchCaseCheckBox.AutoSize = true;
            this.matchCaseCheckBox.Location = new System.Drawing.Point(8, 80);
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.Size = new System.Drawing.Size(82, 17);
            this.matchCaseCheckBox.TabIndex = 5;
            this.matchCaseCheckBox.Text = "Match case";
            this.matchCaseCheckBox.UseVisualStyleBackColor = true;
            // 
            // wrapCheckBox
            // 
            this.wrapCheckBox.AutoSize = true;
            this.wrapCheckBox.Location = new System.Drawing.Point(8, 104);
            this.wrapCheckBox.Name = "wrapCheckBox";
            this.wrapCheckBox.Size = new System.Drawing.Size(88, 17);
            this.wrapCheckBox.TabIndex = 6;
            this.wrapCheckBox.Text = "Wrap around";
            this.wrapCheckBox.UseVisualStyleBackColor = true;
            // 
            // FindForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(429, 129);
            this.Controls.Add(this.wrapCheckBox);
            this.Controls.Add(this.matchCaseCheckBox);
            this.Controls.Add(this.directionGroupBox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.findNextButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindForm";
            this.Text = "Find";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.FindForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindForm_FormClosing);
            this.directionGroupBox.ResumeLayout(false);
            this.directionGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox directionGroupBox;
        private System.Windows.Forms.CheckBox matchCaseCheckBox;
        private System.Windows.Forms.CheckBox wrapCheckBox;
        private System.Windows.Forms.RadioButton downRadioButton;
        private System.Windows.Forms.RadioButton upRadioButton;
        public System.Windows.Forms.Button findNextButton;
    }
}