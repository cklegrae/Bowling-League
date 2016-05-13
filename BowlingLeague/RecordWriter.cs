using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Web;
using HtmlAgilityPack;

namespace BowlingLeague
{
    static class RecordWriter
    {
        public static void Write(int week)
        {
            LeagueStats stats = new LeagueStats(week);
            HtmlDocument document = new HtmlDocument();
            document.Load(@"./rankingsTemplate.html");

            var headerTable = document.GetElementbyId("headerTable");
            headerTable.AppendChild(HtmlNode.CreateNode("<tr><td style = \"text-align: center;\"> Week " + week + " Rankings</td></tr>"));

            var rankingsTable = document.GetElementbyId("rankingsTable");

            int rank = 0;
            foreach (Bowler bowler in stats.GetRankedBowlers())
            {
                if (!bowler.IsOpenSlot())
                {
                    rank++;
                    rankingsTable.AppendChild(HtmlNode.CreateNode("<tr><td width = \"10%\">" + rank + "</td><td width = \"75%\">" + bowler.GetName() + "</td><td>" + bowler.GetMean(week, true) + "</tr>"));
                }
            }

            var highSinglesTable = document.GetElementbyId("highSinglesTable");
            AddBowlerEntries(highSinglesTable, stats.GetHighBowlers(false), 3, 1, week);

            var highTriplesTable = document.GetElementbyId("highTriplesTable");
            AddBowlerEntries(highTriplesTable, stats.GetHighBowlers(true), 3, 2, week);

            if (week > 1) {
                var lastWeekSinglesTable = document.GetElementbyId("lastWeekSinglesTable");
                AddBowlerEntries(lastWeekSinglesTable, stats.GetWeekHighBowlers(), 2, 3, week);
            }

            var teamSinglesTable = document.GetElementbyId("teamSinglesTable");
            var teamTriplesTable = document.GetElementbyId("teamTriplesTable");

            for (int i = 0; i < 2; i++)
            {
                teamSinglesTable.AppendChild(HtmlNode.CreateNode("<tr><td style = \"text-align: center;\" width = \"80%\">" + stats.GetHighTeams(false)[i].GetName() + "</td><td style = \"text-align: center;\">" +
                        stats.GetHighTeams(false)[i].GetHighScore(false) + "</td></tr>"));
                teamTriplesTable.AppendChild(HtmlNode.CreateNode("<tr><td style = \"text-align: center;\" width = \"80%\">" + stats.GetHighTeams(true)[i].GetName() + "</td><td style = \"text-align: center;\">" +
                        stats.GetHighTeams(true)[i].GetHighScore(true) + "</td></tr>"));
            }

            var currentWeekTable = document.GetElementbyId("currentWeekTable");
            var nextWeekTable = document.GetElementbyId("nextWeekTable");
            int matchupIndex = week * League.teams.Count / 2;

            for (int i = 0; i < League.teams.Count / 2; i++)
            {
                currentWeekTable.AppendChild(HtmlNode.CreateNode("<tr><td style = \"text-align: center;\">" + League.matchups[matchupIndex + i].teams[0].GetName() + " vs. " + League.matchups[matchupIndex + i].teams[1].GetName() + "</td>"));
                nextWeekTable.AppendChild(HtmlNode.CreateNode("<tr><td style = \"text-align: center;\">" + League.matchups[matchupIndex + 3 + i].teams[0].GetName() + " vs. " + League.matchups[matchupIndex + 3 + i].teams[1].GetName() + "</td>"));
            }


            document.Save(@"C:\\Bowling\Records\recordsWeek" + week + ".html");
            Process.Start(@"C:\\Bowling\Records\recordsWeek" + week + ".html");
        }

        public static void AddBowlerEntries(HtmlNode table, List<Bowler> bowlers, int desiredEntryCount, int modifier, int week)
        {
            int entryCount = 0;
            while(entryCount < desiredEntryCount)
            {
                if (!bowlers[entryCount].IsOpenSlot())
                {
                    double score = 0;
                    switch (modifier)
                    {
                        case 1: score = bowlers[entryCount].GetHighScore(false);
                                break;
                        case 2: score = bowlers[entryCount].GetHighScore(true);
                                break;
                        case 3: score = bowlers[entryCount].GetScores(week - 1).Max();
                                break;
                    }
                    table.AppendChild(HtmlNode.CreateNode("<tr><td style = \"text-align: center;\" width = \"80%\">" + bowlers[entryCount].GetName() + "</td><td style = \"text-align: center;\">" + 
                        score + "</td></tr>"));
                }
                else
                {
                    desiredEntryCount++;
                }
                entryCount++;
            }
        }
    }
}
