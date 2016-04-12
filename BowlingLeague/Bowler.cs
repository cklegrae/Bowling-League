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
        private string name;
        private string lastName;
        private double initialAverage;
        private double[] means;
        private bool[] active;
        private List<List<int>> scores;
        private Team team;
        private int highSingle = 0;
        private int highTriple = 0;
        private bool isOpenSlot;

        public Bowler(string name, Team t, double mean, int startWeek, bool isOpenSlot)
        {
            SetName(name);

            this.team = t;
            this.initialAverage = mean;

            means = new double[31];
            active = new bool[31];

            scores = new List<List<int>>();
            for(int i = 0; i < 31; i++)
            {
                List<int> weeklyScores = new List<int>();
                for (int q = 0; q < 3; q++)
                    weeklyScores.Add(0);

                scores.Add(weeklyScores);

                if (i >= startWeek)
                    active[i] = true;
                means[i] = initialAverage;
                this.isOpenSlot = isOpenSlot;
            }
        }

        /// <summary> Sets the scores for that week. scores list will always be properly sized because GetScores() is always called before. </summary>
        public double SetScores(int week, String scoreString)
        {
            String[] split = scoreString.Trim().Split(' ');

            if (split.Length != 3)
                return -1;

            List<int> replacementScores = new List<int>();

            for(int i = 0; i < split.Length; i++){
                string value = split[i];
                int number;
                if (int.TryParse(value, out number) && number <= 300 && number >= 0)
                    replacementScores.Add(number);
                else
                    return -1;
            }
            
            scores[week] = replacementScores;
            return Math.Round(UpdateMean(week), 2);
        }

        public double GetMean(int week, bool toRound)
        {     
            if (toRound)
                return Math.Round(means[week], 2);

            return means[week];
        }

        /// <summary> Updates bowler's mean. </summary>
        private double UpdateMean(int week)
        {
            double mean = 0;
            int divisor = 0;
            for(int i = 1; i < scores.Count; i++)
            {
                foreach(int score in scores[i])
                {
                    mean += score;
                    if (score != 0)
                        divisor++;
                }

                if (mean == 0 || divisor == 0)
                    means[i] = initialAverage;
                else
                    means[i] = mean / (double) divisor;
            }
            
            return means[week];
        }

        /// <summary> Updates bowler high scores in relation to current week. Used for reprinting of previous weeks' information. </summary>
        public void UpdateStats(int week)
        {
            int triple;
            highSingle = 0;
            highTriple = 0;
            for(int i = 1; i <= week; i++)
            {
                triple = 0;
                foreach(int score in scores[i])
                {
                    triple += score;
                    if (score > highSingle)
                        highSingle = score;
                }
                if (triple > highTriple)
                    highTriple = triple;
            }
        }

        /// <summary> Sets or replaces the bowler's name - it requires a first and a last name. </summary>
        public bool SetName(String name)
        {
            name = name.Trim();
            if (!name.Contains(" "))
                return false;
            this.name = name;
            this.lastName = name.Substring(this.name.IndexOf(" ") + 1);
            return true;
        }

        /// <summary> Set a player to be inactive starting at startWeek. </summary>
        public void SetInactive(int startWeek)
        {
            for(int i = startWeek; i < 31; i++)
            {
                active[i] = false;
            }
        }

        /// <summary> Checks if a player should be included in rankings. </summary>
        public bool IsValidForPrinting(int week)
        {
            if (means[week] == initialAverage)
            {
                if(ParticipationCounter(week) == 0)
                    return false;
            }
            return true;
        }

        /// <summary> Counts the number of weeks a player has physically attended. </summary>
        public int ParticipationCounter(int week)
        {
            int count = 0;
            for(int i = 1; i <= week; i++)
            {
                for (int q = 0; q < 3; q++)
                {
                    if (scores[i][q] > 0)
                    {
                        count++;
                        break;
                    }
                }
            }
            return count;
        }

        /// <summary> Checks whether the player is a valid contributor to his team's score for that week. </summary>
        public bool IsActive(int week)
        {
            return active[week];
        }

        public List<int> GetScores(int week)
        {
            return scores[week];
        }

        public Team GetTeam()
        {
            return team;
        }

        public string GetName()
        {
            return name;
        }

        public string GetLastName()
        {
            return lastName;
        }
        
        public void SetInitialAverage(double avg)
        {
            initialAverage = avg;
            UpdateMean(30);
        }

        public double GetInitialAverage()
        {
            return initialAverage;
        }

        /// <summary> Returns bowler's highest single game score or single week score (triple). </summary>
        public int GetHighScore(bool getTriple)
        {
            if (getTriple)
                return highTriple;
            return highSingle;
        }

        public bool IsOpenSlot()
        {
            return isOpenSlot;
        }

    }
}
