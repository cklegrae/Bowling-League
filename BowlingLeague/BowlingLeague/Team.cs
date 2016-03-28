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
        List<Bowler> bowlers;
        string name;
        int id;

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

        // Places the replacement in the same position that the original was in to avoid label position issues. It is sorted into its own spot upon reload.
        public bool ReplaceBowler(Bowler toBeRemoved, Bowler replacement)
        {
            int index = bowlers.IndexOf(toBeRemoved);
            if (index < 0)
                return false;
            bowlers[index] = replacement;
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

        // Sorts by last name.
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
    }
}
