using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BowlingLeague
{
    static class League
    {
        public static List<Bowler> bowlers;
        public static List<Team> teams;
        public static List<Matchup> matchups;
        // Set to currentWeek + 1 upon print, so that it automatically loads a new week the majority of time a new week has actually passed.
        public static int nextWeek = 1;

        // Loads a new set of teams at the beginning of a season.
        public static void SetTeams(String filePath)
        {
            teams = new List<Team>();
            bowlers = new List<Bowler>();

            if (File.Exists(filePath))
            {
                string[] stream = File.ReadAllLines(filePath);
                if (!stream[0].Equals("Teams"))
                    return;
                Team team = null;

                // All team names preceded by asterisk.
                for (int i = 1; i < stream.Length; i++)
                {
                    if (stream[i].ElementAt(0).Equals('*'))
                    {
                        if (team != null)
                        {
                            teams.Add(team);
                        }
                        team = new Team(stream[i].Substring(2), stream[i].ElementAt(1));
                    }
                    else
                    {
                        Bowler bowler = new Bowler(stream[i], team, 90, 0);
                        if (team != null)
                        {
                            team.AddBowler(bowler);
                        }
                    }
                }

                // Adds final team.
                teams.Add(team);

                UpdateBowlers(true, 0);
            }
        }

        // Loads a new set of matchups at the beginning of a season. Requires teams to be set.
        public static void SetMatchups(String filePath)
        {
            if (teams == null || teams.Count == 0)
                return;
            matchups = new List<Matchup>();
            if (File.Exists(filePath))
            {
                string[] stream = File.ReadAllLines(filePath);
                if (!stream[0].Equals("Matchups"))
                    return;
                for (int i = 1; i < stream.Length; i++)
                {
                    int teamOne = stream[i].ElementAt(0);
                    int teamTwo = stream[i].ElementAt(2);
                    int week = Convert.ToInt32(stream[i].Substring(4).Trim());
                    Matchup matchup = new Matchup(teams.Find(team => team.GetId() == teamOne), teams.Find(team => team.GetId() == teamTwo), week);
                    matchups.Add(matchup);
                }
                MessageBox.Show("Matchups successfully imported.");
                WriteToFile();
            }
        }

        // Updates the set of bowlers after player removal and when changing weeks. Doesn't always sort to avoid Label related ordering problems.
        public static void UpdateBowlers(bool toBeSorted, int week)
        {
            if (teams == null || teams.Count == 0)
                return;
            bowlers = new List<Bowler>();
            foreach(Team t in teams)
            {
                if(toBeSorted)
                    t.SortBowlers();
                for (int i = 0; i < t.GetBowlers().Count; i++)
                {
                    // Don't add bowlers to this week's set if they aren't active on a team.
                    if(t.GetBowlerAt(i).IsActive(week))
                        bowlers.Add(t.GetBowlerAt(i));
                }
            }
            WriteToFile();
        }

        public static void WriteToFile()
        {
            using (Stream s = File.Open("C:\\Bowling\\league.bin", FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                LeagueHelper helper = new LeagueHelper();
                helper.SetTeams(teams);
                helper.SetMatchups(matchups);
                helper.SetWeek(nextWeek);
                bin.Serialize(s, helper);
                s.Close();
            }
        }

    }
}
