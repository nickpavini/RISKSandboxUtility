﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RISKSandboxUtility
{
    partial class RISKHack
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
            loadCsvButton = new Button();
            playersPanel = new Panel();
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
            // loadCsvButton
            // 
            loadCsvButton.Location = new Point(15, 205);
            loadCsvButton.Name = "loadCsvButton";
            loadCsvButton.Size = new Size(75, 23);
            loadCsvButton.TabIndex = 4;
            loadCsvButton.Text = "Load CSV";
            loadCsvButton.UseVisualStyleBackColor = true;
            loadCsvButton.Click += loadCsvButton_Click;
            // 
            // playersPanel
            // 
            playersPanel.BackColor = Color.White;
            playersPanel.BorderStyle = BorderStyle.FixedSingle;
            playersPanel.Location = new Point(15, 12);
            playersPanel.Name = "playersPanel";
            playersPanel.Size = new Size(313, 138);
            playersPanel.TabIndex = 4;
            // 
            // RISKHack
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(340, 621);
            Controls.Add(playersPanel);
            Controls.Add(loadCsvButton);
            Controls.Add(territoriesPanel);
            Name = "RISKHack";
            Text = "RISK Hack ";
            ResumeLayout(false);
        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel territoriesPanel;

        private List<TextBox> territoryTextBoxes;
        private List<List<Button>> territoryButtons;
        private Button button1;
        private Button loadCsvButton;
        private Panel playersPanel;
    }
}