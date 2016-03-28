using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingLeague
{
    // Matchup represents a weekly matchup between two teams. It delegates wins, losses, and ties to its teams.
    [Serializable()]
    class Matchup
    {
        int week;
        Team teamOne;
        Team teamTwo;

        public Matchup(Team teamOne, Team teamTwo, int currentWeek)
        {
            this.teamOne = teamOne;
            this.teamTwo = teamTwo;
            week = currentWeek;
        }

        public Team GetTeamOne()
        {
            return teamOne;
        }

        public Team GetTeamTWo()
        {
            return teamTwo;
        }

        public int GetWeek()
        {
            return week;
        }

    }
}
