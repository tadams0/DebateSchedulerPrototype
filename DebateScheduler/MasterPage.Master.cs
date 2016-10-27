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
            User user = Help.GetUserSession(Session);
            if (user != null)
                FillLogout();
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            User newUser = DatabaseHandler.AuthenticateUsernamePassword(Login1.UserName, Login1.Password);
            if (newUser != null) //If the new user is not null then the login did not fail.
            {
                Help.AddUserSession(Session, newUser);
                
                FillLogout();
                if (newUser.PermissionLevel == 2)
                    Server.Transfer("RefereeView.aspx", true);
                else if (newUser.PermissionLevel == 3)
                    Server.Transfer("SuperView.aspx", true);
            }
            else
            {
                //Error occured logging in..
            }
        }

        private void FillLogout()
        {
            User user = Help.GetUserSession(Session);
            if (user != null)
            {
                Login1.Visible = false;
                Panel_logout.Visible = true;
                Label_Username.Text = user.Username;
                Label_Permissions.Text = Help.GetPermissionName(user.PermissionLevel);
            }
        }

        protected void Button_Logout_Click(object sender, EventArgs e)
        {
            Help.EndSession(Session);
            Panel_logout.Visible = false;
            Login1.Visible = true;           
        }
    }
}