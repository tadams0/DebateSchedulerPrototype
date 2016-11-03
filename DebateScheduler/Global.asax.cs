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
            DebateSeason mostRecentSeason = DatabaseHandler.GetMostRecentSeason();
            if (!mostRecentSeason.HasEnded)
                Help.SetDebateID(Application, mostRecentSeason.ID);

        }
    }
}