using LogUtils.UI;

namespace ServerGUI
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tb_input = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.logBox = new LogUtils.UI.LogBox();
            this.tb_state = new System.Windows.Forms.RichTextBox();
            this.StateTimer = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tb_commands = new System.Windows.Forms.RichTextBox();
            this.tb_output = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_input
            // 
            this.tb_input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_input.BackColor = System.Drawing.SystemColors.InfoText;
            this.tb_input.ForeColor = System.Drawing.Color.White;
            this.tb_input.Location = new System.Drawing.Point(3, 3);
            this.tb_input.Name = "tb_input";
            this.tb_input.Size = new System.Drawing.Size(921, 22);
            this.tb_input.TabIndex = 1;
            this.tb_input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Input_KeyDown);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.logBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Controls.Add(this.tb_input);
            this.splitContainer1.Size = new System.Drawing.Size(927, 811);
            this.splitContainer1.SplitterDistance = 437;
            this.splitContainer1.TabIndex = 2;
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logBox.ForeColor = System.Drawing.SystemColors.Info;
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.MinimalSkipTime = 100;
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(927, 437);
            this.logBox.TabIndex = 3;
            this.logBox.TabStop = false;
            this.logBox.Text = "";
            this.logBox.WordWrap = false;
            // 
            // tb_state
            // 
            this.tb_state.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_state.BackColor = System.Drawing.SystemColors.MenuText;
            this.tb_state.ForeColor = System.Drawing.SystemColors.Info;
            this.tb_state.Location = new System.Drawing.Point(945, 12);
            this.tb_state.Name = "tb_state";
            this.tb_state.Size = new System.Drawing.Size(454, 811);
            this.tb_state.TabIndex = 4;
            this.tb_state.TabStop = false;
            this.tb_state.Text = "";
            // 
            // StateTimer
            // 
            this.StateTimer.Enabled = true;
            this.StateTimer.Interval = 1000;
            this.StateTimer.Tick += new System.EventHandler(this.StateTimer_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 34);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(924, 336);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tb_output);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(916, 307);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "prompt";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tb_commands);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(916, 307);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "commands";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tb_commands
            // 
            this.tb_commands.BackColor = System.Drawing.SystemColors.MenuText;
            this.tb_commands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_commands.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_commands.ForeColor = System.Drawing.SystemColors.Info;
            this.tb_commands.Location = new System.Drawing.Point(3, 3);
            this.tb_commands.Name = "tb_commands";
            this.tb_commands.Size = new System.Drawing.Size(910, 301);
            this.tb_commands.TabIndex = 3;
            this.tb_commands.TabStop = false;
            this.tb_commands.Text = "";
            // 
            // tb_output
            // 
            this.tb_output.BackColor = System.Drawing.SystemColors.MenuText;
            this.tb_output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_output.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_output.ForeColor = System.Drawing.SystemColors.Info;
            this.tb_output.Location = new System.Drawing.Point(3, 3);
            this.tb_output.Name = "tb_output";
            this.tb_output.Size = new System.Drawing.Size(910, 301);
            this.tb_output.TabIndex = 3;
            this.tb_output.TabStop = false;
            this.tb_output.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1411, 835);
            this.Controls.Add(this.tb_state);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox tb_input;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private LogBox logBox;
        private System.Windows.Forms.RichTextBox tb_state;
        private System.Windows.Forms.Timer StateTimer;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox tb_output;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox tb_commands;
    }
}

