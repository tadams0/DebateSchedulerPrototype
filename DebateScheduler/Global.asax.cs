using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace DebateScheduler
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            bool activeSeason = false;
            int mostRecentSeasonID = DatabaseHandler.GetMostRecentSeasonID(out activeSeason);
            if (activeSeason)
                Help.SetDebateID(Application, mostRecentSeasonID);

        }
    }
}