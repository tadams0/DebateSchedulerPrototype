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
        public int PermissionLevel { get; private set; } = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Setting up the server side backend for database related stuffs
            AppDomain.CurrentDomain.SetData("DataDirectory", DatabaseHandler.GetAppDataPath());
            User user = Help.GetUserSession(Session);
            if (user != null)
                FillLogout();

            if (!Page.IsPostBack)
                CheckPermissions(user);
        }

        public void SetPagePermissionLevel(int permissionLevel)
        {
            PermissionLevel = permissionLevel;
        }

        private void CheckPermissions(User user)
        {
            //If the user is not logged in and the permission level of the page is greator than 1...
            //Or if the user is logged in but their permission level is less than the page's permission level..
            if ((user == null && PermissionLevel > 1) || (user != null && user.PermissionLevel < PermissionLevel))
            {
                if (Request.Url.AbsolutePath.ToUpperInvariant() != "/News.aspx".ToUpperInvariant())
                    Response.Redirect("News.aspx");
            }
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            User newUser = DatabaseHandler.AuthenticateUsernamePassword(Login1.UserName, Login1.Password);
            if (newUser != null) //If the new user is not null then the login did not fail.
            {
                Help.AddUserSession(Session, newUser);
                
                FillLogout();
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
            CheckPermissions(null);       
        }
    }
}