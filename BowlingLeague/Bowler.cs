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
        List<List<int>> scores;
        Team team;

        public Bowler(string name, Team t, double mean)
        {
            SetName(name);
            this.team = t;
            this.initialAverage = mean;
            means = new double[31];
            scores = new List<List<int>>();
        }

        // Gets that week's scores, expands scores list as necessary.
        public List<int> GetScores(int week)
        {
            while (scores.Count <= week)
            {
                List<int> weeklyScores = new List<int>();
                for (int i = 0; i < 3; i++)
                    weeklyScores.Add(0);
                scores.Add(weeklyScores);
            }
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
            if (!IsActive())
                return initialAverage;
            
            if (toRound)
                return Math.Round(means[week], 2);

            return means[week];
        }

        private double UpdateMean(int week)
        {
            double mean = 0;
            // If a player skips or otherwise gets a 0 in a game, then it doesn't affect his average. divisor is used to avoid counting these games.
            int divisor = 0;
            for(int i = 0; i < scores.Count; i++)
            {
                foreach(int score in scores[i])
                {
                    mean += score;
                    if (score != 0)
                        divisor++;
                }
                means[i] = mean / (double) divisor;
            }
            
            if (divisor == 0 || mean == 0)
                return 0;
            return means[week];
        }

        // Used for supplying initial averages - tells if a player hasn't played yet.
        public bool IsActive()
        {
            foreach(List<int> scoreList in scores)
            {
                foreach (int score in scoreList)
                {
                    if (score > 0)
                        return true;
                }
            }
            return false;
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

    }
}
