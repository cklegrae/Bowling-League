using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingLeague
{

    [Serializable()]
    class Team
    {
        private List<Bowler> bowlers;
        private string name;
        private int id;
        private int highSingle = 0;
        private int highTriple = 0;
        public double wins;
        public double losses;

        public Team(string name, int id)
        {
            this.id = id;
            this.name = name;
            bowlers = new List<Bowler>();
        }

        public void AddBowler(Bowler b)
        {
            bowlers.Add(b);
        }

        public double GetTeamAverage(int week)
        {
            double result = 0;
            foreach(Bowler b in bowlers)
            {
                if(b.IsActive(week))
                    result += b.GetMean(week, false);
            }
            return result;
        }

        /// <summary> Returns the three total team scores for the week. Updates high scores if necessary. </summary>
        public List<int> ProcessTeamScores(int week)
        {
            List<int> weekScores = new List<int>();
            int tripleScore = 0;

            for(int i = 0; i < 3; i++)
            {
                int total = 0;

                foreach(Bowler b in bowlers)
                {
                    if (b.IsActive(week))
                    {
                        int score = b.GetScores(week)[i];
                        // A player who misses a game will contribute his (last week's) average - 5 to his team score.
                        if (score == 0)
                            total += (int) Math.Floor(b.GetMean(week - 1, false) - 5);
                        else
                            total += b.GetScores(week)[i];
                    }
                }

                if (total > highSingle)
                    highSingle = total;
                tripleScore += total;
                weekScores.Add(total);
            }

            if (tripleScore > highTriple)
                highTriple = tripleScore;
            return weekScores;
        }

        /// <summary> Places the replacement in the same position that the original was in to avoid label position issues. It is sorted into its own spot upon reload. </summary>
        public bool ReplaceBowler(Bowler toBeRemoved, Bowler replacement, int week)
        {
            int index = bowlers.IndexOf(toBeRemoved);
            if (index < 0)
                return false;
            bowlers[index] = replacement;

            // Removed player remains on team. This is to maintain their role in previous weeks' matchups.
            toBeRemoved.SetInactive(week);
            bowlers.Add(toBeRemoved);
            return true;
        }

        /// <summary> The id represents the team in the scheduling system referred to by the bowling league: 1 vs 6, 2 vs 4, etc. </summary>
        public int GetId()
        {
            return id;
        }

        public string GetName()
        {
            return name;
        }

        /// <summary> Sorts by last name. </summary>
        public void SortBowlers()
        {
            bowlers = bowlers.OrderBy(b => b.GetLastName()).ToList();
        }

        public List<Bowler> GetBowlers()
        {
            return bowlers;
        }

        public Bowler GetBowlerAt(int index)
        {
            return bowlers[index];
        }

        /// <summary> Returns team's highest single score or highest week score (triple). </summary>
        public int GetHighScore(bool getTriple)
        {
            if (getTriple)
                return highTriple;
            return highSingle;
        }

        /// <summary> Resets team stats before recalculating matchups and scores. </summary>
        public void ResetStats()
        {
            wins = 0;
            losses = 0;
            highSingle = 0;
            highTriple = 0;
        }
        
    }
}
