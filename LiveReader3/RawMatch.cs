using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
public enum MatchStatus
{
    Waiting,     // Will start on scheduled start date & time
    Postponed,   // Will not start on scheduled start date & time
    Cancelled,   // Will never start
    FirstHalf,   // Live in period 1st half
    HalfTime,    // Live in half time break
    SecondHalf,  // Live in period 2nd half
    ExtraTime,   // Live in period extra time
    PenaltyKick, // Live in period penalty kick
    Finished,    // Finished after full time
    AfterET,     // Finished after extra time
    AfterPK,     // Finished after penalty kick
    Abandoned,   // Started but will not finish
}
public class RawMatch
{
    #region Fields
    public string WebId;
    public string Source;
    public string Language;
    public string Sport;
    public string Country;
    public string Champ;
    public string StartTime;
    public bool? EmptyTime; // Means that StartTime has only date, time not valid
    public string HomeTeam;
    public string AwayTeam;
    public string HomeYellowCards;
    public string AwayYellowCards;
    public string AwayRedCards;
    public string HomeRedCards;
    public string HomeScore;
    public string AwayScore;
    public string HomeScoreHT;
    public string AwayScoreHT;
    public string HomeScoreET;
    public string AwayScoreET;
    public string HomeScorePK;
    public string AwayScorePK;
    public string Status;
    public string Details;
    #endregion
    #region Methods
    public delegate void WriteLog(string[] messages, int limit = 9999);
    public bool IsModifiedScore(RawMatch match)
    {
        var modified = false;
        if (match != null)
        {
            if (this.HomeScoreHT != match.HomeScoreHT) modified = true;
            if (this.AwayScoreHT != match.AwayScoreHT) modified = true;
            if (this.HomeScore != match.HomeScore) modified = true;
            if (this.AwayScore != match.AwayScore) modified = true;
            if (this.HomeScoreET != match.HomeScoreET) modified = true;
            if (this.AwayScoreET != match.AwayScoreET) modified = true;
        }
        return modified;
    }
    public bool IsModifiedInfo(RawMatch match)
    {
        var modified = false;
        if (match != null)
        {
            if (this.WebId != match.WebId) modified = true;
            if (this.Source != match.Source) modified = true;
            if (this.Language != match.Language) modified = true;
            if (this.Sport != match.Sport) modified = true;
            if (this.Country != match.Country) modified = true;
            if (this.Champ != match.Champ) modified = true;
            if (this.StartTime != match.StartTime) modified = true;
            if (this.Status != match.Status) modified = true;
            if (this.HomeTeam != match.HomeTeam) modified = true;
            if (this.AwayTeam != match.AwayTeam) modified = true;
            if (this.HomeRedCards != match.HomeRedCards) modified = true;
            if (this.AwayRedCards != match.AwayRedCards) modified = true;
        }
        return modified;
    }
    public bool IsModified(RawMatch match)
    {
        var modified = false;
        if (this.IsModifiedInfo(match)) modified = true;
        if (this.IsModifiedScore(match)) modified = true;
        return modified;
    }
    public bool HasTime()
    {
        if (this.EmptyTime == true) return false;
        if (string.IsNullOrEmpty(this.StartTime)) return false;
        if (this.StartTime.Length <= 15) return false;
        return true;
    }
    public static void Post(WriteLog log, params RawMatch[] rawMatches)
    {
        var url = "https://europe-west1-play90.cloudfunctions.net/updateRawMatches";
        var list = new List<object>();
        foreach (var rawMatch in rawMatches)
        {
            (var status, var minute) = StatusFlashscore(rawMatch.Status);
            list.Add(new
            {
                WebId = rawMatch.WebId,
                Source = rawMatch.Source,
                Language = rawMatch.Language,
                Sport = rawMatch.Sport,
                Country = rawMatch.Country,
                Champ = rawMatch.Champ,
                StartTime = rawMatch.StartTime,
                EmptyTime = $"{!rawMatch.HasTime()}",
                Status = $"{status}",
                Minute = $"{minute}",
                HomeTeam = rawMatch.HomeTeam,
                AwayTeam = rawMatch.AwayTeam,
                Score = $"{rawMatch.HomeScore}-{rawMatch.AwayScore}",
                ScoreHT = (FinishedHalf(status)) ? $"{rawMatch.HomeScoreHT}-{rawMatch.AwayScoreHT}" : null,
                ScoreFT = (Finished(status)) ? $"{rawMatch.HomeScore}-{rawMatch.AwayScore}" : null,
                ScoreET = $"{rawMatch.HomeScoreET}-{rawMatch.AwayScoreET}",
                RedCards = $"{rawMatch.HomeRedCards}-{rawMatch.AwayRedCards}",
                Details = rawMatch.Details
            });
        }
        var data = new System.Collections.Specialized.NameValueCollection();
        var json = JsonConvert.SerializeObject(list.ToArray());
        data["json"] = json;
        var bytes = TimedWebClient.PostData(data, url);
        var text = Encoding.UTF8.GetString(bytes);
        var results = JsonConvert.DeserializeObject<string[]>(text);
        log(results);
    }
    public static Task PostAsync(WriteLog log, params RawMatch[] rawMatches)
    {
        return Task.Run(new Action(() =>
        {
            RawMatch.Post(log, rawMatches);
        }));
    }
    #endregion
    #region Static
    public static Dictionary<string, RawMatch> ParseData(string json)
    {
        var data = JsonConvert.DeserializeObject<Dictionary<string, RawMatch>>(json);
        return data;
    }
    public static bool FinishedHalf(MatchStatus? status)
    {
        switch (status)
        {
            case MatchStatus.HalfTime:
            case MatchStatus.SecondHalf:
            case MatchStatus.Finished:
            case MatchStatus.AfterET:
            case MatchStatus.AfterPK:
                return true;
            default:
                return false;
        }
    }
    public static bool Finished(MatchStatus? status)
    {
        switch (status)
        {
            case MatchStatus.Finished:
            case MatchStatus.AfterET:
            case MatchStatus.AfterPK:
                return true;
            default:
                return false;
        }
    }
    public static (MatchStatus?, int?) StatusFlashscore(string status)
    {
        if (!string.IsNullOrWhiteSpace(status))
        {
            try
            {
                if (int.TryParse(status.Replace("'","").Trim(), out int minute))
                {
                    if (minute <= 45)
                    {
                        return (MatchStatus.FirstHalf, minute);
                    }
                    else
                    {
                        return (MatchStatus.SecondHalf, minute);
                    }
                }
                else if (status.Contains("+"))
                {
                    var s = status.Replace("'", "").Trim().Split('+');
                    if (s.Length == 2 && int.TryParse(s[0].Trim(), out int min))
                    {
                        if (int.TryParse(s[1].Trim(), out int ext))
                        {
                            if (min <= 45)
                            {
                                return (MatchStatus.FirstHalf, min + ext);
                            }
                            else
                            {
                                return (MatchStatus.SecondHalf, min + ext);
                            }
                        }
                        else
                        {
                            if (min <= 45)
                            {
                                return (MatchStatus.FirstHalf, min);
                            }
                            else
                            {
                                return (MatchStatus.SecondHalf, min);
                            }
                        }
                    }
                }
                else
                {
                    switch (status.Trim().ToLower())
                    {
                        case "half time": return (MatchStatus.HalfTime, null);
                        case "finished": return (MatchStatus.Finished, null);
                        case "after et": return (MatchStatus.AfterET, null);
                        case "after pen.": return (MatchStatus.AfterPK, null);
                        case "walkover": return (MatchStatus.Postponed, null);
                        case "postponed": return (MatchStatus.Postponed, null);
                        case "cancelled": return (MatchStatus.Cancelled, null);
                        case "abandoned": return (MatchStatus.Abandoned, null);
                        case "fro":
                        default: // StartTime
                            break;
                    }
                }
            }
            catch { }
        }
        return (null, null);
    }
    #endregion
}