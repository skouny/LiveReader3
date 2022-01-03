﻿namespace LiveReader3
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabBrowser = new System.Windows.Forms.TabPage();
            this.spliterBrowser = new System.Windows.Forms.SplitContainer();
            this.splitFlashScore = new System.Windows.Forms.SplitContainer();
            this.listBrowserLog = new System.Windows.Forms.ListBox();
            this.tabUpdatesRealtime = new System.Windows.Forms.TabPage();
            this.listRealtimeLog = new System.Windows.Forms.ListBox();
            this.tabUpdatesNormal = new System.Windows.Forms.TabPage();
            this.listNormalLog = new System.Windows.Forms.ListBox();
            this.tabUpdatesPopup = new System.Windows.Forms.TabPage();
            this.listPopupLog = new System.Windows.Forms.ListBox();
            this.workerRealtime = new System.ComponentModel.BackgroundWorker();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.workerNormal = new System.ComponentModel.BackgroundWorker();
            this.workerPopup = new System.ComponentModel.BackgroundWorker();
            this.labelPopups = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabBrowser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spliterBrowser)).BeginInit();
            this.spliterBrowser.Panel1.SuspendLayout();
            this.spliterBrowser.Panel2.SuspendLayout();
            this.spliterBrowser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitFlashScore)).BeginInit();
            this.splitFlashScore.SuspendLayout();
            this.tabUpdatesRealtime.SuspendLayout();
            this.tabUpdatesNormal.SuspendLayout();
            this.tabUpdatesPopup.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.BottomToolStripPanel
            // 
            this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.tabControl);
            this.toolStripContainer.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1384, 739);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.LeftToolStripPanelVisible = false;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.RightToolStripPanelVisible = false;
            this.toolStripContainer.Size = new System.Drawing.Size(1384, 761);
            this.toolStripContainer.TabIndex = 0;
            this.toolStripContainer.Text = "toolStripContainer1";
            this.toolStripContainer.TopToolStripPanelVisible = false;
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus,
            this.labelPopups});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1384, 22);
            this.statusStrip.TabIndex = 0;
            // 
            // labelStatus
            // 
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(42, 17);
            this.labelStatus.Text = "Status";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabBrowser);
            this.tabControl.Controls.Add(this.tabUpdatesRealtime);
            this.tabControl.Controls.Add(this.tabUpdatesNormal);
            this.tabControl.Controls.Add(this.tabUpdatesPopup);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1384, 739);
            this.tabControl.TabIndex = 0;
            // 
            // tabBrowser
            // 
            this.tabBrowser.Controls.Add(this.spliterBrowser);
            this.tabBrowser.Location = new System.Drawing.Point(4, 23);
            this.tabBrowser.Name = "tabBrowser";
            this.tabBrowser.Padding = new System.Windows.Forms.Padding(3);
            this.tabBrowser.Size = new System.Drawing.Size(1376, 712);
            this.tabBrowser.TabIndex = 0;
            this.tabBrowser.Text = "Browser";
            this.tabBrowser.UseVisualStyleBackColor = true;
            // 
            // spliterBrowser
            // 
            this.spliterBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spliterBrowser.Location = new System.Drawing.Point(3, 3);
            this.spliterBrowser.Name = "spliterBrowser";
            this.spliterBrowser.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spliterBrowser.Panel1
            // 
            this.spliterBrowser.Panel1.Controls.Add(this.splitFlashScore);
            // 
            // spliterBrowser.Panel2
            // 
            this.spliterBrowser.Panel2.Controls.Add(this.listBrowserLog);
            this.spliterBrowser.Size = new System.Drawing.Size(1370, 706);
            this.spliterBrowser.SplitterDistance = 555;
            this.spliterBrowser.TabIndex = 0;
            // 
            // splitFlashScore
            // 
            this.splitFlashScore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitFlashScore.IsSplitterFixed = true;
            this.splitFlashScore.Location = new System.Drawing.Point(0, 0);
            this.splitFlashScore.Name = "splitFlashScore";
            this.splitFlashScore.Panel1MinSize = 75;
            this.splitFlashScore.Size = new System.Drawing.Size(1370, 555);
            this.splitFlashScore.SplitterDistance = 821;
            this.splitFlashScore.TabIndex = 0;
            // 
            // listBrowserLog
            // 
            this.listBrowserLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBrowserLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.listBrowserLog.FormattingEnabled = true;
            this.listBrowserLog.IntegralHeight = false;
            this.listBrowserLog.ItemHeight = 14;
            this.listBrowserLog.Location = new System.Drawing.Point(0, 0);
            this.listBrowserLog.Name = "listBrowserLog";
            this.listBrowserLog.Size = new System.Drawing.Size(1370, 147);
            this.listBrowserLog.TabIndex = 0;
            // 
            // tabUpdatesRealtime
            // 
            this.tabUpdatesRealtime.Controls.Add(this.listRealtimeLog);
            this.tabUpdatesRealtime.Location = new System.Drawing.Point(4, 23);
            this.tabUpdatesRealtime.Name = "tabUpdatesRealtime";
            this.tabUpdatesRealtime.Padding = new System.Windows.Forms.Padding(3);
            this.tabUpdatesRealtime.Size = new System.Drawing.Size(1376, 712);
            this.tabUpdatesRealtime.TabIndex = 1;
            this.tabUpdatesRealtime.Text = "Realtime Updates";
            this.tabUpdatesRealtime.UseVisualStyleBackColor = true;
            // 
            // listRealtimeLog
            // 
            this.listRealtimeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listRealtimeLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.listRealtimeLog.FormattingEnabled = true;
            this.listRealtimeLog.IntegralHeight = false;
            this.listRealtimeLog.ItemHeight = 14;
            this.listRealtimeLog.Location = new System.Drawing.Point(3, 3);
            this.listRealtimeLog.Name = "listRealtimeLog";
            this.listRealtimeLog.Size = new System.Drawing.Size(1370, 706);
            this.listRealtimeLog.TabIndex = 0;
            // 
            // tabUpdatesNormal
            // 
            this.tabUpdatesNormal.Controls.Add(this.listNormalLog);
            this.tabUpdatesNormal.Location = new System.Drawing.Point(4, 23);
            this.tabUpdatesNormal.Name = "tabUpdatesNormal";
            this.tabUpdatesNormal.Padding = new System.Windows.Forms.Padding(3);
            this.tabUpdatesNormal.Size = new System.Drawing.Size(1376, 712);
            this.tabUpdatesNormal.TabIndex = 2;
            this.tabUpdatesNormal.Text = "Normal Updates";
            this.tabUpdatesNormal.UseVisualStyleBackColor = true;
            // 
            // listNormalLog
            // 
            this.listNormalLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listNormalLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.listNormalLog.FormattingEnabled = true;
            this.listNormalLog.IntegralHeight = false;
            this.listNormalLog.ItemHeight = 14;
            this.listNormalLog.Location = new System.Drawing.Point(3, 3);
            this.listNormalLog.Name = "listNormalLog";
            this.listNormalLog.Size = new System.Drawing.Size(1370, 706);
            this.listNormalLog.TabIndex = 0;
            // 
            // tabUpdatesPopup
            // 
            this.tabUpdatesPopup.Controls.Add(this.listPopupLog);
            this.tabUpdatesPopup.Location = new System.Drawing.Point(4, 23);
            this.tabUpdatesPopup.Name = "tabUpdatesPopup";
            this.tabUpdatesPopup.Size = new System.Drawing.Size(1376, 712);
            this.tabUpdatesPopup.TabIndex = 3;
            this.tabUpdatesPopup.Text = "Popup Updates";
            this.tabUpdatesPopup.UseVisualStyleBackColor = true;
            // 
            // listPopupLog
            // 
            this.listPopupLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listPopupLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.listPopupLog.FormattingEnabled = true;
            this.listPopupLog.IntegralHeight = false;
            this.listPopupLog.ItemHeight = 14;
            this.listPopupLog.Location = new System.Drawing.Point(0, 0);
            this.listPopupLog.Name = "listPopupLog";
            this.listPopupLog.Size = new System.Drawing.Size(1376, 712);
            this.listPopupLog.TabIndex = 0;
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "LiveReader3";
            this.notifyIcon.Visible = true;
            // 
            // labelPopups
            // 
            this.labelPopups.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.labelPopups.Margin = new System.Windows.Forms.Padding(16, 3, 0, 2);
            this.labelPopups.Name = "labelPopups";
            this.labelPopups.Size = new System.Drawing.Size(47, 17);
            this.labelPopups.Text = "Popups";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 761);
            this.Controls.Add(this.toolStripContainer);
            this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(1400, 800);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LiveReader3";
            this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabBrowser.ResumeLayout(false);
            this.spliterBrowser.Panel1.ResumeLayout(false);
            this.spliterBrowser.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spliterBrowser)).EndInit();
            this.spliterBrowser.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitFlashScore)).EndInit();
            this.splitFlashScore.ResumeLayout(false);
            this.tabUpdatesRealtime.ResumeLayout(false);
            this.tabUpdatesNormal.ResumeLayout(false);
            this.tabUpdatesPopup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabBrowser;
        private System.Windows.Forms.TabPage tabUpdatesRealtime;
        private System.Windows.Forms.SplitContainer spliterBrowser;
        private System.Windows.Forms.ListBox listBrowserLog;
        private System.ComponentModel.BackgroundWorker workerRealtime;
        private System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.SplitContainer splitFlashScore;
        private System.ComponentModel.BackgroundWorker workerNormal;
        private System.ComponentModel.BackgroundWorker workerPopup;
        private System.Windows.Forms.ListBox listRealtimeLog;
        private System.Windows.Forms.TabPage tabUpdatesNormal;
        private System.Windows.Forms.ListBox listNormalLog;
        private System.Windows.Forms.TabPage tabUpdatesPopup;
        private System.Windows.Forms.ListBox listPopupLog;
        private System.Windows.Forms.ToolStripStatusLabel labelPopups;
    }
}
