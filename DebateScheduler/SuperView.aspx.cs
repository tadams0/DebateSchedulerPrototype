using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace DebateScheduler
{
    public partial class SuperView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage)Master).SetPagePermissionLevel(3);
        }

        /// <summary>
        /// Gets Debate Id, Score1, and Score2 from the DropDownBoxes and updates the debate with the correct Id.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {            
            int DebateID = Int32.Parse(DropDownList3.SelectedValue);
            int Score1 = Int32.Parse(DropDownList1.SelectedValue);
            int Score2 = Int32.Parse(DropDownList2.SelectedValue);
            Debate debate = DatabaseHandler.GetDebate(DebateID);
            debate.Team1Score = Score1;
            debate.Team2Score = Score2;

            DatabaseHandler.UpdateDebate(Session, debate);
        }
    }
}