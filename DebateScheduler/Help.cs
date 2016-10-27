using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace DebateScheduler
{
    /// <summary>
    /// Defines many helpful methods used to interact with the session and other areas of the application.
    /// </summary>
    public static class Help
    {
        private static readonly string DateFormat = "O";

        /// <summary>
        /// Gets the user object from the current session.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <returns>Returns a user object representative of the current user session. This will be null if there is no session.</returns>
        public static User GetUserSession(HttpSessionState session)
        {
            return session["User"] as User;
        }

        /// <summary>
        /// Adds a user object to the current session/
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="user">The user object the session will hold onto.</param>
        public static void AddUserSession(HttpSessionState session, User user)
        {
            session.Add("User", user);
        }

        /// <summary>
        /// Ends the current session.
        /// </summary>
        /// <param name="session">The session to clear.</param>
        public static void EndSession(HttpSessionState session)
        {
            session.RemoveAll();
        }

        /// <summary>
        /// Gets the name of the permission level based on the int value.
        /// </summary>
        /// <param name="permissionLevel">The permission level whose name will be returned.</param>
        /// <returns>Returns a string representation of the given permission level.</returns>
        public static string GetPermissionName(int permissionLevel)
        {
            switch (permissionLevel)
            {
                default: return "Regular User";
                case 2: return "Referee";
                case 3: return "Super Referee";
            }
        }

        /// <summary>
        /// Gets the int value of a permission level based on the name.
        /// </summary>
        /// <param name="permissionName">The name of the permission level.</param>
        /// <returns>Returns a number that represents the level of permissions the given name has.</returns>
        public static int GetPermissionLevel(string permissionName)
        {
            switch (permissionName)
            {
                default: return 0;
                case "Referee": return 2;
                case "Super Referee": return 3;
            }
        }

        /// <summary>
        /// Gets a string representation of a given date in a specific format type.
        /// </summary>
        /// <param name="date">The date to turn into a string.</param>
        /// <returns>Returns a string representation of the date.</returns>
        public static string GetDateString(DateTime date)
        {
            return date.ToString(DateFormat);
        }

        /// <summary>
        /// Gets a date from a given string.
        /// </summary>
        /// <param name="date">The string representation of the date.</param>
        /// <returns>Returns a date time parsed back from a string.</returns>
        public static DateTime GetDate(string date)
        {
            return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// creates every match for the season.
        /// </summary>
        /// <param name="teamList">The list of teams in a season.</param>
        /// <returns>Returns a list of debates for the season.</returns>
        public static List<Debate> MatchMake(List<Team> teamList)
        {
            List<Debate> MatchList = new List<Debate>();

            if (teamList.Count <= 1)
            {
                MatchList.Add(new Debate(0, new Team("Error: Not enough teams", 0, 0, 0, 0, 0), new Team("", 1, 0, 0, 0, 0), 0, 0, DateTime.Now, true));
                
            }
            for (int i = 0; i < teamList.Count - 1; i++)
            {
                bool alreadyDebated = false;
                for (int j = 1; j < teamList.Count; j++)
                {
                    DateTime matchDate = DateTime.Today.AddDays(7 * j - 7);
                    if (!alreadyDebated)
                    {
                        MatchList.Add(new Debate(0, teamList[0], teamList[j], 0, 0, matchDate, true));
                        alreadyDebated = true;
                    }
                    else
                    {
                        MatchList.Add(new Debate(0, teamList[0], teamList[j], 0, 0, matchDate, false));
                        alreadyDebated = false;
                    }
                }
                teamList.RemoveAt(0);
            }
            return (MatchList);
        }
    }
}