using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace DebateScheduler
{
    /// <summary>
    /// Defines basic actions to handle database information.
    /// </summary>
    public static class DatabaseHandler
    {
        /// <summary>
        /// The path where exceptions are logged.
        /// </summary>
        private static string exceptionFileName = "dump.txt";

        /// <summary>
        /// The folder that will be logged to.
        /// </summary>
        private static string logFolderPath = "Logs";

        /// <summary>
        /// The full path of where logs are written to in the file system. This is created dynamically and should instantiated as string.Empty if a specific path is not specifed.
        /// </summary>
        private static string fullLogPath = string.Empty; //"C:\\"

        /// <summary>
        /// The file type that the logs are saved as.
        /// </summary>
        private static string logFileType = ".txt";

        /// <summary>
        /// The name of the file where logs are stored.
        /// </summary>
        private static string logFileName = "logs";

        private static int permissionToAddUsers = 3;

        private static int permissionToRemoveUsers = 3;

        /// <summary>
        /// Gets a data table in a database.
        /// </summary>
        /// <param name="connectionString">The connection string used to direct the connection to a specific database.</param>
        /// <param name="table">The name of the table in the database to connect to and retrieve.</param>
        /// <param name="exceptionInfo">The info that will be logged in the event of an exception occuring.</param>
        /// <returns>Returns a data table from the connected database.</returns>
        private static DataTable GetDataTable(string connectionString, string table, string exceptionInfo = "")
        {
            DataTable resultingTable = null;

            SqlConnection connection = new SqlConnection(GetConnectionStringUsersTable());

            try
            {
                SqlCommand command = new SqlCommand("SELECT * FROM " + table, connection);
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    resultingTable = new DataTable();
                    adapter.Fill(resultingTable);
                }
            }
            catch (Exception e)
            {
                if (exceptionInfo == "")
                {
                    LogException(e, "exception occured while getting a database table.");
                }
                else
                {
                    LogException(e, exceptionInfo);
                }
            }
            finally
            {
                connection.Close();
            }

            return resultingTable;
        }

        /// <summary>
        /// Gets a data table in a database.
        /// </summary>
        /// <param name="connectionString">The connection string used to direct the connection to a specific database.</param>
        /// <param name="table">The name of the table in the database to connect to.</param>
        /// <param name="variable">The variable inside the table to scan for matches.</param>
        /// <param name="match">The variable which will be matched against the variable parameter, any matches will be returned in the resulting data table.</param>
        /// <param name="dataType">The data type of the match variable.</param>
        /// <param name="maxLength">The maximum length (size) of the variable.</param>
        /// <param name="exceptionInfo">The info that will be logged in the event of an exception occuring.</param>
        /// <returns>Returns a data table with the matched data from the connected database.</returns>
        private static DataTable GetDataTable(string connectionString, string table, string variable, string match, SqlDbType dataType, int maxLength, string exceptionInfo = "")
        {
            DataTable resultingTable = null;

            SqlConnection connection = new SqlConnection(GetConnectionStringUsersTable());

            try
            {
                SqlCommand command = new SqlCommand("SELECT * FROM " + table + " WHERE " + variable + " = @LookFor", connection);
                SqlParameter parameter = command.Parameters.Add("@LookFor", dataType, maxLength);
                parameter.Value = match;
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    resultingTable = new DataTable();
                    adapter.Fill(resultingTable);
                }
            }
            catch (Exception e)
            {
                if (exceptionInfo == "")
                {
                    LogException(e, "exception occured while getting a database table.");
                }
                else
                {
                    LogException(e, exceptionInfo);
                }
            }
            finally
            {
                connection.Close();
            }

            return resultingTable;
        }

        /// <summary>
        /// Attempts to execute a SQL query. The query is not sanitized in this method, so all sanitization must be done before passing the query in.
        /// </summary>
        /// <param name="connectionString">The database connection string where the query will be directed to.</param>
        /// <param name="query">The SQL command or query to execute. This should already be sanitized if necessary.</param>
        /// <param name="exceptionMessage">The exeception message to log in the event an exception occurs while executing the SQL.</param>
        /// <returns>Returns the SQL Data reader result from the query execution.</returns>
        private static SqlDataReader ExecuteSQL(string connectionString, string query, string exceptionMessage)
        {
            SqlDataReader reader = null;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                reader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                if (exceptionMessage == "")
                    LogException(e, "exception occured while executing SQL query.");
                else
                    LogException(e, exceptionMessage);
            }
            finally
            {
                connection.Close();
            }
            return reader;
        }

        /// <summary>
        /// Attempts to execute a SQL query. The query is not sanitized in this method, so all sanitization must be done before passing the query in.
        /// </summary>
        /// <param name="connectionString">The database connection string where the query will be directed to.</param>
        /// <param name="query">The SQL command or query to execute. This should already be sanitized if necessary.</param>
        /// <param name="exceptionMessage">The exeception message to log in the event an exception occurs while executing the SQL.</param>
        /// <param name="parameters">The parameters to add to the command once it is created.</param>
        /// <returns>Returns the SQL Data reader result from the query execution.</returns>
        private static SqlDataReader ExecuteSQL(string connectionString, string query, string exceptionMessage, params SqlParameter[] parameters)
        {
            SqlDataReader reader = null;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddRange(parameters);

                connection.Open();
                reader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                if (exceptionMessage == "")
                    LogException(e, "exception occured while executing SQL query.");
                else
                    LogException(e, exceptionMessage);
            }
            finally
            {
                connection.Close();
            }
            return reader;
        }

        /// <summary>
        /// Gets the connection string that can be used to connection to the user table within the database.
        /// </summary>
        /// <returns>Returns the connection string to the user table in the database.</returns>
        private static string GetConnectionStringUsersTable()
        {
            return ConfigurationManager.ConnectionStrings["UserTable"].ConnectionString;
        }

        /// <summary>
        /// Authenticates the username and password combo.
        /// </summary>
        /// <param name="username">The username, case sensitivity does not matter.</param>
        /// <param name="password">The password, case sensitive.</param>
        /// <returns>Returns null if the authentication failed. Returns a user object if the authentication suceeded.</returns>
        public static User AuthenticateUsernamePassword(string username, string password) //TODO: Send a session or something back.. I don't know, send data back adequete enough to do login.
        {
            User resultingUser = null;
            string realUsername = username.ToUpperInvariant(); //The username is converted to the upper invariant (upper case) to prevent case sensitivity on usernames.

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Users", "Name", realUsername, SqlDbType.NChar, 50, "exception occured while authenticating username/password.");

            if (table.Rows.Count > 0)
            {
                DataRow userRow = table.Rows[0]; //If there is more than 1 row, then we've got a problem because there's more than 1 matching username.
                string matchedName = userRow["Name"] as string;
                string matchedPassword = userRow["Password"] as string;
                int matchedPermissions = (int)userRow["Permissions"];
                int matchedID = (int)userRow["Id"];

                if (matchedName == realUsername && password == matchedPassword) //Compares the username and password to the username and password found in the database.
                {
                    //Log the user in, as the username and password match.
                    resultingUser = new User(matchedPermissions, username, matchedID);
                }
                else
                {
                    //Username/Password did not match send message.
                }

            }
            else
            {
                //The username does not exists.
            }

            return resultingUser;
        }

        /// <summary>
        /// Checks whether a username already exists in the database.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>Returns true if the username exists, false otherwise.</returns>
        public static bool UserExists(string username)
        {
            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Users", "Name", username.ToUpperInvariant(), SqlDbType.NChar, 50, "exception occured while checking if a user exists.");

            if (table.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a user to the database.
        /// </summary>
        /// <param name="session">The session, used to determine the user who is adding to the database. The session user must have high enough permissions or nothing will happen.</param>
        /// <param name="newUser">The new user being added to the database.</param>
        /// <param name="password">The plain text password of the user being added.</param>
        /// <returns>Returns true if the user was successfully added, false if the user already exists or an error occured.</returns>
        public static bool AddUser(HttpSessionState session, User newUser, string password, string email)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.
            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToAddUsers) //If the user exists and their permission level is super referee or greater...
            {
                string realUsername = newUser.Username.ToUpperInvariant(); //The username as it would appear in the database.
                string realEmail = email.ToUpperInvariant();

                if (!UserExists(realUsername)) //If the username does not exist.
                {
                    string sqlQuery = "INSERT INTO Users (Name, Password, Permissions, Email) VALUES " + //Id, 
                        "('" + realUsername + "', '" + password + "', '" + newUser.PermissionLevel + "', " + realEmail + "')"; //'" + newUser.ID + "', 

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a user.");
                    if (result != null) //If the result is not null, then the query succeeded and should be logged.
                    {
                        Log(currentSessionUser, currentSessionUser.Username + " added a new user named " + newUser.Username + ".");
                        return true;
                    }
                }
            }
            else
            {
                //There is no user logged in or the permission level is too low.
            }
            
            return false;
        }

        /// <summary>
        /// Adds a user to the database.
        /// </summary>
        /// <param name="session">The session, used to determine the user who is adding to the database. The session user must have high enough permissions or nothing will happen.</param>
        /// <param name="newUser">The new user being added to the database.</param>
        /// <param name="password">The plain text password of the user being added.</param>
        /// <returns>Returns true if the user was successfully added, false if the user already exists or an error occured.</returns>
        public static bool AddUser(string ipAddress, User newUser, string password, string email)
        {
            if (ipAddress != "") //If the ip address is not blank.
            {
                string realUsername = newUser.Username.ToUpperInvariant(); //The username as it would appear in the database.
                string realEmail = email.ToUpperInvariant();

                if (!UserExists(realUsername)) //If the username does not exist.
                {
                    string sqlQuery = "INSERT INTO Users (Name, Password, Permissions, Email) VALUES " + //Id, 
                        "('" + realUsername + "', '" + password + "', '" + newUser.PermissionLevel + "', " + realEmail + "')"; //'" + newUser.ID + "', 

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a user.");
                    if (result != null) //If the result is not null, then the query succeeded and should be logged.
                    {
                        Log(ipAddress, ipAddress + " (IP) added a new user named " + newUser.Username + ".");
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes a user with the matching username from the database.
        /// </summary>
        /// <param name="session">The session, used to determine who is removing from the database. The session user must have high enough permissions or the command will be denied.</param>
        /// <param name="username">The username of the user to remove, this is not case sensitive.</param>
        /// <returns>Returns true if the user was properly removed, false otherwise.</returns>
        public static bool RemoveUser(HttpSessionState session, string username)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.
            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToRemoveUsers) //If the user exists and their permission level is super referee or greater...
            {
                string realUsername = username.ToUpperInvariant(); //The username as it would appear in the database.

                if (UserExists(realUsername)) //If the username does not exist.
                {
                    string sqlQuery = "DELETE FROM Users WHERE Name = @Value";
                    SqlParameter parameter = new SqlParameter("@Value", SqlDbType.NChar, 50);
                    parameter.Value = username;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while removing a user.", parameter);
                    if (result != null) //If the result is not null, then the query succeeded and should be logged.
                    {
                        Log(currentSessionUser, currentSessionUser.Username + " removed a user named " + username + ".");
                        return true;
                    }
                }
            }
            else
            {
                //There is no user logged in or the permission level is too low.
            }

            return false;
        }

        /// <summary>
        /// Gets a list of all the teams in the database.
        /// </summary>
        /// <returns>Returns a list populated with all the teams in the database.</returns>
        public static List<Team> GetTeams()
        {
            List<Team> teams = new List<Team>();

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Teams", "exception occured while gathering the team table.");

            foreach (DataRow row in table.Rows) //Iterates through each row retrieved.
            {
                int id = (int)row["Id"];
                string teamName = row["Name"] as string;
                int wins = (int)row["Wins"];
                int losses = (int)row["Losses"];
                int ties = (int)row["Ties"];
                int totalScore = (int)row["TotalScore"];
                teams.Add(new Team(teamName, id, wins, losses, ties, totalScore)); //Creates a new team object for the team list.
            }

            return teams;
        }

        /// <summary>
        /// Gets the path to the App_Data folder which contains data for the application.
        /// </summary>
        /// <returns></returns>
        public static string GetAppDataPath()
        {
            return HttpContext.Current.Server.MapPath("~/App_Data/");
        }


        /// <summary>
        /// Creates the log path that the log files are saved to.
        /// </summary>
        private static void CreateLogPath()
        {
            string assemblyPath = GetAppDataPath();
            fullLogPath = assemblyPath += "\\" + logFolderPath + "\\";
        }

        /// <summary>
        /// Logs a message in the log file.
        /// </summary>
        /// <param name="user">The name of the user which will be logged.</param>
        /// <param name="message">The message to log.</param>
        private static void Log(string user, string message)
        {
            if (fullLogPath == string.Empty) //there is no full path, so one must be constructed.
            {
                CreateLogPath();
            }

            string logMessage = "Username: " + user + " Message: \"" + message + "\" Logged at " + DateTime.Now + " server time.";

            try
            {
                Directory.CreateDirectory(fullLogPath); //Creates the log directory if it does not exist, otherwise does nothing.

                using (StreamWriter writer = new StreamWriter(fullLogPath + logFileName + logFileType, true))
                {
                    writer.WriteLine(logMessage); //Logs the message to the text file.
                }
            }
            catch (Exception e)
            {
                LogException(e, "exception occured while logging."); //An error has occured and should be logged.
            }

        }

        /// <summary>
        /// Logs a message in the log file.
        /// </summary>
        /// <param name="user">The user whose username will be logged with the message.</param>
        /// <param name="message">The message to log.</param>
        private static void Log(User user, string message)
        {
            Log(user.Username, message);
        }

        /// <summary>
        /// Logs exceptions to the log folder.
        /// </summary>
        /// <param name="e">The exception whose message will be logged.</param>
        /// <param name="info">Additional information to log along with the exception, it is generally a good idea to include where the exception occured. IE: logging in, editing data, etc...</param>
        private static void LogException(Exception e, string info)
        {
            try
            {
                Directory.CreateDirectory(fullLogPath);

                using (StreamWriter writer = new StreamWriter(exceptionFileName + exceptionFileName, true))
                {
                    writer.WriteLine("Exception with message  \"" + e.Message + "\" was found. Additional Info: \"" + info + "\" logged at " + DateTime.Now); //Logs the message to the exception file.
                }
            }
            catch //(Exception exception)
            {
                //Well an exception occured, but it cannot be logged since this is the method for logging it. In this case we do nothing really.
            }

        }
        

    }
}