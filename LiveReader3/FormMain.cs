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
            //Cache.Restore();
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
        /// <summary>
        /// Realtime + Normal Workers
        /// </summary>
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
                    if (!this.workerMatches.IsBusy && !e.IsLoading)
                    {
                        this.workerMatches.RunWorkerAsync();
                    }
                    if (!this.workerUploads.IsBusy && !e.IsLoading)
                    {
                        this.workerUploads.RunWorkerAsync();
                    }
                }));
            };
            this.browser.LoadError += (sender, e) =>
            {
                this.WriteLog($"Browser LoadError: {e.ErrorText}");
            };
            this.workerMatches.DoWork += (sender, e) =>
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
                    if (response.Success && response.Result != null)
                    {
                        var result = (string)response.Result;
                        var data = RawMatch.ParseData(result);
                        this.WriteStatus($"Found {data.Values.Count} matches [{((DateTime.Now - time).TotalMilliseconds):#,##0}ms]");
                        Cache.Update(data);
                    }
                    else
                    {
                        this.WriteStatus($"Read failure => {response.Message}", true);
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            };
            this.workerUploads.DoWork += (sender, e) =>
            {
                while (true)
                {
                    var chunk = Cache.DequeueChunk();
                    if (chunk.Length > 0)
                    {
                        var t = DateTime.Now;
                        if (RawMatch.Post(this.WriteUploadLog, chunk))
                        {
                            this.WriteLog($"Posted {chunk.Length} update(s) [{((DateTime.Now - t).TotalMilliseconds):#,##0}ms]");
                        }
                        else
                        {
                            this.WriteLog($"Error on posting {chunk.Length} update(s) [{((DateTime.Now - t).TotalMilliseconds):#,##0}ms]");
                            Cache.EnqueueChunk(chunk);
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(200);
                    }
                }
            };
        }
        /// <summary>
        /// Popup Worker
        /// </summary>
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
                    if (!this.workerPopups.IsBusy && !e.IsLoading)
                    {
                        this.workerPopups.RunWorkerAsync();
                    }
                }));
            };
            this.popup.LoadError += (sender, e) =>
            {
                this.Invoke(new Action(() =>
                {

                }));
            };
            this.workerPopups.DoWork += (sender, e) =>
            {
                var wait = 2 * 1000; // Miliseconds
                var i = 0;
                while (true)
                {
                    // Realtime (score changed or unread)
                    var priority = Cache.Priority.Realtime;
                    var query = Cache.Popups.Where(x => x.Value.Priority == priority).OrderBy(x => x.Value.Updated).ToArray();
                    // Normal (is live)
                    if (query.Length == 0 && i < 3) // <= read 3 normal and 1 low
                    {
                        priority = Cache.Priority.Normal;
                        query = Cache.Popups.Where(x => x.Value.Priority == priority && x.Value.Active && TimeDiff(x.Value.Updated).TotalMinutes >= 1).OrderBy(x => x.Value.Updated).ToArray();
                        i++;
                    }
                    // Low (not started)
                    if (query.Length == 0)
                    {
                        priority = Cache.Priority.Low;
                        query = Cache.Popups.Where(x => x.Value.Priority == priority && x.Value.Active && TimeDiff(x.Value.Updated).TotalHours >= 1).OrderBy(x => x.Value.Updated).ToArray();
                        i = 0;
                    }
                    // Read
                    if (query.Length > 0)
                    {
                        var item = query[0];
                        var info = this.ReadPopup(item.Key, wait);
                        if (info != null)
                        {
                            this.WritePopupLog($"Popup read success [{priority}] => Key: {item.Key}, Length: {info.JsonData.Length}, StartTime: {info.StartTime}");
                            Cache.Popups[item.Key] = info;
                            this.UpdatePopupStatus();
                            //Cache.Save();
                            continue;
                        }
                    }
                    // Break if nothing to do
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
                        if (string.IsNullOrWhiteSpace(info.Status))
                        {
                            info.Priority = Cache.Priority.Low;
                        }
                        else
                        {
                            info.Priority = Cache.Priority.Normal;
                        }
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
        private void WriteUploadLog(string[] messages, int limit = 9999)
        {
            this.WriteLog(this.listUploadLog, messages, limit);
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
            var normal = Cache.Popups.Where(x => x.Value.Priority == Cache.Priority.Normal && x.Value.Active).Count();
            var low = Cache.Popups.Where(x => x.Value.Priority == Cache.Priority.Low && x.Value.Active).Count();
            var never = Cache.Popups.Where(x => !x.Value.Active).Count();
            var total = realtime + normal + low + never;
            this.Invoke(new Action(() =>
            {
                this.labelPopups.Text = $"Popup Cache: Realtime = {realtime}, Normal = {normal}, Low = {low}, Never = {never} [Total: {total}]";
                this.labelPopups.ForeColor = Color.Navy;
            }));
        }
        public static TimeSpan TimeDiff(DateTime t)
        {
            return DateTime.Now - t;
        }
        public static TimeSpan TimeDiff(string s)
        {
            return TimeDiff(DateTime.Parse(s));
        }
    }
}
