using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace LiveReader3
{
    public partial class FormMain : Form
    {
        const string URL = "https://www.flashscore.com/";
        private ChromiumWebBrowser browser;
        private ChromiumWebBrowser popup;
        private bool allowExit = false;
        public FormMain()
        {
            Cache.Restore();
            this.InitializeComponent();
            this.InitializeForm();
            this.InitializeMenu();
            this.InitializeNotify();
            this.InitializeBrowser();
            this.InitializePopup();
        }
        private void InitializeForm()
        {
            this.Icon = Properties.Resources.icon;
            this.Load += (sender, e) =>
            {
                this.UpdatePopupStatus();
            };
            this.Shown += (sender, e) =>
            {
                //this.Hide();
            };
            this.VisibleChanged += (sender, e) =>
            {
                var query = this.ContextMenuStrip.Items.Find("Show", false);
                if (query.Count() > 0)
                {
                    ((ToolStripMenuItem)query[0]).Checked = this.Visible;
                }
            };
            this.FormClosing += (sender, e) =>
            {
                if (!this.allowExit)
                {
                    this.Hide();
                    e.Cancel = true;
                }
            };

        }
        private void InitializeMenu()
        {
            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Items.Add("Show", null, (sender, e) =>
            {
                this.Visible = !this.Visible;
            });
            this.ContextMenuStrip.Items.Add("Path", null, (sender, e) =>
            {
                System.Diagnostics.Process.Start(Application.LocalUserAppDataPath);
            });
#if DEBUG
            this.ContextMenuStrip.Items.Add("Dev Tools", null, (sender, e) =>
            {
                this.browser.ShowDevTools();
            });
#endif
            this.ContextMenuStrip.Items.Add("Exit", null, (sender, e) =>
            {
                this.allowExit = true;
                Application.Exit();
            });
        }
        private void InitializeNotify()
        {
            this.notifyIcon.Icon = this.Icon;
            this.notifyIcon.Text = this.Text;
            this.notifyIcon.ContextMenuStrip = this.ContextMenuStrip;
            this.notifyIcon.MouseClick += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Visible = !this.Visible;
                }
            };
        }
        private void InitializeBrowser()
        {
            this.browser = new ChromiumWebBrowser(URL)
            {
                Dock = DockStyle.Fill
            };
            this.splitFlashScore.Panel1.Controls.Add(this.browser);
            this.browser.JavascriptMessageReceived += (sender, e) =>
            {
                this.Invoke(new Action(() =>
                {

                }));
            };
            this.browser.LoadingStateChanged += (sender, e) =>
            {
                this.Invoke(new Action(() =>
                {
                    if (!this.workerRealtime.IsBusy && !e.IsLoading)
                    {
                        this.workerRealtime.RunWorkerAsync();
                    }
                    if (!this.workerNormal.IsBusy && !e.IsLoading)
                    {
                        this.workerNormal.RunWorkerAsync();
                    }
                }));
            };
            this.browser.LoadError += (sender, e) =>
            {
                this.Invoke(new Action(() =>
                {

                }));
            };
            this.workerRealtime.DoWork += (sender, e) =>
            {
                // Delay start
                this.WriteStatus($"Starting...");
                System.Threading.Thread.Sleep(5 * 1000);
                // Loop
                while (true)
                {
                    var time = DateTime.Now;
                    var script = Properties.Resources.FlashScore_v2;
                    var task = this.browser.EvaluateScriptAsync(script);
                    task.Wait();
                    var response = task.Result;
                    if (response.Success)
                    {
                        if (response.Result != null)
                        {
                            var result = (string)response.Result;
                            var data = RawMatch.ParseData(result);
                            this.WriteStatus($"Found {data.Values.Count} matches [{((DateTime.Now - time).TotalMilliseconds):#,##0}ms]");
                            // Update
                            var chunks = Cache.GetModifiedScoresAndQueue(data);
                            foreach (var chunk in chunks)
                            {
                                var t = DateTime.Now;
                                RawMatch.Post(this.WriteRealtimeLog, chunk);
                                this.WriteLog($"Posted {chunk.Length} priority update(s) [{((DateTime.Now - t).TotalMilliseconds):#,##0}ms]");
                            }
                        }
                    }
                    else
                    {
                        this.WriteStatus($"Read failure => {response.Message}", true);
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            };
            this.workerNormal.DoWork += (sender, e) =>
            {
                while (true)
                {
                    var chunks = Cache.GetQueue();
                    foreach (var chunk in chunks)
                    {
                        var t = DateTime.Now;
                        RawMatch.Post(this.WriteNormalLog, chunk);
                        this.WriteLog($"Posted {chunk.Length} info update(s) [{((DateTime.Now - t).TotalMilliseconds):#,##0}ms]");
                    }
                    System.Threading.Thread.Sleep(30 * 1000);
                }
            };
        }
        private void InitializePopup()
        {
            this.popup = new ChromiumWebBrowser(URL)
            {
                Dock = DockStyle.Fill
            };
            this.splitFlashScore.Panel2.Controls.Add(this.popup);
            this.popup.JavascriptMessageReceived += (sender, e) =>
            {
                this.Invoke(new Action(() =>
                {

                }));
            };
            this.popup.LoadingStateChanged += (sender, e) =>
            {
                this.Invoke(new Action(() =>
                {
                    if (!this.workerPopup.IsBusy && !e.IsLoading)
                    {
                        this.workerPopup.RunWorkerAsync();
                    }
                }));
            };
            this.popup.LoadError += (sender, e) =>
            {
                this.Invoke(new Action(() =>
                {

                }));
            };
            this.workerPopup.DoWork += (sender, e) =>
            {
                var wait = 2 * 1000; // Miliseconds
                var refresh = 60; // Minutes
                while (true)
                {
                    // Realtime
                    var queryRealtime = Cache.Popups.Where(x => x.Value.Priority == Cache.Priority.Realtime).OrderBy(x => x.Value.Updated).ToArray();
                    if (queryRealtime.Length > 0)
                    {
                        foreach (var item in queryRealtime)
                        {
                            var info = this.ReadPopup(item.Key, wait);
                            if (info != null)
                            {
                                this.WritePopupLog($"Popup read success [Realtime] => Key: {item.Key}, Length: {info.JsonData.Length}, StartTime: {info.StartTime}");
                                Cache.Popups[item.Key] = info;
                                Cache.Save();
                                this.UpdatePopupStatus();
                            }
                        }
                    }
                    else
                    {
                        // Normal
                        var queryNormal = Cache.Popups.Where(x => x.Value.Priority == Cache.Priority.Normal && x.Value.Active && (DateTime.Now - x.Value.Updated).TotalMinutes > refresh).OrderBy(x => x.Value.Updated).ToArray();
                        if (queryNormal.Length > 0)
                        {
                            var item = queryNormal[0];
                            var info = this.ReadPopup(item.Key, wait);
                            if (info != null)
                            {
                                this.WritePopupLog($"Popup read success [Normal] => Key: {item.Key}, Length: {info.JsonData.Length}, StartTime: {info.StartTime}");
                                Cache.Popups[item.Key] = info;
                                Cache.Save();
                            }
                        }
                    }
                    this.UpdatePopupStatus();
                    // Break
                    System.Threading.Thread.Sleep(wait);
                }
            };
        }
        private Cache.PopupInfo ReadPopup(string eventId, int wait, int retries = 3, int minJsonLength = 10)
        {
            var url = $"https://www.flashscore.com/match/{eventId}/";
            // Navigate
            this.popup.Load(url);
            // Wait to load
            while (true)
            {
                System.Threading.Thread.Sleep(wait);
                if (!this.popup.IsLoading) break;
            }
            // Try to read
            for (int i = 0; i < retries; i++)
            {
                var script = Properties.Resources.FlashScorePopup;
                var task = this.popup.EvaluateScriptAsync(script);
                task.Wait();
                var response = task.Result;
                if (response.Success)
                {
                    var json = (string)response.Result ?? string.Empty;
                    if (json.Length >= minJsonLength)
                    {
                        var info = Cache.PopupInfo.Parse(json);
                        return info;
                    }
                }
                else
                {
                    this.WritePopupLog($"Popup read failure [{eventId}] => {response.Message}");
                }
                System.Threading.Thread.Sleep(2 * wait);
            }
            // Read failure
            return null;
        }
        private void WriteLog(string message, int limit = 9999)
        {
            this.WriteLog(this.listBrowserLog, message, limit);
        }
        private void WriteRealtimeLog(string[] messages, int limit = 9999)
        {
            this.WriteLog(this.listRealtimeLog, messages, limit);
        }
        private void WriteNormalLog(string[] messages, int limit = 9999)
        {
            this.WriteLog(this.listNormalLog, messages, limit);
        }
        private void WritePopupLog(string message, int limit = 9999)
        {
            this.WriteLog(this.listPopupLog, message, limit);
        }
        private void WriteLog(ListBox listBox, string message, int limit = 9999)
        {
            this.WriteLog(listBox, new string[] { message }, limit);
        }
        private void WriteLog(ListBox listBox, string[] messages, int limit = 9999)
        {
            this.Invoke(new Action(() =>
            {
                var now = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                foreach (var message in messages)
                {
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        listBox.Items.Insert(0, $"{now}: {message}");
                    }
                }
                var count = listBox.Items.Count;
                while (count > limit)
                {
                    listBox.Items.RemoveAt(count - 1);
                }
            }));
        }
        private void WriteStatus(string message, bool error = false)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                this.Invoke(new Action(() =>
                {
                    this.labelStatus.Text = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}";
                    this.labelStatus.ForeColor = (error) ? Color.Red : Color.Green;
                }));
            }
        }
        private void UpdatePopupStatus()
        {
            var realtime = Cache.Popups.Where(x => x.Value.Priority == Cache.Priority.Realtime).Count();
            var normal = Cache.Popups.Where(x => x.Value.Priority == Cache.Priority.Normal).Count();
            this.Invoke(new Action(() =>
            {
                this.labelPopups.Text = $"Popup Cache: Realtime = {realtime}, Normal = {normal}";
                this.labelPopups.ForeColor = Color.Navy;
            }));
        }
    }
}
