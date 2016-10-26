using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebateScheduler
{
    /// <summary>
    /// Defines a team object which contains a name, id, and stats about the team.
    /// </summary>
    public class Team
    {
        /// <summary>
        /// The name of the team.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// The number of wins the team has.
        /// </summary>
        public int Wins
        {
            get { return wins; }
            set { wins = value; }
        }

        /// <summary>
        /// the number of losses the team has.
        /// </summary>
        public int Losses
        {
            get { return losses; }
            set { losses = value; }
        }

        /// <summary>
        /// The number of ties the win has.
        /// </summary>
        public int Ties
        {
            get { return ties; }
            set { ties = value; }
        }

        /// <summary>
        /// The total cumulative score of the team.
        /// </summary>
        public int TotalScore
        {
            get { return totalScore; }
            set { totalScore = value; }
        }

        /// <summary>
        /// The unique ID (non-GUID based) which can be matched in the data base.
        /// </summary>
        public int ID { get { return id; } }

        
        private string name;
        private int id;
        private int wins;
        private int losses;
        private int ties;
        private int totalScore;

        public Team(string name, int id, int wins, int losses, int ties, int totalScore)
        {
            this.name = name;
            this.id = id;
            this.wins = wins;
            this.losses = losses;
            this.ties = ties;
            this.totalScore = totalScore;
        }

        /// <summary>
        /// Gets the string representation of the team object.
        /// </summary>
        /// <returns>Returns a string representation of the team object including all the stats it contains. It excludes the ID.</returns>
        public override string ToString()
        {
            return "{ Name: " + name + ", Wins: " + wins + ", Losses: " + losses + ", Ties: " + ties + ", Total Score: " + totalScore + " }";
        }

    }
}