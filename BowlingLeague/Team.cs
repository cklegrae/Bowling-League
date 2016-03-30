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

        // Returns the three total team scores for the week.
        public List<int> GetTeamScores(int week)
        {
            List<int> weekScores = new List<int>();
            for(int i = 0; i < 3; i++)
            {
                int total = 0;
                foreach(Bowler b in bowlers)
                {
                    if(b.IsActive(week))
                        total += b.GetScores(week)[i];
                }
                weekScores.Add(total);
            }
            return weekScores;
        }

        // Places the replacement in the same position that the original was in to avoid label position issues. It is sorted into its own spot upon reload.
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

        // The id represents the team in the scheduling system referred to by the bowling league: 1 vs 6, 2 vs 4, etc.
        public int GetId()
        {
            return id;
        }

        public string GetName()
        {
            return name;
        }

        // Sorts by last name, places inactive bowlers out of reach of UI code.
        public void SortBowlers(int week)
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
    }
}
