using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
/// <summary>
/// RawMatch cache control
/// </summary>
public static class Cache
{
    public enum Priority { Realtime = 0, Normal = 1 }
    public class PopupInfo
    {
        public PopupInfo(Priority priority = Priority.Normal)
        {
            this.Priority = priority;
        }
        /// <summary>
        /// Match start date & time as ISO string.
        /// </summary>
        public string StartTime;
        /// <summary>
        /// Match status
        /// </summary>
        public string Status;
        /// <summary>
        /// All popup's info as json string.
        /// </summary>
        public string JsonData;
        /// <summary>
        /// Last update datetime.
        /// </summary>
        public DateTime Updated = DateTime.MinValue;
        /// <summary>
        /// Priority flag.
        /// </summary>
        public Priority Priority = Priority.Normal;
        public bool Active
        {
            get
            {
                switch (this.Status)
                {
                    case "Postponed":
                    case "Cancelled":
                    case "Finished":
                        return false;
                    default:
                        return true;
                }
            }
        }
        #region Parse
        class JsonPart { public string StartTime; public string Status; }
        public static PopupInfo Parse(string json, Priority priority = Priority.Normal)
        {
            var popup = new PopupInfo(priority);
            var part = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonPart>(json);
            popup.StartTime = part.StartTime;
            popup.Status = part.Status;
            popup.JsonData = json;
            popup.Updated = DateTime.Now;
            return popup;
        }
        #endregion
    }
    /// <summary>
    /// All rawMatches passed to GetModifiedScoresAndQueue
    /// </summary>
    private static Dictionary<string, RawMatch> _Data = new Dictionary<string, RawMatch>();
    /// <summary>
    /// Low priority queue
    /// </summary>
    private static Dictionary<string, RawMatch> _Queue = new Dictionary<string, RawMatch>();
    /// <summary>
    /// Info from popup
    /// </summary>
    public static Dictionary<string, PopupInfo> Popups = new Dictionary<string, PopupInfo>();
    /// <summary>
    /// High Priority Updates. Get score modifications and queue the rest of modifications. In every call cache is reset.
    /// </summary>
    public static List<RawMatch[]> GetModifiedScoresAndQueue(Dictionary<string, RawMatch> data, int chunkSize = 20)
    {
        // Add time & details to rawMatches from popups, schedule popup's reading.
        foreach (var item in data)
        {
            var rawMatch = item.Value;
            var popup = (Popups.ContainsKey(item.Key)) ? Popups[item.Key] : null;
            if (popup != null && !string.IsNullOrWhiteSpace(popup.StartTime))
            {
                rawMatch.StartTime = popup.StartTime;
                rawMatch.EmptyTime = false;
                rawMatch.Details = popup.JsonData;
            }
            // Schedule popup's reading
            if (!Popups.ContainsKey(item.Key))
            {
                if (!rawMatch.HasTime())
                {
                    Popups[item.Key] = new PopupInfo(Priority.Realtime);
                }
                else
                {
                    Popups[item.Key] = new PopupInfo(Priority.Normal);
                }
            }
        }
        // Filter matches with empty time (we don't post matches with empty time)
        var filter = data.Where(x => x.Value.HasTime()).ToDictionary(k => k.Key, v => v.Value);
        // Separate realtime & normal priority updates
        var realtime = new List<RawMatch>();
        foreach (var item in filter)
        {
            var rawMatchNew = item.Value;
            var rawMatchOld = (_Data.ContainsKey(item.Key)) ? _Data[item.Key] : null;
            // Realtime updates
            if (rawMatchOld == null || rawMatchNew.IsModifiedScore(rawMatchOld))
            {
                realtime.Add(rawMatchNew);
                // Read popup again, immediately
                if (rawMatchOld != null && Popups.ContainsKey(item.Key))
                {
                    Popups[item.Key].Priority = Priority.Realtime;
                }
                // Realtime updates must not be in normal updates
                if (_Queue.ContainsKey(item.Key)) _Queue.Remove(item.Key);
            }
            // Normal updates
            else if (rawMatchNew.IsModifiedInfo(rawMatchOld))
            {
                _Queue[item.Key] = rawMatchNew;
            }
        }
        // Reset
        _Data = filter;
        // Return
        return Chunks(realtime, chunkSize);
    }
    /// <summary>
    /// Low Priority Updates.
    /// </summary>
    public static List<RawMatch[]> GetQueue(int chunkSize = 30)
    {
        var list = new List<RawMatch>();
        while (_Queue.Count > 0)
        {
            var item = _Queue.First();
            _Queue.Remove(item.Key);
            list.Add(item.Value);
        }
        return Chunks(list, chunkSize);
    }
    /// <summary>
    /// 
    /// </summary>
    public static List<T[]> Chunks<T>(List<T> list, int size)
    {
        var result = new List<T[]>();
        var items = new Queue<T>(list);
        while (items.Count > 0)
        {
            var chunk = new List<T>();
            while (items.Count > 0 && chunk.Count < size)
            {
                var item = items.Dequeue();
                chunk.Add(item);
            }
            result.Add(chunk.ToArray());
        }
        return result;
    }
    public static string Filepath
    {
        get
        {
            var day = $"{DateTime.Now:yyyy-MM-dd}.json";
            var s = day.Split('-');
            var dir = Path.Combine(Application.LocalUserAppDataPath, "Popups", s[0], s[1]);
            var path = Path.Combine(dir, s[2]);
            Directory.CreateDirectory(dir);
            return path;
        }
    }
    public static void Save()
    {
        try
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(Popups);
            File.WriteAllText(Filepath, json);
        }
        catch { }
    }
    public static void Restore()
    {
        try
        {
            var json = File.ReadAllText(Filepath);
            Popups = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PopupInfo>>(json);
        }
        catch { }
    }
}