using System;
using System.Collections.Generic;
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
        /// Creates the log path that the log files are saved to.
        /// </summary>
        private static void CreateLogPath()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string assemblyPath = Environment.CurrentDirectory;
            fullLogPath = assemblyPath += "\\" + logFolderPath + "\\";
        }

        /// <summary>
        /// Logs a message in the log file.
        /// </summary>
        /// <param name="sessionID">The session ID of the user who is editing the database.</param>
        /// <param name="message">The message to log.</param>
        public static void Log(string sessionID, string message)
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
                //An error has occured and should be logged.. except this is the log method!
            }

        }

        

    }
}