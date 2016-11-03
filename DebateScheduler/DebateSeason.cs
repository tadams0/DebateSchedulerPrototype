using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DebateScheduler
{
    public class DebateSeason
    {

        /// <summary>
        /// Parses and returns a list of team ids in the string given..
        /// </summary>
        /// <param name="teamString">The string to parse into teams.</param>
        /// <returns>Returns a list of ids corresponding to teams in the team database.</returns>
        public static List<int> ParseTeamString(string teamString)
        {
            List<int> teamIDs = new List<int>();
            string[] ids = teamString.Split('|');
            foreach (string s in ids)
            {
                int id = int.Parse(s);
                teamIDs.Add(id);
            }
            return teamIDs;
        }

        /// <summary>
        /// Parses and returns a list of debate ids in the string given.
        /// </summary>
        /// <param name="debateString">The string containing debate ids seperated by a | character.</param>
        /// <returns>Returns a list of ids corresponding to debates in the debate database.</returns>
        public static List<int> ParseDebateString(string debateString)
        {
            List<int> debateIDs = new List<int>();
            string[] ids = debateString.Split('|');
            foreach (string s in ids)
            {
                int id = int.Parse(s);
                debateIDs.Add(id);
            }

            return debateIDs;
        }

        /// <summary>
        /// The ID of the debate season.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// The list of teams associated with this debate season.
        /// </summary>
        public List<Team> Teams
        {
            get { return teams; }
            set { teams = value; }
        }

        /// <summary>
        /// A list of debates associated with this debate season.
        /// </summary>
        public List<Debate> Debates
        {
            get { return debates; }
            set { debates = value; }
        }

        private List<Team> teams = new List<Team>();
        private List<Debate> debates = new List<Debate>();

        public DebateSeason(int id, List<Team> teams, List<Debate> debates)
        {
            ID = id;
            this.teams = teams;
            this.debates = debates;
        }

        /// <summary>
        /// Builds and returns a string representing team ids.
        /// </summary>
        /// <returns>Returns a string that represents the ids associated with this debate season.</returns>
        public string GetTeamString()
        {
            StringBuilder builder = new StringBuilder(teams.Count * 2);
            foreach (Team t in teams)
            {
                builder.Append(t.ID);
                builder.Append("|");
            }
            return builder.ToString();
        }
        
        public string GetDebateString()
        {
            StringBuilder builder = new StringBuilder(debates.Count * 2);
            foreach (Debate d in debates)
            {
                builder.Append(d.ID);
                builder.Append("|");
            }
            return builder.ToString();
        }
        
        /// <summary>
        /// Replaces a team within the debate season by name.
        /// </summary>
        /// <param name="oldTeamName">The name of the team to replace.</param>
        /// <param name="newTeam">The new team that will take its place.</param>
        /// <returns>Returns true if the team was replaced, false otherwise.</returns>
        public bool ReplaceTeam(string oldTeamName, Team newTeam)
        {
            string upperName = oldTeamName.ToUpperInvariant();
            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i].Name.ToUpperInvariant() == upperName)
                {
                    teams[i] = newTeam;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Replaces a team within the debate season by id.
        /// </summary>
        /// <param name="id">The id of the team to replace.</param>
        /// <param name="newTeam">The new team that will take its place.</param>
        /// <returns>Returns true if the team was replaced, false otherwise.</returns>
        public bool ReplaceTeam(int id, Team newTeam)
        {
            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i].ID == id)
                {
                    teams[i] = newTeam;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///Replaces a debate within the debate season.
        /// </summary>
        /// <param name="id">The id of the debate to replace.</param>
        /// <param name="newDebate">The new debate to replace with.</param>
        /// <returns>Returns true if the debate was replaced. False otherwise.</returns>
        public bool ReplaceDebate(int id, Debate newDebate)
        {
            for (int i = 0; i < debates.Count; i++)
            {
                if (debates[i].ID == id)
                {
                    debates[i] = newDebate;
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return " { ID: " + ID + " }";
        }

    }
}