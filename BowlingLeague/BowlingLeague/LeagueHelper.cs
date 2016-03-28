using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingLeague
{
    [Serializable()]
    class LeagueHelper
    {
        public List<Team> t;
        public List<Matchup> m;

        public LeagueHelper()
        {

        }

        public void SetTeams(List<Team> list)
        {
            t = list;
        }

        public void SetMatchups(List<Matchup> list)
        {
            m = list;
        }

        public List<Team> GetTeams()
        {
            return t;
        }

        public List<Matchup> GetMatchups()
        {
            return m;
        }
    }
}
