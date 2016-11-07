using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        /// The number of days that must pass before a user code expires.
        /// </summary>
        private static int daysBeforeCodeExpiration = 14;

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

        private static int permissionToGetUsers = 3;
        private static int permissionToAddUsers = 3;
        private static int permissionToRemoveUsers = 3;
        private static int permissionToUpdateUsers = 3;

        private static int permissionToAddTeams = 3;
        private static int permissionToUpdateTeams = 3;
        private static int permissionToRemoveTeams = 3;

        private static int permissionToAddDebates = 3;
        private static int permissionToUpdateDebates = 3;
        private static int permissionToRemoveDebates = 3;
        private static int permissionToClearDebates = 4;

        private static int permissionToAddNews = 3;
        private static int permissionToUpdateNews = 3;
        private static int permissionToRemoveNews = 3;

        private static int permissionToViewLogs = 3;

        private static int permissionToAddSeasons = 3;
        private static int permissionToUpdateSeasons = 3;
        private static int permissionToRemoveSeasons = 4; //debate seasons cannot be removed.

        private static int permissionToAddUserCodes = 3;
        private static int permissionToGetActiveUserCodes = 3;
        
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
        /// Gets the data between two rows in the data table.
        /// </summary>
        /// <param name="connectionString">The connection string used to direct the connection to a specific database.</param>
        /// <param name="table">The name of the table in the database to connect to and retrieve.</param>
        /// <param name="exceptionInfo">The info that will be logged in the event of an exception occuring.</param>
        /// <param name="startingRow">The row to start gathering data.</param>
        /// <param name="endingRow">The row to stop gathering data.</param>
        /// <returns>Returns a data table from the connected database.</returns>
        private static DataTable GetDataTable(string connectionString, string table, int startingRow, int endingRow, string exceptionInfo = "")
        {
            DataTable resultingTable = null;

            SqlConnection connection = new SqlConnection(GetConnectionStringUsersTable());

            try
            {
                //SqlCommand command = new SqlCommand("SELECT * FROM " + table + " WHERE Id BETWEEN " + startingRow + " AND " + endingRow, connection);
                //SqlCommand command = new SqlCommand("SELECT * FROM " + table + " LIMIT 10, 10", connection);
                SqlCommand command = new SqlCommand("SELECT * FROM (SELECT TOP " + endingRow + " ROW_NUMBER() OVER(ORDER BY Id) AS RowNr, * FROM " + table + ") as alias WHERE RowNr BETWEEN " + startingRow + " AND " + endingRow, connection);
                //SqlCommand command = new SqlCommand("SELECT * FROM " + table + " OFFSET " + startingRow + " ROWS FETCH NEXT " + (endingRow - startingRow) + " ROWS ONLY;", connection);
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
                    LogException(e, "exception occured while getting a database table between two rows.");
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
        /// <param name="caseSensitive">If true the case of the variable and match must be the same, if false then the capitalization of the variable and match won't matter.</param>
        /// <param name="exceptionInfo">The info that will be logged in the event of an exception occuring.</param>
        /// <returns>Returns a data table with the matched data from the connected database.</returns>
        private static DataTable GetDataTable(string connectionString, string table, string variable, string match, SqlDbType dataType, int maxLength, bool caseSensitive, string exceptionInfo = "")
        {
            DataTable resultingTable = null;

            SqlConnection connection = new SqlConnection(GetConnectionStringUsersTable());

            try
            {
                SqlCommand command;

                if (caseSensitive)
                    command = new SqlCommand("SELECT * FROM " + table + " WHERE " + variable + " = @LookFor", connection);
                else
                    command = new SqlCommand("SELECT * FROM " + table + " WHERE UPPER(" + variable + ") = UPPER(@LookFor)", connection);

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
        /// Gets the latest record added to a database table.
        /// </summary>
        /// <param name="connectionString">The connection string used to direct the connection to a specific database.</param>
        /// <param name="table">The name of the table in the database to connect to.</param>
        /// <param name="exceptionInfo">The info that will be logged in the event of an exception occuring.</param>
        /// <returns>Returns a data table with the matched data from the connected database.</returns>
        private static DataTable GetLatestRecord(string connectionString, string table, string exceptionInfo = "")
        {
            DataTable resultingTable = null;

            SqlConnection connection = new SqlConnection(GetConnectionStringUsersTable());

            try
            {
                SqlCommand command = new SqlCommand("SELECT TOP 1 * FROM " + table + " ORDER BY Id DESC", connection);
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
                    LogException(e, "exception occured while getting a database table's latest record.");
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
        /// Authenticates the given answer to a user's security question with the real answer.
        /// </summary>
        /// <param name="username">The username of the user who holds the security question to test against.</param>
        /// <param name="answer">The answer to test against the real answer, case insensitive.</param>
        /// <returns>Returns true if the answer matches, false if it does not.</returns>
        public static bool AuthenticateSecurityQuestion(string username, string answer)
        {
            string realAnswer = GetUserSecurityAnswer(username);
            if (realAnswer.ToUpperInvariant() == answer.ToUpperInvariant())
            {
                return true;
            }

            return false;
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
            
            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Users", "Name", username, SqlDbType.NChar, 50, false, "exception occured while authenticating username/password.");

            if (table.Rows.Count > 0)
            {
                DataRow userRow = table.Rows[0]; //If there is more than 1 row, then we've got a problem because there's more than 1 matching username.
                string matchedName = userRow["Name"] as string;
                string matchedPassword = userRow["Password"] as string;
                string matchedEmail = userRow["Email"] as string;
                string matchedQuestion = userRow["SecurityQuestion"] as string;
                int matchedPermissions = (int)userRow["Permissions"];
                int matchedID = (int)userRow["Id"];

                if (matchedName == realUsername && password == matchedPassword) //Compares the username and password to the username and password found in the database.
                {
                    //Log the user in, as the username and password match.
                    resultingUser = new User(matchedPermissions, username, matchedEmail, matchedQuestion, matchedID);
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
        /// Sends an email containing the username and password of the given username.
        /// </summary>
        /// <param name="username">The username of the user whose email will be used to send their password.</param>
        public static void EmailUserPassword(string username)
        {
            string password = string.Empty;
            string email = string.Empty;

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Users", "Name", username.ToUpperInvariant(), SqlDbType.NVarChar, username.Length, true, "exception occured while gathering a user.");
            if (table.Rows.Count > 0)
            {
                email = table.Rows[0]["Email"] as string;
                password = table.Rows[0]["Password"] as string;
            }

            //return password; //TODO: Make this method email the password instead of returning the password.

            SmtpClient smtpClient = new SmtpClient("smtp@gmail.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("TeamWheresTheRightClick", "Right123456");
            smtpClient.EnableSsl = true;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("TeamWheresTheRightClick@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Password Recovery";
            mail.Body = "A password recovery request was completed in Team Where's the Right Click's Debate Scheduler. The username and password to your account is \nUsername: " + username + " \nPassword: " + password;
            mail.IsBodyHtml = true;

            smtpClient.Send(mail);
        }

        /// <summary>
        /// Checks whether a username already exists in the database.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>Returns true if the username exists, false otherwise.</returns>
        public static bool UserExists(string username)
        {
            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Users", "Name", username.ToUpperInvariant(), SqlDbType.NChar, 50, true, "exception occured while checking if a user exists.");

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
        /// Gets and returns the security question of the given username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>Returns the security question of the user who matches usernames with the given username.</returns>
        public static string GetUserSecurityQuestion(string username)
        {
            string question = string.Empty;

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Users", "Name", username.ToUpperInvariant(), SqlDbType.NVarChar, username.Length, false, "exception occured while gathering a user's security question.");
            if (table.Rows.Count > 0)
            {
                question = table.Rows[0]["SecurityQuestion"] as string;
            }

            return question;
        }

        /// <summary>
        /// Gets and returns the security answer of the given username. This can only be called inside the DatabaseHandler for security reasons.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>Returns the security answer of the user who matches usernames with the given username.</returns>
        private static string GetUserSecurityAnswer(string username)
        {
            string answer = string.Empty;

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Users", "Name", username.ToUpperInvariant(), SqlDbType.NVarChar, username.Length, false, "exception occured while gathering a user's security answer.");
            if (table.Rows.Count > 0)
            {
                answer = table.Rows[0]["SecurityAnswer"] as string;
            }

            return answer;
        }

        /// <summary>
        /// Gets the user with the given username.
        /// </summary>
        /// <param name="session">The session that is trying to find a user.</param>
        /// <param name="username">The username to find in the database.</param>
        /// <returns>Returns null if there was no username by the given name.</returns>
        public static User GetUser(HttpSessionState session, string username)
        {
            User currentSessionUser = Help.GetUserSession(session);
            User resultingUser = null;

            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToGetUsers)
            {
                DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Users", "Name", username.ToUpperInvariant(), SqlDbType.NVarChar, username.Length, true, "exception occured while gathering a user.");
                if (table.Rows.Count > 0)
                {
                    int id = (int)table.Rows[0]["Id"];
                    //string matchedUsername = table.Rows[0]["Name"] as string;
                    string email = table.Rows[0]["Email"] as string;
                    string securityQuestion = table.Rows[0]["SecurityQuestion"] as string;
                    int permissions = (int)table.Rows[0]["Permissions"];
                    resultingUser = new User(permissions, username, email, securityQuestion, id); //NOTE: We use the username we were given not the one in the database. This prevents the username from being all caps.
                }
            }

            return resultingUser;
        }

        /// <summary>
        /// Gets the user with the given user id.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>Returns null if there was no id in the database.</returns>
        private static User GetUser(int id)
        {
            User resultingUser = null;

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Users", "Id", id.ToString(), SqlDbType.Int, int.MaxValue, true, "exception occured while gathering a user by id.");
            if (table.Rows.Count > 0)
            {
                string username = table.Rows[0]["Name"] as string;
                string email = table.Rows[0]["Email"] as string;
                string securityQuestion = table.Rows[0]["SecurityQuestion"] as string;
                int permissions = (int)table.Rows[0]["Permissions"];
                resultingUser = new User(permissions, username, email, securityQuestion, id);
            }

            return resultingUser;
        }

        /// <summary>
        /// Adds a user to the database.
        /// </summary>
        /// <param name="session">The session, used to determine the user who is adding to the database. The session user must have high enough permissions or nothing will happen.</param>
        /// <param name="newUser">The new user being added to the database.</param>
        /// <param name="password">The plain text password of the user being added.</param>
        /// <returns>Returns true if the user was successfully added, false if the user already exists or an error occured.</returns>
        public static bool AddUser(HttpSessionState session, User newUser, string password, string securityAnswer)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.
            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToAddUsers) //If the user exists and their permission level is super referee or greater...
            {
                string realUsername = newUser.Username.ToUpperInvariant(); //The username as it would appear in the database.
                string realEmail = newUser.Email.ToUpperInvariant();

                if (!UserExists(realUsername)) //If the username does not exist.
                {
                    string sqlQuery = "INSERT INTO Users (Name, Password, Permissions, Email, SecurityQuestion, SecuritAnswer) VALUES " +
                        "(@Username, @Password, '" + newUser.PermissionLevel + "', @Email, @SecurityQuestion, @SecurityAnswer)";

                    SqlParameter username = new SqlParameter("@Username", SqlDbType.NVarChar, newUser.Username.Length);
                    username.Value = realUsername;
                    SqlParameter passwordParameter = new SqlParameter("@Password", SqlDbType.NVarChar, password.Length);
                    passwordParameter.Value = password;
                    SqlParameter emailParameter = new SqlParameter("@Email", SqlDbType.NVarChar, realEmail.Length);
                    emailParameter.Value = realEmail;
                    SqlParameter securityQuestion = new SqlParameter("@SecurityQuestion", SqlDbType.NVarChar, newUser.SecurityQuestion.Length);
                    securityQuestion.Value = newUser.SecurityQuestion;
                    SqlParameter securityAnswerPara = new SqlParameter("@SecurityAnswer", SqlDbType.NVarChar, securityAnswer.Length);
                    securityAnswerPara.Value = securityAnswer;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a user.",
                        username, passwordParameter, emailParameter, securityQuestion, securityAnswerPara);

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
        public static bool AddUser(string ipAddress, User newUser, string password, string securityAnswer)
        {
            if (ipAddress != "") //If the ip address is not blank.
            {
                string realUsername = newUser.Username.ToUpperInvariant(); //The username as it would appear in the database.
                string realEmail = newUser.Email.ToUpperInvariant();

                if (!UserExists(realUsername)) //If the username does not exist.
                {
                    string sqlQuery = "INSERT INTO Users (Name, Password, Permissions, Email, SecurityQuestion, SecurityAnswer) VALUES " +
                        "(@Username, @Password, '" + newUser.PermissionLevel + "', @Email, @SecurityQuestion, @SecurityAnswer)";

                    SqlParameter username = new SqlParameter("@Username", SqlDbType.NVarChar, newUser.Username.Length);
                    username.Value = realUsername;
                    SqlParameter passwordParameter = new SqlParameter("@Password", SqlDbType.NVarChar, password.Length);
                    passwordParameter.Value = password;
                    SqlParameter emailParameter = new SqlParameter("@Email", SqlDbType.NVarChar, realEmail.Length);
                    emailParameter.Value = realEmail;
                    SqlParameter securityQuestion = new SqlParameter("@SecurityQuestion", SqlDbType.NVarChar, newUser.SecurityQuestion.Length);
                    securityQuestion.Value = newUser.SecurityQuestion;
                    SqlParameter securityAnswerPara = new SqlParameter("@SecurityAnswer", SqlDbType.NVarChar, securityAnswer.Length);
                    securityAnswerPara.Value = securityAnswer;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a user.",
                        username, passwordParameter, emailParameter, securityQuestion, securityAnswerPara);
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
        /// Updates a user with the given user object.
        /// </summary>
        /// <param name="session">The session that is updating the user.</param>
        /// <param name="user">The user object, whose data will replace the current data in the database.</param>
        /// <returns>Returns true if the update worked, false otherwise.</returns>
        public static bool UpdateUser(HttpSessionState session, User user)
        {
            User updatingUser = Help.GetUserSession(session);
            if (updatingUser != null && updatingUser.PermissionLevel >= permissionToUpdateUsers) //If the user's permission level is high enough
            {
                if (UserExists(user.Username)) //We ensure that the data exists, otherwise we cannot update something that doesn't exist.
                {
                    string sqlQuery = "UPDATE Users SET " +
                                "Name = @Name, Email = @Email, Permissions = @Permissions, SecurityQuestion = @SecurityQuestion" +
                                " WHERE Id = " + user.ID;
                    //ID is omitted because changing it will result in incorrect foreign keys in the debates table.

                    //Generating the parameters, this is done for sanitization reasons.
                    SqlParameter name = new SqlParameter("@Name", SqlDbType.NVarChar, user.Username.Length); //It is important the size is the size of the string and no the max limit.
                    name.Value = user.Username.ToUpperInvariant();
                    SqlParameter email = new SqlParameter("@Email", SqlDbType.NVarChar, user.Email.Length);
                    email.Value = user.Email;
                    SqlParameter permissions = new SqlParameter("@Permissions", SqlDbType.Int);
                    permissions.Value = user.PermissionLevel;
                    SqlParameter securityQuestion = new SqlParameter("@SecurityQuestion", SqlDbType.NVarChar, user.SecurityQuestion.Length);
                    securityQuestion.Value = user.SecurityQuestion;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while updating a user.",
                        name, email, permissions, securityQuestion);

                    if (result != null)
                    {
                        Log(updatingUser.Username,
                            updatingUser.Username + " updated a user with the username " + user.Username);

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the session user's security info with the given info.
        /// </summary>
        /// <param name="session">The session whose user is updating their info.</param>
        /// <param name="securityQuestion">The security question.</param>
        /// <param name="securityAnswer">The security answer.</param>
        /// <returns>Returns true if the security info was updating correctly, false if it was not updated.</returns>
        public static bool UpdateUserSecurity(HttpSessionState session, string securityQuestion, string securityAnswer)
        {
            User updatingUser = Help.GetUserSession(session);
            User targetedUser = GetUser(session, updatingUser.Username);
            if (updatingUser != null && targetedUser.ID == updatingUser.ID) //If the user changing the security questions/answers is the same user
            {
                if (targetedUser != null) //We ensure that the data exists, otherwise we cannot update something that doesn't exist.
                {
                    string sqlQuery = "UPDATE Users SET " +
                                "SecurityAnswer = @SecurityAnswer, SecurityQuestion = @SecurityQuestion" +
                                " WHERE Id = " + updatingUser.ID;
                    //ID is omitted because changing it will result in incorrect foreign keys in the debates table.

                    //Generating the parameters, this is done for sanitization reasons.
                    SqlParameter securityAnswerParameter = new SqlParameter("@SecurityAnswer", SqlDbType.NVarChar, securityAnswer.Length); //It is important the size is the size of the string and no the max limit.
                    securityAnswerParameter.Value = securityAnswer;
                    SqlParameter securityQuestionParameter = new SqlParameter("@SecurityQuestion", SqlDbType.NVarChar, securityQuestion.Length);
                    securityQuestionParameter.Value = securityQuestion;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while updating a user's security info.",
                        securityAnswerParameter, securityQuestionParameter);

                    if (result != null)
                    {
                        Log(updatingUser.Username,
                            updatingUser.Username + " updated their security info to Question: \"" + securityQuestion + "\" Answer: \"" + securityAnswer + "\"");

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

                if (UserExists(realUsername)) //If the username does exist.
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

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Teams", "Id", id.ToString(), SqlDbType.Int, int.MaxValue, false, "exception occured while gathering a single team by ID.");

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
        /// Gets a team from the database based on their name. This returns the most recent addition to the database, if there are more than one team with the same name. Use by ID to get exact teams.
        /// </summary>
        /// <param name="name">The name of the team to search for.</param>
        /// <returns>Returns a team object which contains all the data of the matching team in the database. This will be null if there is no matching team.</returns>
        public static Team GetTeam(string name)
        {
            Team newTeam = null;

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Teams", "Name", name, SqlDbType.NChar, 50, false, "exception occured while gathering a single team by name.");

            if (table.Rows.Count > 0)
            {
                int id = (int)table.Rows[table.Rows.Count - 1]["Id"];
                string teamName = table.Rows[table.Rows.Count - 1]["Name"] as string;
                int wins = (int)table.Rows[table.Rows.Count - 1]["Wins"];
                int losses = (int)table.Rows[table.Rows.Count - 1]["Losses"];
                int ties = (int)table.Rows[table.Rows.Count - 1]["Ties"];
                int totalScore = (int)table.Rows[table.Rows.Count - 1]["TotalScore"];
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

                //if (matchedTeam == null) //If there is no team that matches names. This is done when creating the schedule instead.
                //{
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
                //}
            }

            return false;
        }

        /// <summary>
        /// Adds the given team to the database.
        /// </summary>
        /// <param name="session">The session of the user who is adding the team.</param>
        /// <param name="newTeam">The new team that is being added. ID is ignored.</param>
        /// <param name="id">The id given to the team. This will be -1 if there was an error or no id was assigned.</param>
        /// <returns>Returns true if the team was successfully added to the database, false if it was not added.</returns>
        public static bool AddTeam(HttpSessionState session, Team newTeam, out int id)
        {
            id = -1;
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.

            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToAddTeams && newTeam != null) //If the given team is not null and proper permissions are matched.
            {
                Team matchedTeam = GetTeam(newTeam.Name);

                //if (matchedTeam == null) //If there is no team that matches names. This is done when creating the schedule instead.
                //{
                    string sqlQuery = "INSERT INTO Teams (Name, Wins, Losses, Ties, TotalScore) " + //Id, 
                        "OUTPUT INSERTED.Id " +
                        "VALUES(@TeamName, '" + newTeam.Wins + "', '" + newTeam.Losses + "', '" + newTeam.Ties + "', '" + newTeam.TotalScore + "')"; //'" + newUser.ID + "', 
                    SqlParameter teamName = new SqlParameter("@TeamName", SqlDbType.NChar, newTeam.Name.Length);
                    teamName.Value = newTeam.Name;

                    object result = ExecuteSQLScaler(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a team.", teamName);
                    if (result != null) //If the result is not null, then the query succeeded and should be logged.
                    {
                        id = (int)result;
                        Log(currentSessionUser.Username, currentSessionUser.Username + " added a new team with parameters " + newTeam.ToString() + ".");
                        return true;
                    }
                //}
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

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Debates", "Id", id.ToString(), SqlDbType.Int, int.MaxValue, true, "exception occured while gathering a single debate by ID.");

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
        /// Adds a new debate to the database.
        /// </summary>
        /// <param name="session">The session that is adding the debate.</param>
        /// <param name="newDebate">The new debate that is being added.</param>
        /// <param name="id">The id of the inserted debate. This will be -1 if no id was set or there was an error.</param>
        /// <returns>Returns true if the debate was successfully added, false if it was not added.</returns>
        public static bool AddDebate(HttpSessionState session, Debate newDebate, out int id)
        {
            id = -1;
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.

            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToAddDebates && newDebate != null) //If the given debate is not null and proper permissions are matched.
            {
                string dateString = Help.GetDateString(newDebate.Date);

                int bitValue = 0;
                if (newDebate.MorningDebate)
                    bitValue = 1;

                string sqlQuery = "INSERT INTO Debates (TID1, TID2, T1Score, T2Score, Date, MorningDebate)" +
                    " OUTPUT INSERTED.Id " +
                        "VALUES ('" + newDebate.Team1.ID + "', '" + newDebate.Team2.ID + "', '" + newDebate.Team1Score + "', '" + newDebate.Team2Score + "', '" + dateString + "', '" + bitValue + "')";

                object result = ExecuteSQLScaler(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a debate.");
                if (result != null) //If the result is not null, then the query succeeded and should be logged.
                {
                    id = (int)result;
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
        /// Gets a list of news posts between two indexs.
        /// </summary>
        /// <param name="startingIndex">The starting index.</param>
        /// <param name="endingIndex">The ending index.</param>
        /// <returns>Returns an empty list if there are no news posts, otherwise populates a list of news posts that were found between two indexes in the database.</returns>
        public static List<NewsPost> GetNewsPosts(int startingIndex, int endingIndex)
        {
            List<NewsPost> posts = new List<NewsPost>();

            if (endingIndex < startingIndex) //In the event the ending index is below the starting index, the ending index becomes the starting index and only that index is selected.
                endingIndex = startingIndex;

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "News", startingIndex, endingIndex, "exception occured while gathering a group of news posts.");

            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    int id = (int)row["Id"];

                    int creatorID = (int)row["UserID"];
                    User creator = GetUser(creatorID);

                    string dateString = row["Date"] as string;
                    DateTime date = Help.GetDate(dateString);

                    string updateDateString = row["LastUpdateDate"] as string;
                    DateTime updateDate = Help.GetDate(updateDateString);

                    string title = row["Title"] as string;

                    string data = row["NewsData"] as string;

                    NewsPost post = new NewsPost(id, creator, date, title, data);
                    post.LastUpdateDate = updateDate;
                    posts.Add(post);

                }
            }

            return posts;
        }

        /// <summary>
        /// Gets a news post by the id.
        /// </summary>
        /// <param name="id">The id of the news post.</param>
        /// <returns>Returns a news post object if the id was valid, otherwise it returns null.</returns>
        public static NewsPost GetNewsPost(int id)
        {
            NewsPost resultingNewsPost = null;

            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "News", "Id", id.ToString(), SqlDbType.Int, int.MaxValue, true, "exception occured while gathering a news post.");

            if (table.Rows.Count > 0)
            {
                int creatorID = (int)table.Rows[0]["UserID"];
                User creator = GetUser(creatorID);

                string dateString = table.Rows[0]["Date"] as string;
                DateTime date = Help.GetDate(dateString);

                string updateDateString = table.Rows[0]["LastUpdateDate"] as string;
                DateTime updateDate = Help.GetDate(updateDateString);

                string title = table.Rows[0]["Title"] as string;

                string data = table.Rows[0]["NewsData"] as string;

                resultingNewsPost = new NewsPost(id, creator, date, title, data);
                resultingNewsPost.LastUpdateDate = updateDate;
            }

            return resultingNewsPost;
        }

        /// <summary>
        /// Adds a news post to the database.
        /// </summary>
        /// <param name="session">The current session, used to determine the user performing this action.</param>
        /// <param name="post">The news post that is being added to the database.</param>
        /// <returns>Returns true if the news post was successfully added to the database, false otherwise.</returns>
        public static bool AddNewsPost(HttpSessionState session, NewsPost post)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.

            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToAddNews && post != null) //If the given debate is not null and proper permissions are matched.
            {
                string dateString = Help.GetDateString(post.Date);
                string updateDateString = Help.GetDateString(post.LastUpdateDate);

                string sqlQuery = "INSERT INTO News (UserID, Date, LastUpdateDate, NewsData, Title) VALUES " +
                        "(@UserID, @Date, @UpdateDate, @Data, @Title)";

                SqlParameter userID = new SqlParameter("@UserID", SqlDbType.Int);
                userID.Value = post.Creator.ID;
                SqlParameter date = new SqlParameter("@Date", SqlDbType.NVarChar, dateString.Length);
                date.Value = dateString;
                SqlParameter updateDate = new SqlParameter("@UpdateDate", SqlDbType.NVarChar, updateDateString.Length);
                updateDate.Value = updateDateString;
                SqlParameter data = new SqlParameter("@Data", SqlDbType.NVarChar, post.Data.Length);
                data.Value = post.Data;
                SqlParameter title = new SqlParameter("@Title", SqlDbType.NVarChar, post.Title.Length);
                title.Value = post.Title;

                SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a news post.",
                    userID, date, updateDate, data, title);

                if (result != null) //If the result is not null, then the query succeeded and should be logged.
                {
                    Log(currentSessionUser.Username, currentSessionUser.Username + " added a new news post with parameters " + post.ToString() + ".");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the news post with the same id as the given news post, and replaces all data of the database copy with the given copy.
        /// </summary>
        /// <param name="session">The session that is updating the news post.</param>
        /// <param name="post">The new debate data that the database will hold.</param>
        /// <returns>Returns true if the news post was successfully updated, false if an error occured or permissions were not high enough.</returns>
        public static bool UpdateNewsPost(HttpSessionState session, NewsPost post)
        {
            User updatingUser = Help.GetUserSession(session);
            if (updatingUser != null && updatingUser.PermissionLevel >= permissionToUpdateNews) //If the user's permission level is high enough
            {
                NewsPost currentPost = GetNewsPost(post.ID);
                if (currentPost != null) //We ensure that the data exists, otherwise we cannot update something that doesn't exist.
                {
                    string sqlQuery = "UPDATE News SET " +
                                "NewsData = @Data, LastUpdateDate = @UpdateDate, Title = @Title" +
                                " WHERE Id = " + post.ID;

                    string updateDateString = Help.GetDateString(DateTime.Now); //The current time is used as the updating time.

                    //Generating the parameters, this is done for sanitization reasons.
                    SqlParameter postData = new SqlParameter("@Data", SqlDbType.NVarChar, post.Data.Length);
                    postData.Value = post.Data;

                    SqlParameter updateDate = new SqlParameter("@UpdateDate", SqlDbType.NVarChar, updateDateString.Length);
                    updateDate.Value = updateDateString;

                    SqlParameter title = new SqlParameter("@Title", SqlDbType.NVarChar, post.Title.Length);
                    title.Value = post.Title;
                    
                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while updating a news post.",
                        postData, updateDate, title);

                    if (result != null)
                    {
                        Log(updatingUser.Username,
                            updatingUser.Username + " updated a news post from " + currentPost.ToString() + " to " + post.ToString());

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
        /// <param name="id">The id of the news post to remove.</param>
        /// <returns>Returns true if the news post was properly removed, false if it was not.</returns>
        public static bool RemoveNewsPost(HttpSessionState session, int id)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.
            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToRemoveNews) //If the user exists and their permission level is super referee or greater...
            {
                NewsPost post = GetNewsPost(id);
                if (post != null) //We ensure that the debate exists.
                {
                    string sqlQuery = "DELETE FROM News WHERE Id = @Value";
                    SqlParameter parameter = new SqlParameter("@Value", SqlDbType.Int);
                    parameter.Value = id;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while removing a news post.", parameter);

                    if (result != null) //If the result is not null, then the query succeeded and should be logged.
                    {
                        Log(currentSessionUser, currentSessionUser.Username + " removed a news post with the data of " + 
                            "{ " + post.Data + " }");
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the most recent debate season found in the database.
        /// </summary>
        /// <returns>Returns a debate season that was found in the database which was also the most recent addition.</returns>
        public static DebateSeason GetMostRecentSeason()
        {
            DebateSeason resultingSeason = null;
            DataTable table = GetLatestRecord(GetConnectionStringUsersTable(), "Seasons", "exception occured while gathering a debate season.");

            if (table.Rows.Count > 0)
            {
                string debateString = table.Rows[0]["Debates"] as string;
                List<int> debateIDs = DebateSeason.ParseDebateString(debateString);

                string teamsString = table.Rows[0]["Teams"] as string;
                List<int> teamIDs = DebateSeason.ParseTeamString(teamsString);

                int id = (int)table.Rows[0]["Id"];

                bool hasEnded = Convert.ToBoolean(table.Rows[0]["HasEnded"]);

                //Loading in the teams
                List<Team> teams = new List<Team>();
                foreach (int i in teamIDs)
                {
                    teams.Add(GetTeam(i));
                }

                //Loading in the debates
                List<Debate> debates = new List<Debate>();
                foreach (int i in debateIDs)
                {
                    debates.Add(GetDebate(i));
                }

                resultingSeason = new DebateSeason(id, hasEnded, teams, debates);
            }

            return resultingSeason;
        }

        /// <summary>
        /// Gets the ID of the most recent debate season in the database.
        /// </summary>
        /// <returns>Returns the id of the most recent debate season added in the database.</returns>
        public static int GetMostRecentSeasonID()
        {
            int id = -1;

            DataTable table = GetLatestRecord(GetConnectionStringUsersTable(), "Seasons", "exception occured while gathering a debate season.");

            if (table.Rows.Count > 0)
            {
                id = (int)table.Rows[0]["Id"];
            }

            return id;
        }

        /// <summary>
        /// Gets the ID of the most recent debate season in the database.
        /// </summary>
        /// <param name="active">If true the season is still active, if false then the season has ended.</param>
        /// <returns>Returns the id of the most recent debate season added in the database.</returns>
        public static int GetMostRecentSeasonID(out bool active)
        {
            int id = -1;
            active = false;

            DataTable table = GetLatestRecord(GetConnectionStringUsersTable(), "Seasons", "exception occured while gathering a debate season.");

            if (table.Rows.Count > 0)
            {
                id = (int)table.Rows[0]["Id"];

                active = !Convert.ToBoolean(table.Rows[0]["HasEnded"]);
            }

            return id;
        }

        /// <summary>
        /// Gets the list of teams in a given debate season.
        /// </summary>
        /// <param name="id">The id of the debate season.</param>
        /// <returns>Returns a list of teams populated from the database.</returns>
        public static List<Team> GetDebateSeasonTeams(int id)
        {
            List<Team> teams = new List<Team>();
            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Seasons", "Id", id.ToString(), SqlDbType.Int, int.MaxValue, true, "exception occured while gathering a debate season.");

            if (table.Rows.Count > 0)
            {
                string teamsString = table.Rows[0]["Teams"] as string;
                List<int> teamIDs = DebateSeason.ParseTeamString(teamsString);

                //Loading in the teams
                foreach (int i in teamIDs)
                {
                    teams.Add(GetTeam(i));
                }
                
            }

            return teams;
        }

        /// <summary>
        /// Gets a list of debates in a given debate season.
        /// </summary>
        /// <param name="id">The id of the debate.</param>
        /// <returns>Returns a list of debates in a given season.</returns>
        public static List<Debate> GetDebateSeasonDebates(int id)
        {
            List<Debate> debates = new List<Debate>();
            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Seasons", "Id", id.ToString(), SqlDbType.Int, int.MaxValue, true, "exception occured while gathering a debate season.");

            if (table.Rows.Count > 0)
            {
                string debateString = table.Rows[0]["Debates"] as string;
                List<int> debateIDs = DebateSeason.ParseDebateString(debateString);
                
                //Loading in the debates
                foreach (int i in debateIDs)
                {
                    debates.Add(GetDebate(i));
                }
            }

            return debates;
        }

        /// <summary>
        /// Gets a debate season by id.
        /// </summary>
        /// <param name="id">The id of the debate season.</param>
        /// <returns>Returns a debate season populated with the debates and teams associated with it.</returns>
        public static DebateSeason GetDebateSeason(int id)
        {
            DebateSeason resultingSeason = null;
            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "Seasons", "Id", id.ToString(), SqlDbType.Int, int.MaxValue, true, "exception occured while gathering a debate season.");

            if (table.Rows.Count > 0)
            {
                string debateString = table.Rows[0]["Debates"] as string;
                List<int> debateIDs = DebateSeason.ParseDebateString(debateString);

                string teamsString = table.Rows[0]["Teams"] as string;
                List<int> teamIDs = DebateSeason.ParseTeamString(teamsString);

                bool hasEnded = Convert.ToBoolean(table.Rows[0]["HasEnded"]);

                //Loading in the teams
                List<Team> teams = new List<Team>();
                foreach (int i in teamIDs)
                {
                    teams.Add(GetTeam(i));
                }

                //Loading in the debates
                List<Debate> debates = new List<Debate>();
                foreach (int i in debateIDs)
                {
                    debates.Add(GetDebate(i));
                }

                resultingSeason = new DebateSeason(id, hasEnded, teams, debates);
            }

            return resultingSeason;
        }

        /// <summary>
        /// Adds a debate season to the database.
        /// </summary>
        /// <param name="session">The session that is adding the debate season.</param>
        /// <param name="season">The debate season being added.</param>
        /// <returns>Returns true if the debate season was added to the database, false otherwise.</returns>
        public static bool AddDebateSeason(HttpSessionState session, DebateSeason season)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.

            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToAddSeasons && season != null) //If the given season is not null and proper permissions are matched.
            {
                string sqlQuery = "INSERT INTO Seasons (Debates, Teams, HasEnded) VALUES " +
                        "(@Debates, @Teams, @Ended)";

                string debateString = season.GetDebateString();
                string teamString = season.GetTeamString();

                byte bitValue = 0;
                if (season.HasEnded)
                    bitValue = 1;

                SqlParameter debates = new SqlParameter("@Debates", SqlDbType.NVarChar, debateString.Length);
                debates.Value = debateString;
                SqlParameter teams = new SqlParameter("@Teams", SqlDbType.NVarChar, teamString.Length);
                teams.Value = teamString;
                SqlParameter ended = new SqlParameter("@Ended", SqlDbType.Bit);
                ended.Value = bitValue;

                SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a new debate season.",
                    debates,teams, ended);

                if (result != null) //If the result is not null, then the query succeeded and should be logged.
                {
                    Log(currentSessionUser.Username, currentSessionUser.Username + " added a new debate season with the id " + season.ToString() + ".");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a debate season to the database.
        /// </summary>
        /// <param name="session">The session that is adding the debate season.</param>
        /// <param name="season">The debate season being added.</param>
        /// <param name="id">The id assigned to the debate season when it was added to the database.</param>
        /// <returns>Returns true if the debate season was added to the database, false otherwise.</returns>
        public static bool AddDebateSeason(HttpSessionState session, DebateSeason season, out int id)
        {
            id = -1;
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.

            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToAddSeasons && season != null) //If the given season is not null and proper permissions are matched.
            {
                string sqlQuery = "INSERT INTO Seasons (Debates, Teams, HasEnded) " +
                    "OUTPUT INSERTED.Id "+ 
                        "VALUES(@Debates, @Teams, @Ended)";

                string debateString = season.GetDebateString();
                string teamString = season.GetTeamString();

                byte bitValue = 0;
                if (season.HasEnded)
                    bitValue = 1;

                SqlParameter debates = new SqlParameter("@Debates", SqlDbType.NVarChar, debateString.Length);
                debates.Value = debateString;
                SqlParameter teams = new SqlParameter("@Teams", SqlDbType.NVarChar, teamString.Length);
                teams.Value = teamString;
                SqlParameter ended = new SqlParameter("@Ended", SqlDbType.Bit);
                ended.Value = bitValue;


                object result = ExecuteSQLScaler(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a new debate season.",
                    debates, teams, ended);

                if (result != null) //If the result is not null, then the query succeeded and should be logged.
                {
                    id = (int)result;
                    Log(currentSessionUser.Username, currentSessionUser.Username + " added a new debate season with the id " + season.ToString() + ".");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates a debate season with the new data.
        /// </summary>
        /// <param name="session">The session that is updating the debate season.</param>
        /// <param name="season">The debate season data that has changed and will overwrite the existing data.</param>
        /// <returns>Returns true if the debate season was updated, false otherwise.</returns>
        public static bool UpdateDebateSeason(HttpSessionState session, DebateSeason season)
        {
            User updatingUser = Help.GetUserSession(session);
            if (updatingUser != null && updatingUser.PermissionLevel >= permissionToUpdateSeasons) //If the user's permission level is high enough
            {
                DebateSeason currentSeason = GetDebateSeason(season.ID);
                if (currentSeason != null) //We ensure that the data exists, otherwise we cannot update something that doesn't exist.
                {
                    string sqlQuery = "UPDATE Seasons SET " +
                                "Debates = @Debates, Teams = @Teams, HasEnded = @Ended" +
                                " WHERE Id = " + season.ID;
                    
                    //Generating the parameters, this is done for sanitization reasons.
                    string debateString = season.GetDebateString();
                    SqlParameter debateData = new SqlParameter("@Debates", SqlDbType.NVarChar, debateString.Length);
                    debateData.Value = debateString;

                    string teamString = season.GetTeamString();
                    SqlParameter teamData = new SqlParameter("@Teams", SqlDbType.NVarChar, teamString.Length);
                    teamData.Value = teamString;

                    byte bitValue = 0;
                    if (season.HasEnded)
                        bitValue = 1;
                    SqlParameter ended = new SqlParameter("@Ended", SqlDbType.Bit);
                    ended.Value = bitValue;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while updating a debate season.",
                        debateData, teamData, ended);

                    if (result != null)
                    {
                        Log(updatingUser.Username,
                            updatingUser.Username + " updated tbe debate season with the id " + season.ToString());

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes a debate season that matches with the given id.
        /// </summary>
        /// <param name="session">The session removing the debate season.</param>
        /// <param name="id">the id of the debate season to remove.</param>
        /// <returns>Returns true if the debate season was successfully removed, false otherwise.</returns>
        public static bool RemoveDebateSeason(HttpSessionState session, int id)
        {
            User currentSessionUser = Help.GetUserSession(session); //Get the current user who is running this code.
            if (currentSessionUser != null && currentSessionUser.PermissionLevel >= permissionToRemoveSeasons) //If the user exists and their permission level is suffecient.
            {
                DebateSeason currentSeason = GetDebateSeason(id);
                if (currentSeason != null) //We ensure that the debate exists.
                {
                    string sqlQuery = "DELETE FROM Seasons WHERE Id = @Value";
                    SqlParameter parameter = new SqlParameter("@Value", SqlDbType.Int);
                    parameter.Value = id;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while removing a debate season.", parameter);

                    if (result != null) //If the result is not null, then the query succeeded and should be logged.
                    {
                        Log(currentSessionUser, currentSessionUser.Username + " removed a debate season with id of " + currentSeason.ToString());
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the given user code exists in the database.
        /// </summary>
        /// <param name="code">The code to test.</param>
        /// <returns>Returns true if the code exists in the database, false otherwise.</returns>
        private static bool UserCodeExists(string code, out UserCodeError error)
        {
            DataTable table = GetDataTable(GetConnectionStringUsersTable(), "UserCodes", "Code", code, SqlDbType.NVarChar, code.Length, true, "exception occured while gathering a user code.");
            error = UserCodeError.CodeDoesntExist;

            if (table.Rows.Count > 0)
            {
                error = UserCodeError.None;

                bool hasBeenUsed = Convert.ToBoolean(table.Rows[0]["Used"]);
                string dateString = table.Rows[0]["Date"] as string;
                DateTime date = Help.GetDate(dateString);

                if ((date - DateTime.Now).Days > daysBeforeCodeExpiration)
                {
                    error = UserCodeError.CodeExpired;
                }
                else if (hasBeenUsed)
                {
                    error = UserCodeError.CodeUsed;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a list of active codes in the database.
        /// </summary>
        /// <param name="session">The session that is getting the active codes.</param>
        /// <returns>Returns a list of codes that can be currently be redeemed.</returns>
        public static List<string> GetActiveCodes(HttpSessionState session)
        {
            User sessionUser = Help.GetUserSession(session);
            List<string> codes = new List<string>();

            if (sessionUser != null && sessionUser.PermissionLevel >= permissionToAddUserCodes) //If the user exists and their permissions are high enough.
            {
                DataTable table = GetDataTable(GetConnectionStringUsersTable(), "UserCodes", "exception occured while gathering the user codes table.");

                foreach (DataRow row in table.Rows)
                {
                    string dateString = row["Date"] as string;
                    DateTime date = Help.GetDate(dateString);
                    
                    bool used = Convert.ToBoolean(row["Used"]);

                    if (!used && (date - DateTime.Now).Days <= daysBeforeCodeExpiration) //If the code is not used and it has not expired...
                    {
                        string codeString = row["Code"] as string;

                        codes.Add(codeString);
                    }

                }
            }

            return codes;
        }

        /// <summary>
        /// Adds a newly generated user code to the database.
        /// </summary>
        /// <param name="session">The session adding the user code.</param>
        /// <param name="code">The resulting code that was generated.</param>
        /// <returns>Returns true if the code was successfully added to the database. False otherwise.</returns>
        public static bool AddUserCode(HttpSessionState session, out string code)
        {
            User sessionUser = Help.GetUserSession(session);
            code = string.Empty;

            if (sessionUser != null && sessionUser.PermissionLevel >= permissionToAddUserCodes) //If the user exists and their permissions are high enough.
            {
                code = Help.GenerateUserCode();

                string sqlQuery = "INSERT INTO UserCodes (Code, Used, Date) VALUES " +
                        "(@Code, @Used, @Date)";

                SqlParameter codeParameter = new SqlParameter("@Code", SqlDbType.NVarChar, code.Length);
                codeParameter.Value = code;
                SqlParameter usedParameter = new SqlParameter("@Used", SqlDbType.Bit);
                usedParameter.Value = 0;

                string dateString = Help.GetDateString(DateTime.Now);
                SqlParameter dateParameter = new SqlParameter("@Date", SqlDbType.NVarChar, dateString.Length);
                dateParameter.Value = dateString;

                SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while adding a new user code.",
                    codeParameter, usedParameter, dateParameter);

                if (result != null) //If the result is not null, then the query succeeded and should be logged.
                {
                    Log(sessionUser, sessionUser.Username + " added a new user code with the value: " + code + ".");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Deactivates a user code in the database so it cannot be used again.
        /// </summary>
        /// <param name="code">The code to deactivate.</param>
        /// <param name="username">The username of the user who is deactivating this user code.</param>
        /// <returns>Returns true if the code was deactived, false otherwise.</returns>
        public static bool DeactivateUserCode(string username, string code, out UserCodeError error)
        {
            error = UserCodeError.None;
            if (UserCodeExists(code, out error)) //We must make sure the user code exists or we cannot deactivate it.
            {
                if (error == UserCodeError.None) //If the code has not been used and has not expired...
                {
                    string sqlQuery = "UPDATE UserCodes SET " +
                                "Used = @Used" +
                                " WHERE Code = @Code";

                    //Generating the parameters, this is done for sanitization reasons.
                    SqlParameter codeParameter = new SqlParameter("@Code", SqlDbType.NVarChar, code.Length);
                    codeParameter.Value = code;

                    SqlParameter usedParameter = new SqlParameter("@Used", SqlDbType.Bit);
                    usedParameter.Value = 1;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while deactivating a user code.",
                        codeParameter, usedParameter);

                    if (result != null)
                    {
                        Log(username, username + " has redeemed the user code " + code);

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Deactivates a user code in the database so it cannot be used again.
        /// </summary>
        /// <param name="code">The code to deactivate.</param>
        /// <returns>Returns true if the code was deactived, false otherwise.</returns>
        public static bool DeactivateUserCode(string code, out UserCodeError error)
        {
            error = UserCodeError.None;
            if (UserCodeExists(code, out error)) //We must make sure the user code exists or we cannot deactivate it.
            {
                if (error == UserCodeError.None) //If the code has not been used and has not expired...
                {
                    string sqlQuery = "UPDATE UserCodes SET " +
                                "Used = @Used" +
                                " WHERE Code = @Code";

                    //Generating the parameters, this is done for sanitization reasons.
                    SqlParameter codeParameter = new SqlParameter("@Code", SqlDbType.NVarChar, code.Length);
                    codeParameter.Value = code;

                    SqlParameter usedParameter = new SqlParameter("@Used", SqlDbType.Bit);
                    usedParameter.Value = 1;

                    SqlDataReader result = ExecuteSQL(GetConnectionStringUsersTable(), sqlQuery, "exception occured while deactivating a user code.",
                        codeParameter, usedParameter);

                    if (result != null)
                    {
                        Log("", "A user code has been anonymousely deactivated. The code was " + code);

                        return true;
                    }
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
        /// Reads the entire log file from start to finish then returns all of the lines as a seperate string.
        /// </summary>
        /// <returns>Returns an array of strings representing each line in the log file, or returns null if there was an error.</returns>
        public static string[] GetLog(HttpSessionState session)
        {
            User user = Help.GetUserSession(session);
            if (user != null && user.PermissionLevel >= permissionToViewLogs)
            {
                try
                {
                    if (fullLogPath == string.Empty) //there is no full path, so one must be constructed.
                    {
                        CreateLogPath();
                    }

                    return File.ReadAllLines(fullLogPath + logFileName + logFileType);
                }
                catch (Exception e)
                {
                    LogException(e, "exception occured while gathering the log."); //An error has occured and should be logged.
                }
            }
            return null;
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

                using (StreamWriter writer = new StreamWriter(fullLogPath + exceptionFileName, true))
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