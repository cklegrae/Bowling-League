﻿using System;
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
        List<Team> teams;

        public Matchup(Team teamOne, Team teamTwo, int currentWeek)
        {
            teams = new List<Team>();
            teams.Add(teamOne);
            teams.Add(teamTwo);
            week = currentWeek;
        }

        // Computes the matchup.
        public void Compute()
        {
            int[] handicaps = DetermineHandicap();
            List<int> teamOneScores = teams[0].GetTeamScores(week);
            List<int> teamTwoScores = teams[1].GetTeamScores(week);
            int teamOneTotal = 0;
            int teamTwoTotal = 0;

            for (int i = 0; i < 4; i++)
            {
                // Adds 'pinfall' game: the fourth win/loss given out is a comparison between the two team's week totals.
                if(i == 3)
                {
                    teamOneScores.Add(teamOneTotal);
                    teamTwoScores.Add(teamTwoTotal);
                }

                // Accounts for handicap. The team without a handicap will have handicap[x] be 0.
                teamOneScores[i] += handicaps[0];
                teamTwoScores[i] += handicaps[1];

                if(teamOneScores[i] > teamTwoScores[i])
                {
                    DelegateResult(0, 1);
                    DelegateResult(1, -1);
                }else if(teamOneScores[i] < teamTwoScores[i])
                {
                    DelegateResult(0, -1);
                    DelegateResult(1, 1);
                }
                else
                {
                    DelegateResult(0, 0.5);
                    DelegateResult(1, 0.5);
                }

                teamOneTotal += teamOneScores[i];
                teamTwoTotal += teamTwoScores[i];
            }
        }

        // Give out wins or losses. 1 = victory, -1 = loss, 0.5 indicates a tie, where a team earns half a victory and half a loss.
        public void DelegateResult(int index, double result)
        {
            if (result == 0.5)
            {
                teams[index].wins += result;
                teams[index].losses += result;
            }else if(result > 0)
            {
                teams[index].wins++;
            }
            else
            {
                teams[index].losses++;
            }
        }

        // Handicap rules in this particular league: 2/3rds of the difference between team averages, rounded down.
        public int[] DetermineHandicap()
        {
            int[] handicaps = new int[2];
            double teamOneAverage = teams[0].GetTeamAverage(week);
            double teamTwoAverage = teams[1].GetTeamAverage(week);
            if (teamOneAverage < teamTwoAverage)
                handicaps[0] = (int)Math.Floor((teamTwoAverage - teamOneAverage) * (2 / 3));
            else
                handicaps[1] = (int)Math.Floor((teamOneAverage - teamTwoAverage) * (2 / 3));
            return handicaps;
        }

    }
}
