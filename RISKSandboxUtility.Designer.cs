using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RISKSandboxUtility
{
    partial class RISKSandboxUtility
    {

        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            territoriesPanel = new Panel();
            SuspendLayout();
            // 
            // territoriesPanel
            // 
            territoriesPanel.BackColor = Color.White;
            territoriesPanel.BorderStyle = BorderStyle.FixedSingle;
            territoriesPanel.Location = new Point(12, 238);
            territoriesPanel.Name = "territoriesPanel";
            territoriesPanel.Size = new Size(313, 361);
            territoriesPanel.TabIndex = 3;
            // 
            // RISKSandboxUtility
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1018, 621);
            Controls.Add(territoriesPanel);
            Name = "RISKSandboxUtility";
            Text = "RISK Sandbox Utitlity";
            ResumeLayout(false);
        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel territoriesPanel;

        private List<TextBox> territoryTextBoxes;
        private List<List<Button>> territoryButtons;
        private Button button1;
    }
}