using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateScheduler
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Setting up the server side backend for database related stuffs
            AppDomain.CurrentDomain.SetData("DataDirectory", DatabaseHandler.GetAppDataPath());

        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            DatabaseHandler.AuthenticateUsernamePassword(Login1.UserName, Login1.Password);
        }
    }
}