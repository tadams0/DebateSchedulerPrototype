using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace DebateScheduler
{
    public partial class Register : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            DebateScheduler.User user = Help.GetUserSession(Session);
            if (user != null)
            {
                CreateUserWizard.Visible = false;
            }
            Label_UserCreated.Visible = false;
            Label_ErrorMessage.Visible = false;
        }

        protected void CreateUserWizard_CreatingUser(object sender, LoginCancelEventArgs e)
        {

            e.Cancel = true;

            if (CreateUserWizard.UserName.Contains('%'))
            {
                Label_ErrorMessage.Text = "Username cannot contain %";
                Label_ErrorMessage.Visible = true;
            }
            else
            {
                string ipAddress = Request.UserHostAddress;

                bool result = DatabaseHandler.AddUser(ipAddress, new User(0, CreateUserWizard.UserName, CreateUserWizard.Email, CreateUserWizard.Question, 0), CreateUserWizard.Password, CreateUserWizard.Answer);

                if (result)
                {
                    Label_UserCreated.Visible = true;
                    CreateUserWizard.Visible = false;
                    Label_ErrorMessage.Visible = false;
                }
                else
                {
                    Label_ErrorMessage.Text = "Username already exists.";
                    Label_ErrorMessage.Visible = true;
                }
            }


        }

        
    }
}