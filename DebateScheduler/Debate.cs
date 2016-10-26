using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebateScheduler
{
    /// <summary>
    /// Defines an object that holds information reguarding debate times, and teams involved.
    /// </summary>
    public class Debate
    {

        /// <summary>
        /// The ID of the debate. Used to identify the location of the debate in the database.
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// The first team in the debate.
        /// </summary>
        public Team Team1
        {
            get { return team1; }
            set { team1 = value; }
        }

        /// <summary>
        /// the second team in the debate.
        /// </summary>
        public Team Team2
        {
            get { return team2; }
            set { team2 = value; }
        }

        /// <summary>
        /// The score the first team won in the debate. -1 represents no score set yet.
        /// </summary>
        public int Team1Score
        {
            get { return team1Score; }
            set { team1Score = value; }
        }

        /// <summary>
        /// The score the second team won in the debate. -1 represents no score set yet.
        /// </summary>
        public int Team2Score
        {
            get { return team2Score; }
            set { team2Score = value; }
        }
        
        /// <summary>
        /// The date the debate takes place on.
        /// </summary>
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        /// <summary>
        /// Determines whether the debate takes place in the morning (true) or the afternoon (false).
        /// </summary>
        public bool MorningDebate
        {
            get { return morningDate; }
            set { morningDate = value; }
        }
        
        private int id;
        private Team team1;
        private Team team2;
        private int team1Score;
        private int team2Score;
        private DateTime date;
        private bool morningDate; //If true then the debate takes place in the morning. If false it takes place in the afternoon.

        /// <summary>
        /// Instantiates a debate object with information of a generic debate.
        /// </summary>
        /// <param name="id">The id of the debate within the database, this does not matter if the debate is going to be added as a new debate to the database since it will be auto assigned.</param>
        /// <param name="teamID1">The id in the database of the first team in the debate.</param>
        /// <param name="teamID2">The id in the database of the second team in the debate.</param>
        /// <param name="team1Score">The score the first team currently has in the debate. Use -1 if no score has been set yet.</param>
        /// <param name="team2Score">The score the second team currently has in the debate. Use -1 if no score has been set yet.</param>
        /// <param name="date">The date the debate will take place on.</param>
        /// <param name="morningDate">If true the debate will take place during the morning. If false it will take place during the afternoon.</param>
        public Debate(int id, Team team1, Team team2, int team1Score, int team2Score, DateTime date, bool morningDate)
        {
            this.id = id;
            this.team1 = team1;
            this.team2 = team2;
            this.team1Score = team1Score;
            this.team2Score = team2Score;
            this.date = date;
            this.morningDate = morningDate;
        }

        public override string ToString()
        {
            string morningAddition = "takes place in the ";
            if (morningDate)
                morningAddition += "morning";
            else
                morningAddition += "afternoon";
            return "{ Team 1 ID: " + team1.ID + ", Team 1 Score: " + team1Score + ", Team 2 ID: " + team2.ID + ", Team 2 Score: " + team2Score + ", On Date: " + date.ToString() + " and " + morningAddition + " }";
        }

    }
}