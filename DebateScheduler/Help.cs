using System;
using System.Collections.Generic;
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
                default: return "Permissionless :(";
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

    }
}