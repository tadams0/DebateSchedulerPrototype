using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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

        private static int permissionToAddTeams = 3;
        private static int permissionToUpdateTeams = 3;
        private static int permissionToRemoveTeams = 3;

        private static int permissionToAddDebates = 3;
        private static int permissionToUpdateDebates = 3;
        private static int permissionToRemoveDebates = 3;
        private static int permissionToClearDebates = 3;

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
        /// Attempts to execute a SQL query. The query is not sanitized in this method, so all sanitization must be done before passing the query in or by utilizing the parameters.
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
        /// Attempts to execute an SQL query, and stops after finding a single row. Sanitization should be done by utilizing the parameters field.
        /// </summary>
        /// <param name="connectionString">The database connection string where the query will be directed to.</param>
        /// <param name="query">The SQL command or query to execute. This should already be sanitized if necessary, such as including the parameter names already.</param>
        /// <param name="exceptionMessage">The exeception message to log in the event an exception occurs while executing the SQL.</param>
        /// <param name="parameters">The parameters to add to the command once it is created.</param>
        /// <returns>Returns first column of the first row found.</returns>
        private static object ExecuteSQLScaler(string connectionString, string query, string exceptionMessage, params SqlParameter[] parameters)
        {
            object result = null;

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddRange(parameters);

                connection.Open();
                result = command.ExecuteScalar();
            }
            catch (Exception e)
            {
                if (exceptionMessage == "")
                    LogException(e, "exception occured while executing scaler SQL query.");
                else
                    LogException(e, exceptionMessage);
            }
            finally
            {
                connection.Close();
            }

            return result;
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
                    string sqlQuery = "INSERT INTO Users (Name, Password, Permissions, Email) VALUES " +
                        "(@Username, @Password, '" + newUser.PermissionLevel + "', @Email)"; //TODO: Sanitize the username.

                    SqlParameter username = new SqlParameter("@Username", SqlDbType.NChar, newUser.Username.Length);
                    username.Value = realUsername;
                    SqlParameter passwordParameter = new SqlParameter("@Password", SqlDbType.NChar, password.Length);
                    passwordParameter.Value = password;
                    SqlParameter emailParameter = new SqlParameter("@Email", SqlDbType.NChar, realEmail.Length);
                    emailParameter.Value = realEmail;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a user.",
                        username, passwordParameter, emailParameter);
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
        /// <returns>Returns a list populated with all the teams in the database. Returns an empty list if there are no teams in the database.</returns>
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
        /// Gets a team from the database based on their ID.
        /// </summary>
        /// <param name="id">The ID of the team.</param>
        /// <returns>Returns a team object which contains all the data of the matching team in the database. This will be null if there is no matching team.</returns>
        public static Team GetTeam(int id)
        {
            Team newTeam = null;

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Teams", "Id", id.ToString(), SqlDbType.Int, int.MaxValue, "exception occured while gathering a single team by ID.");

            if (table.Rows.Count > 0)
            {
                int matchedID = (int)table.Rows[0]["Id"];
                string teamName = table.Rows[0]["Name"] as string;
                int wins = (int)table.Rows[0]["Wins"];
                int losses = (int)table.Rows[0]["Losses"];
                int ties = (int)table.Rows[0]["Ties"];
                int totalScore = (int)table.Rows[0]["TotalScore"];
                newTeam = new Team(teamName, matchedID, wins, losses, ties, totalScore); //The resulting team object.
            }

            return newTeam;
        }

        /// <summary>
        /// Gets a team from the database based on their name.
        /// </summary>
        /// <param name="name">The name of the team to search for.</param>
        /// <returns>Returns a team object which contains all the data of the matching team in the database. This will be null if there is no matching team.</returns>
        public static Team GetTeam(string name)
        {
            Team newTeam = null;

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Teams", "Name", name, SqlDbType.NChar, 50, "exception occured while gathering a single team by name.");

            if (table.Rows.Count > 0)
            {
                int id = (int)table.Rows[0]["Id"];
                string teamName = table.Rows[0]["Name"] as string;
                int wins = (int)table.Rows[0]["Wins"];
                int losses = (int)table.Rows[0]["Losses"];
                int ties = (int)table.Rows[0]["Ties"];
                int totalScore = (int)table.Rows[0]["TotalScore"];
                newTeam = new Team(teamName, id, wins, losses, ties, totalScore); //The resulting team object.
            }

            return newTeam;
        }

        /// <summary>
        /// Adds the given team to the database.
        /// </summary>
        /// <param name="session">The session of the user who is adding the team.</param>
        /// <param name="newTeam">The new team that is being added. ID is ignored.</param>
        /// <returns>Returns true if the team was successfully added to the database, false if it was not added.</returns>
        public static bool AddTeam(HttpSessionState session, Team newTeam)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.

            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToAddTeams && newTeam != null) //If the given team is not null and proper permissions are matched.
            {
                Team matchedTeam = GetTeam(newTeam.Name);

                if (matchedTeam == null) //If there is no team that matches names.
                {
                    string sqlQuery = "INSERT INTO Teams (Name, Wins, Losses, Ties, TotalScore) VALUES " + //Id, 
                        "(@TeamName, '" + newTeam.Wins + "', '" + newTeam.Losses + "', '" + newTeam.Ties + "', '" + newTeam.TotalScore + "')"; //'" + newUser.ID + "', 
                    SqlParameter teamName = new SqlParameter("@TeamName", SqlDbType.NChar, newTeam.Name.Length);
                    teamName.Value = newTeam.Name;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a team.", teamName);
                    if (result != null) //If the result is not null, then the query succeeded and should be logged.
                    {
                        Log(currentSessionUser.Username, currentSessionUser.Username + " added a new team with parameters " + newTeam.ToString() + ".");
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the team with a matching id as the given team, and replaces all data of the database copy with the given copy.
        /// </summary>
        /// <param name="session">The session that is updating the team.</param>
        /// <param name="team">The new team data that the database will hold.</param>
        /// <returns>Returns true if the team was successfully updated, false if an error occured or permissions were not high enough.</returns>
        public static bool UpdateTeam(HttpSessionState session, Team team)
        {
            User updatingUser = Help.GetUserSession(session);
            if (updatingUser != null && updatingUser.PermissionLevel >= permissionToUpdateTeams) //If the user's permission level is high enough
            {
                Team previousTeamData = GetTeam(team.ID); //We get the team data that currently exists in the database...
                if (previousTeamData != null) //We ensure that the data exists, otherwise we cannot update something that doesn't exist.
                {
                    string sqlQuery = "UPDATE Teams SET " +
                                "Name = @Name, Wins = @Wins, Losses = @Losses, Ties = @Ties, TotalScore = @TotalScore" +
                                " WHERE Id = " + team.ID;
                    //ID is omitted because changing it will result in incorrect foreign keys in the debates table.

                    //Generating the parameters, this is done for sanitization reasons.
                    SqlParameter name = new SqlParameter("@Name", SqlDbType.NChar, team.Name.Length); //It is important the size is the size of the string and no the max limit.
                    name.Value = team.Name;
                    SqlParameter wins = new SqlParameter("@Wins", SqlDbType.Int);
                    wins.Value = team.Wins;
                    SqlParameter losses = new SqlParameter("@Losses", SqlDbType.Int);
                    losses.Value = team.Losses;
                    SqlParameter ties = new SqlParameter("@Ties", SqlDbType.Int);
                    ties.Value = team.Ties;
                    SqlParameter totalScore = new SqlParameter("@TotalScore", SqlDbType.Int);
                    totalScore.Value = team.TotalScore;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while updating a team.",
                        name, wins, losses, ties, totalScore);

                    if (result != null)
                    {
                        Log(updatingUser.Username,
                            updatingUser.Username + " updated a team from " + previousTeamData.ToString() + " to " + team.ToString());

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all data of the team with the matching id of the given id.
        /// </summary>
        /// <param name="session">The current session, used to determine the user performing this action.</param>
        /// <param name="id">The id of the team to remove.</param>
        /// <returns>Returns true if the team was properly removed, false if it was not.</returns>
        public static bool RemoveTeam(HttpSessionState session, int id)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.
            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToRemoveTeams) //If the user exists and their permission level is super referee or greater...
            {
                Team team = GetTeam(id); //We get the team if it exists in the database (there are probably faster ways, but this works well enough).
                if (team != null) //We ensure that the team exists.
                {
                    string sqlQuery = "DELETE FROM Teams WHERE Id = @Value";
                    SqlParameter parameter = new SqlParameter("@Value", SqlDbType.Int);
                    parameter.Value = id;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while removing a team by id.", parameter);

                    if (result != null) //If the result is not null, then the query succeeded and should be logged.
                    {
                        Log(currentSessionUser, currentSessionUser.Username + " removed a team with the data of " + team.ToString());
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all data of the team with the matching name of the given name.
        /// </summary>
        /// <param name="session">The current session, used to determine the user performing this action.</param>
        /// <param name="teamName">The team name of the team to remove.</param>
        /// <returns>Returns true if the team was properly removed, false if it was not.</returns>
        public static bool RemoveTeam(HttpSessionState session, string teamName)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.
            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToRemoveTeams) //If the user exists and their permission level is super referee or greater...
            {
                Team team = GetTeam(teamName); //We get the team if it exists in the database (there are probably faster ways, but this works well enough).
                if (team != null) //We ensure that the team exists.
                {
                    string sqlQuery = "DELETE FROM Teams WHERE Name = @Value";
                    SqlParameter parameter = new SqlParameter("@Value", SqlDbType.NChar, teamName.Length);
                    parameter.Value = teamName;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while removing a team by name.", parameter);

                    if (result != null) //If the result is not null, then the query succeeded and should be logged.
                    {
                        Log(currentSessionUser, currentSessionUser.Username + " removed a team with the data of " + team.ToString());
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the list of debates in the database.
        /// </summary>
        /// <returns>Returns a list of debate objects stored in the database.</returns>
        public static List<Debate> GetDebates()
        {
            List<Debate> debates = new List<Debate>();

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Debates", "exception occured while gathering the debate table.");

            foreach (DataRow row in table.Rows) //Iterates through each row retrieved.
            {
                int matchedID = (int)row["Id"];
                int team1ID = (int)row["TID1"];
                int team2ID = (int)row["TID2"];
                int team1Score = (int)row["T1Score"];
                int team2Score = (int)row["T2Score"];
                string dateString = row["Date"] as string;
                DateTime date = Help.GetDate(dateString);
                bool morningDebate = Convert.ToBoolean(row["MorningDebate"]);

                Team team1 = GetTeam(team1ID); //Gets the first team in the debate.
                Team team2 = GetTeam(team2ID); //Gets the second team in the debate.

                debates.Add(new Debate(matchedID, team1, team2, team1Score, team2Score, date, morningDebate));

            }

            return debates;
        }

        /// <summary>
        /// Gets a debate from the database based on its ID.
        /// </summary>
        /// <param name="id">The ID of the debate.</param>
        /// <returns>Returns a debate object which contains all the data of the matching debate in the database. This will be null if there is no matching debate.</returns>
        public static Debate GetDebate(int id)
        {
            Debate resultingDebate = null;

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Debates", "Id", id.ToString(), SqlDbType.Int, int.MaxValue, "exception occured while gathering a single debate by ID.");

            if (table.Rows.Count > 0)
            {
                int matchedID = (int)table.Rows[0]["Id"];
                int team1ID = (int)table.Rows[0]["TID1"];
                int team2ID = (int)table.Rows[0]["TID2"];
                int team1Score = (int)table.Rows[0]["T1Score"];
                int team2Score = (int)table.Rows[0]["T2Score"];
                string dateString = table.Rows[0]["Date"] as string;
                DateTime date = Help.GetDate(dateString);
                bool morningDebate = Convert.ToBoolean(table.Rows[0]["MorningDebate"]);

                Team team1 = GetTeam(team1ID); //Gets the first team in the debate.
                Team team2 = GetTeam(team2ID); //Gets the second team in the debate.

                resultingDebate = new Debate(matchedID, team1, team2, team1Score, team2Score, date, morningDebate);

            }

            return resultingDebate;
        }

        /// <summary>
        /// Adds a new debate to the database.
        /// </summary>
        /// <param name="session">The session that is adding the debate.</param>
        /// <param name="newDebate">The new debate that is being added.</param>
        /// <returns>Returns true if the debate was successfully added, false if it was not added.</returns>
        public static bool AddDebate(HttpSessionState session, Debate newDebate)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.

            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToAddDebates && newDebate != null) //If the given debate is not null and proper permissions are matched.
            {
                string dateString = Help.GetDateString(newDebate.Date);

                int bitValue = 0;
                if (newDebate.MorningDebate)
                    bitValue = 1;

                string sqlQuery = "INSERT INTO Debates (TID1, TID2, T1Score, T2Score, Date, MorningDebate) VALUES " + 
                        "('" + newDebate.Team1.ID + "', '" + newDebate.Team2.ID + "', '" + newDebate.Team1Score + "', '" + newDebate.Team2Score + "', '" + dateString + "', '" + bitValue + "')";
                
                SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a debate.");
                if (result != null) //If the result is not null, then the query succeeded and should be logged.
                {
                    Log(currentSessionUser.Username, currentSessionUser.Username + " added a new debate with parameters " + newDebate.ToString() + ".");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the debate with the same id as the given debate, and replaces all data of the database copy with the given copy.
        /// </summary>
        /// <param name="session">The session that is updating the debate.</param>
        /// <param name="debate">The new debate data that the database will hold.</param>
        /// <returns>Returns true if the debate was successfully updated, false if an error occured or permissions were not high enough.</returns>
        public static bool UpdateDebate(HttpSessionState session, Debate debate)
        {
            User updatingUser = Help.GetUserSession(session);
            if (updatingUser != null && updatingUser.PermissionLevel >= permissionToUpdateDebates) //If the user's permission level is high enough
            {
                Debate previousDebateData = GetDebate(debate.ID); //We get the debate data that currently exists in the database...
                if (previousDebateData != null) //We ensure that the data exists, otherwise we cannot update something that doesn't exist.
                {
                    string sqlQuery = "UPDATE Debates SET " +
                                "TID1 = @Team1ID, TID2 = @Team2ID, T1Score = @Team1Score, T2Score = @Team2Score, Date = @Date, MorningDebate = @MorningDebate" +
                                " WHERE Id = " + debate.ID;
                    //ID is omitted because changing it will result in incorrect foreign keys in the debates table.

                    //Generating the parameters, this is done for sanitization reasons.
                    SqlParameter team1ID = new SqlParameter("@Team1ID", SqlDbType.Int);
                    team1ID.Value = debate.Team1.ID;
                    SqlParameter team2ID = new SqlParameter("@Team2ID", SqlDbType.Int);
                    team2ID.Value = debate.Team2.ID;
                    SqlParameter team1Score = new SqlParameter("@Team1Score", SqlDbType.Int);
                    team1Score.Value = debate.Team1Score;
                    SqlParameter team2Score = new SqlParameter("@Team2Score", SqlDbType.Int);
                    team2Score.Value = debate.Team2Score;

                    byte bitValue = 0;
                    if (debate.MorningDebate)
                        bitValue = 1;

                    SqlParameter morningDebate = new SqlParameter("@MorningDebate", SqlDbType.Bit);
                    morningDebate.Value = bitValue;

                    string dateString = Help.GetDateString(debate.Date);
                    SqlParameter date = new SqlParameter("@Date", SqlDbType.NChar, dateString.Length);
                    date.Value = dateString;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while updating a debate.",
                        team1ID, team2ID, team1Score, team2Score, morningDebate, date);

                    if (result != null)
                    {
                        Log(updatingUser.Username,
                            updatingUser.Username + " updated a debate from " + previousDebateData.ToString() + " to " + debate.ToString());

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all data in the debates table associated with the given id.
        /// </summary>
        /// <param name="session">The current session, used to determine the user performing this action.</param>
        /// <param name="id">The id of the debate to remove.</param>
        /// <returns>Returns true if the debate was properly removed, false if it was not.</returns>
        public static bool RemoveDebate(HttpSessionState session, int id)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.
            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToRemoveDebates) //If the user exists and their permission level is super referee or greater...
            {
                Debate debate = GetDebate(id); //We get the debate if it exists in the database (there are probably faster ways, but this works well enough and we need it later in the log).
                if (debate != null) //We ensure that the debate exists.
                {
                    string sqlQuery = "DELETE FROM Debates WHERE Id = @Value";
                    SqlParameter parameter = new SqlParameter("@Value", SqlDbType.Int);
                    parameter.Value = id;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while removing a debate.", parameter);

                    if (result != null) //If the result is not null, then the query succeeded and should be logged.
                    {
                        Log(currentSessionUser, currentSessionUser.Username + " removed a debate with the data of " + debate.ToString());
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Clears all the debates in the database.
        /// </summary>
        /// <param name="session">The session that is removing all the debates.</param>
        /// <returns></returns>
        public static bool ClearDebates(HttpSessionState session)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.
            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToClearDebates) //If the user exists and their permission level is correct.
            {
                string sqlQuery = "DELETE FROM Debates";

                SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while removing a debate.");

                if (result != null) //If the result is not null, then the query succeeded and should be logged.
                {
                    Log(currentSessionUser, currentSessionUser.Username + " has cleared the entire debate schedule.");
                    return true;
                }
            }

            return false;
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

            if (fullLogPath == string.Empty) //there is no full path, so one must be constructed.
            {
                CreateLogPath();
            }

            try
            {
                Directory.CreateDirectory(fullLogPath);

                using (StreamWriter writer = new StreamWriter(exceptionFileName + exceptionFileName, true))
                {
                    writer.WriteLine("Exception with message  \"" + e.Message + "\" was found. Additional Info: \"" + info + "\" logged at " + DateTime.Now); //Logs the message to the exception file.
                }
            }
            catch
            {
                //Well an exception occured, but it cannot be logged since this is the method for logging it. In this case we do nothing really.
            }

        }
        

    }
}