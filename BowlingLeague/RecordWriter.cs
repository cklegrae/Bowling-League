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
        /// <summary> Uses an HTML template to create a printable record of this league's information. </summary>
        public static void Write(int week)
        {
            LeagueStats stats = new LeagueStats(week);
            HtmlDocument document = new HtmlDocument();
            document.Load(@"./rankingsTemplate.html");

            WriteStatsPage(document, stats, week);

            document.Save(@"C:\\Bowling\Records\recordsWeek" + week + ".html");
            
            document.Load(@"./scoreCardTemplate.html");

            HtmlNode body = document.GetElementbyId("div");
            for (int i = 0; i < League.teams.Count / 2; i++)
            {
                Matchup matchup = League.matchups[week * League.teams.Count / 2 + i];
                if (i > 0)
                    body.AppendChild(HtmlNode.CreateNode("<div class=\"pagebreak\"></div>"));
                WriteMatchupPage(document, stats, week, matchup, body);
            }

            document.Save(@"C:\\Bowling\Records\cardsWeek" + week + ".html");

            Process.Start(@"C:\\Bowling\Records\recordsWeek" + week + ".html");
            Process.Start(@"C:\\Bowling\Records\cardsWeek" + week + ".html");
        }

        /// <summary> Creates the individual/team rankings and awards page. </summary>
        private static void WriteStatsPage(HtmlDocument document, LeagueStats stats, int week)
        {
            var headerTable = document.GetElementbyId("headerTable");
            headerTable.AppendChild(HtmlNode.CreateNode("<tr><td> Week " + week + " Rankings</td></tr>"));

            // Rankings table.
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

            // High singles and threes tables.
            var highSinglesTable = document.GetElementbyId("highSinglesTable");
            AddBowlerEntries(highSinglesTable, stats.GetHighBowlers(false), 3, 1, week);

            var highTriplesTable = document.GetElementbyId("highTriplesTable");
            AddBowlerEntries(highTriplesTable, stats.GetHighBowlers(true), 3, 2, week);

            // Last week singles table.
            if (week > 1)
            {
                var lastWeekSinglesTable = document.GetElementbyId("lastWeekSinglesTable");
                AddBowlerEntries(lastWeekSinglesTable, stats.GetWeekHighBowlers(), 2, 3, week);
            }

            // Team singles and threes tables.
            var teamSinglesTable = document.GetElementbyId("teamSinglesTable");
            var teamTriplesTable = document.GetElementbyId("teamTriplesTable");

            for (int i = 0; i < 2; i++)
            {
                teamSinglesTable.AppendChild(HtmlNode.CreateNode("<tr><td width = \"80%\">" + stats.GetHighTeams(false)[i].GetName() + "</td><td>" +
                        stats.GetHighTeams(false)[i].GetHighScore(false) + "</td></tr>"));
                teamTriplesTable.AppendChild(HtmlNode.CreateNode("<tr><td width = \"80%\">" + stats.GetHighTeams(true)[i].GetName() + "</td><td>" +
                        stats.GetHighTeams(true)[i].GetHighScore(true) + "</td></tr>"));
            }

            // Current and next week matchups tables.
            var currentWeekTable = document.GetElementbyId("currentWeekTable");
            var nextWeekTable = document.GetElementbyId("nextWeekTable");
            int matchupIndex = week * League.teams.Count / 2;

            for (int i = 0; i < League.teams.Count / 2; i++)
            {
                currentWeekTable.AppendChild(HtmlNode.CreateNode("<tr><td>" + League.matchups[matchupIndex + i].teams[0].GetName() + 
                    " vs. " + League.matchups[matchupIndex + i].teams[1].GetName() + "</td>"));
                nextWeekTable.AppendChild(HtmlNode.CreateNode("<tr><td>" + League.matchups[matchupIndex + League.teams.Count / 2 + i].teams[0].GetName() + 
                    " vs. " + League.matchups[matchupIndex + League.teams.Count / 2 + i].teams[1].GetName() + "</td>"));
            }

            // Team standings table.
            var teamTable = document.GetElementbyId("teamTable");
            for(int i = 0; i < League.teams.Count; i++)
            {
                Team team = stats.GetRankedTeams()[i];
                teamTable.AppendChild(HtmlNode.CreateNode("<tr><td>" + (i + 1) + "</td><td>" + team.GetName() + "</td><td>" + team.wins + "</td><td>" + team.losses + "</td><td>" + 
                    (stats.GetRankedTeams()[0].wins - team.wins) + "</td><td>" + Math.Round(team.GetTeamAverage(week), 2) + "</td></tr>"));
            }
        }

        /// <summary> Places in HTML for team stats for this week's matchups, each on individual pages. </summary>
        private static void WriteMatchupPage(HtmlDocument document, LeagueStats stats, int week, Matchup matchup, HtmlNode body)
        {
            body.AppendChild(HtmlNode.CreateNode("<br>"));
            body.AppendChild(HtmlNode.CreateNode("<table id=\"headerTable\" style=\"text-align: center; border-collapse: collapse; height: 100%; width: 100%;\" border=\"1\" cellpadding=\"2\" cellspacing=\"2\"><tbody><tr><td>Week " + (week + 1) + "</td></tr></tbody></table>"));
            body.AppendChild(HtmlNode.CreateNode("<br>"));

            int[] handicaps = matchup.DetermineHandicap();

            for (int i = 0; i < 2; i++)
            {
                body.AppendChild(HtmlNode.CreateNode("<br>"));
                HtmlNode table = HtmlNode.CreateNode("<table id=\"" + matchup.teams[i].GetName() + "\" style=\"text-align: center; border-collapse: collapse; height: 100%; width: 100%;\" border=\"1\" cellpadding=\"3\" cellspacing=\"4\"><col style=\"width: 40%;\"><col style=\"width: 15%;\"><col style=\"width: 10%;\"><col style=\"width: 10%;\"><col style=\"width: 10%;\"><col style=\"width: 15%;\"></table>");
                body.AppendChild(table);

                table.AppendChild(HtmlNode.CreateNode("<tr><td colspan = \"2\">Team " + matchup.teams[i].GetName() + "</td><td colspan = \"4\"></td></tr>"));
                table.AppendChild(HtmlNode.CreateNode("<tr><td>NAME</td><td>AVG</td><td>1</td><td>2</td><td>3</td><td>TOTAL</td></tr>"));

                List<Bowler> bowlers = new List<Bowler>(matchup.teams[i].GetBowlers());
                bowlers = bowlers.OrderBy(b => b.GetMean(week + 1, false)).ToList();
                for (int q = 0; q < bowlers.Count; q++)
                {
                    string bowlerRow = String.Format("<tr><td>{0}</td><td>{1}</td><td></td><td></td><td></td><td></td></tr>", bowlers[q].GetName(), bowlers[q].GetMean(week + 1, true));
                    table.AppendChild(HtmlNode.CreateNode(bowlerRow));
                }
                string averageTotalRow = String.Format("<tr><td></td><td>{0}</td><td></td><td></td><td></td><td></td></tr>", Math.Round(matchup.teams[i].GetTeamAverage(week + 1), 2));
                table.AppendChild(HtmlNode.CreateNode(averageTotalRow));

                string handicapString;
                if(handicaps[i] > 0)
                    handicapString = String.Format("<tr><td colspan = \"2\">HANDICAP</td><td>{0}</td><td>{0}</td><td>{0}</td><td>{1}</td></tr>", handicaps[i], handicaps[i] * 3);
                else
                    handicapString = "<tr><td colspan = \"2\">HANDICAP</td><td></td><td></td><td></td><td></td></tr>";
                table.AppendChild(HtmlNode.CreateNode(handicapString));

                table.AppendChild(HtmlNode.CreateNode("<tr><td colspan = \"2\">TEAM TOTAL</td><td></td><td></td><td></td><td></td></tr></tbody>"));
                body.AppendChild(HtmlNode.CreateNode("<br>"));
            }
        } 

        /// <summary> Fills in table entries related to bowler stats. </summary>
        private static void AddBowlerEntries(HtmlNode table, List<Bowler> bowlers, int desiredEntryCount, int modifier, int week)
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
                    table.AppendChild(HtmlNode.CreateNode("<tr><td width = \"80%\">" + bowlers[entryCount].GetName() + "</td><td>" + score + "</td></tr>"));
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
