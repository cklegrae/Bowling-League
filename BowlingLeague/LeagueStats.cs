using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingLeague
{
    /// <summary> Calculates the necessary statistics for administration printouts. </summary>
    class LeagueStats
    {
        private List<Bowler> bowlers;
        private List<Team> teams;
        int week;

        public LeagueStats(int week)
        {
            bowlers = new List<Bowler>(League.bowlers);
            foreach(Bowler bowler in bowlers)
                bowler.UpdateStats(week);
            teams = new List<Team>(League.teams);
            this.week = week;
        }

        /// <summary> Gets bowlers list ranked by averages. </summary>
        public List<Bowler> GetRankedBowlers()
        {
            return bowlers.OrderBy(b => -b.GetMean(week, true)).ToList();
        }

        /// <summary> Gets teams list ranked by wins. </summary>
        public List<Team> GetRankedTeams()
        {
            return teams.OrderBy(t => -t.wins).ToList();
        }

        /// <summary> Gets bowlers list ranked by highest single of the week. </summary>
        public List<Bowler> GetWeekHighBowlers()
        {
            return bowlers.OrderBy(b => -b.GetScores(week).Max()).ToList();
        }

        /// <summary> Gets teams list ranked by highest week single or triple according to bool parameter. </summary>
        public List<Team> GetHighTeams(bool getTriple)
        {
            return teams.OrderBy(t => -t.GetHighScore(getTriple)).ToList();
        }

        /// <summary> Gets bowlers list ranked by highest week single or triple according to bool parameter. </summary>
        public List<Bowler> GetHighBowlers(bool getTriple)
        {
            return bowlers.OrderBy(b => -b.GetHighScore(getTriple)).ToList();
        }

    }
}
