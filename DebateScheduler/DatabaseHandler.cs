using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

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
        public static void AuthenticateUsernamePassword(string username, string password) //TODO: Send a session or something back.. I don't know, send data back adequete enough to do login.
        {
            username = username.ToUpperInvariant(); //The username is converted to the upper invariant (upper case) to prevent case sensitivity on usernames.

            SqlConnection connection = new SqlConnection(GetConnectionStringUsersTable());

            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Users WHERE Name LIKE '%" + username + "%'", connection))
                {
                    DataTable resultingTable = new DataTable();
                    adapter.Fill(resultingTable);
                    //if (resultingTable.)
                    if (resultingTable.Rows.Count > 0)
                    {
                        DataRow userRow = resultingTable.Rows[0]; //If there is more than 1 row, then we've got a problem because there's more than 1 matching username.
                        string matchedName = userRow["Name"] as string;
                        string matchedPassword = userRow["Password"] as string;
                        int matchedPermissions = (int)userRow["Permissions"];

                        if (matchedName == username && password == matchedPassword) //Compares the username and password to the username and password found in the database.
                        {
                            //Log the user in, as the username and password match.
                        }
                        else
                        {
                            //Username/Password did not match send message.
                        }

                    }
                    else
                    {
                        //The username does not exists.
                        //TODO: Send a prompt or message to the username letting them know either the username/password did not work.
                    }
                    
                }


            }
            catch (Exception e)
            {
                LogException(e, "exception occured while authenticating username/password.");
            }
            finally
            {
                connection.Close(); //The connection is guranteed a close within the finally code block.
            }
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
            //string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string assemblyPath = Environment.CurrentDirectory;
            string assemblyPath = GetAppDataPath();
            fullLogPath = assemblyPath += "\\" + logFolderPath + "\\";
        }

        /// <summary>
        /// Logs a message in the log file.
        /// </summary>
        /// <param name="sessionID">The session ID of the user who is editing the database.</param>
        /// <param name="message">The message to log.</param>
        private static void Log(string sessionID, string message)
        {
            if (fullLogPath == string.Empty) //there is no full path, so one must be constructed.
            {
                CreateLogPath();
            }

            string logMessage = "ID: " + sessionID + " Message: \"" + message + "\" Logged at " + DateTime.Now + " server time.";

            try
            {
                Directory.CreateDirectory(fullLogPath);

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
            catch (Exception exception)
            {
                //Well an exception occured, but it cannot be logged since this is the method for logging it. In this case we do nothing really.
            }

        }
        

    }
}