using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BowlingLeague
{
    [Serializable()]
    class Bowler
    {
        string name;
        string lastName;
        double initialAverage;
        double[] means;
        bool[] active;
        List<List<int>> scores;
        Team team;

        public Bowler(string name, Team t, double mean, int startWeek)
        {
            SetName(name);

            this.team = t;
            this.initialAverage = mean;

            means = new double[31];
            active = new bool[31];

            // Expand scores list.
            scores = new List<List<int>>();
            for(int i = 0; i < 31; i++)
            {
                List<int> weeklyScores = new List<int>();
                for (int q = 0; q < 3; q++)
                    weeklyScores.Add(0);
                scores.Add(weeklyScores);
                // Sets the weeks for which bowler is considered active.
                if(i >= startWeek)
                    active[i] = true;
                means[i] = initialAverage;
            }
        }

        // Gets that week's scores.
        public List<int> GetScores(int week)
        {
            return scores[week];
        }

        // Sets the scores for that week. scores list will always be properly sized because GetScores() is always called before.
        public double SetScores(int week, String scoreString)
        {
            String[] split = scoreString.Trim().Split(' ');

            if (split.Length != 3)
                return -1;

            // List that is to replace scores[week].
            List<int> replacementScores = new List<int>();

            for(int i = 0; i < split.Length; i++){
                string value = split[i];
                int number;
                // If it's an int, then it should be a valid bowling score.
                if (int.TryParse(value, out number) && number <= 300 && number >= 0)
                    replacementScores.Add(number);
                else
                    return -1;
            }
            
            scores[week] = replacementScores;
            return Math.Round(UpdateMean(week));
        }

        public double GetMean(int week, bool toRound)
        {     
            if (toRound)
                return Math.Round(means[week], 2);

            return means[week];
        }

        private double UpdateMean(int week)
        {
            double mean = 0;
            // If a player skips or otherwise gets a 0 in a game, then it doesn't affect his average. divisor is used to avoid counting these games.
            int divisor = 0;

            for(int i = 1; i < scores.Count; i++)
            {
                foreach(int score in scores[i])
                {
                    mean += score;
                    if (score != 0)
                        divisor++;
                }
                // Used to avoid problems with players who haven't done anything yet.
                if (mean == 0 || divisor == 0)
                    means[i] = initialAverage;
                else
                    means[i] = mean / (double) divisor;
            }
            
            return means[week];
        }

        // Used for dealing with player add/drops: if a player is active at 'week,' he's a valid contributor to his team's score for that week.
        public bool IsActive(int week)
        {
            return active[week];
        }

        public double GetInitialAverage()
        {
            return initialAverage;
        }

        public string GetName()
        {
            return name;
        }

        public Team GetTeam()
        {
            return team;
        }

        public string GetLastName()
        {
            return lastName;
        }

        // Sets or replaces the bowler's name - it requires a first and a last name.
        public bool SetName(String name)
        {
            name = name.Trim();
            // Name will only contain a space if there are at least two words.
            if (!name.Contains(" "))
                return false;
            this.name = name;
            this.lastName = name.Substring(this.name.IndexOf(" ") + 1);
            return true;
        }

        // Set a player to be inactive starting at startWeek.
        public void SetInactive(int startWeek)
        {
            for(int i = startWeek; i < 31; i++)
            {
                active[i] = false;
            }
        }

    }
}
