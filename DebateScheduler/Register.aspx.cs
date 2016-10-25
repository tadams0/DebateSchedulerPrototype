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

        }

        protected void CreateUserWizard_CreatedUser(object sender, EventArgs e)
        {
            string ipAddress = Request.UserHostAddress;

            DatabaseHandler.AddUser(ipAddress, new DebateScheduler.User(0, CreateUserWizard.UserName, 0), CreateUserWizard.Password, CreateUserWizard.Email);

        }
    }
}