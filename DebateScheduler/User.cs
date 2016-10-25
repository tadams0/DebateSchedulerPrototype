using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebateScheduler
{
    /// <summary>
    /// Defines a user object which can hold user data including the name and permission level.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The username of the user.
        /// </summary>
        public string Username { get; private set; }
        
        /// <summary>
        /// The permission level of the user.
        /// </summary>
        public int PermissionLevel { get; private set; }

        /// <summary>
        /// The id of the user within the database.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Creates a user object with the given parameters.
        /// </summary>
        /// <param name="permissionLevel">The level of access the user has.</param>
        /// <param name="username">The username of the user.</param>
        public User(int permissionLevel, string username, int id)
        {
            PermissionLevel = permissionLevel;
            Username = username;
            ID = id;
        }

        /// <summary>
        /// Creates a user object from a string representation of a previously created user object.
        /// </summary>
        /// <param name="userString">The ToString representation of a previous object.</param>
        public User(string userString)
        {
            string[] parameters = userString.Split('%');
            Username = parameters[0];
            PermissionLevel = int.Parse(parameters[1]);
            ID = int.Parse(parameters[2]);
        }

        /// <summary>
        /// Gets the string representation of the user object including the username and permission level.
        /// </summary>
        public override string ToString()
        {
            return Username + "%" + PermissionLevel + "%" + ID;
        }
    }
}