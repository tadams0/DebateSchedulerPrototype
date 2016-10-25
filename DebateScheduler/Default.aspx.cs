using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateScheduler
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button_Test1_Click(object sender, EventArgs e)
        {
            DatabaseHandler.AddUser(Session, new DebateScheduler.User(2, "NewRefDude", 0), "123", "test@Test.com");
        }

        protected void Button_Test2_Click(object sender, EventArgs e)
        {
            DatabaseHandler.RemoveUser(Session, "NewRefDude");
        }
    }
}