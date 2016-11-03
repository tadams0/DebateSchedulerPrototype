using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateScheduler
{
    public partial class RefereeView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage)Master).SetPagePermissionLevel(2);

        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Makes the day unselectable and its back color "Ghost White" if it is not a Saturday.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void DayRender(Object source, DayRenderEventArgs e)
        {

            if (e.Day.Date.DayOfWeek != DayOfWeek.Saturday)
            {
                e.Day.IsSelectable = false;
                e.Cell.BackColor = System.Drawing.Color.GhostWhite;
            }
        }

        /// <summary>
        /// Shows Error messages on the ErrorLabel.
        /// </summary>
        /// <param name="messsage"></param>
        /// <param name="color"></param>
        private void ShowErrorInfo(string messsage, Color color)
        {
            ErrorLabel.ForeColor = color;
            ErrorLabel.Text = messsage;
            ErrorLabel.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {
            User user = Help.GetUserSession(Session);
            int debateId = int.Parse(DropDownList1.SelectedValue);
            int team1Score = int.Parse(DropDownList2.SelectedValue);
            int team2Score = int.Parse(DropDownList3.SelectedValue);

            Debate debate = DatabaseHandler.GetDebate(debateId);
            if(user.PermissionLevel == 3)
            {
                debate.Team1Score = team1Score;
                debate.Team2Score = team2Score;
                DatabaseHandler.UpdateDebate(Session, debate);
            }
            else if(user.PermissionLevel == 2)
            {
                if (debate.Team1Score == -1)
                {
                    debate.Team1Score = team1Score;
                    debate.Team2Score = team2Score;
                    DatabaseHandler.UpdateDebate(Session, debate);
                }
                else
                    ShowErrorInfo("The debate has already been scored.", System.Drawing.Color.Red);

            }
        }
    }
}