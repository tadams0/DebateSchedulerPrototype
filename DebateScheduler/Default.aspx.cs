using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Collections.Specialized;

namespace DebateScheduler
{

    public partial class Default : System.Web.UI.Page
    {

        private const int nameCellWidth = 250;
        private const int statsCellWidth = 90;

        private readonly Color headerTableColor = Color.CornflowerBlue;

        private OrderBy order = OrderBy.Ascending;
        private TeamOrderVar teamOrder = TeamOrderVar.Name;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Gathering query values
            NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            string orderString = queryValues.Get("Order");
            string teamOrderString = queryValues.Get("TeamOrder");

            if (orderString != null)
            {
                order = (OrderBy)(int.Parse(orderString));
            }
            if (teamOrderString != null)
            {
                teamOrder = (TeamOrderVar)(int.Parse(teamOrderString));
            }

            int currentDebateID = Help.GetDebateSeasonID(Application);
            List<Team> teams = DatabaseHandler.GetDebateSeasonTeams(currentDebateID);

            teams = Help.OrderTeams(order, teamOrder, teams);

            TableRow header = CreateHeaderRow();
            Table1.Rows.Add(header);

            foreach (Team t in teams)
            {
                TableRow teamRow = CreateTeamRow(t);
                Table1.Rows.Add(teamRow);
            }


        }

        private TableRow CreateHeaderRow()
        {
            TableRow row = new TableRow();

            TableCell nameCell = new TableCell();
            TableCell winCell = new TableCell();
            TableCell lossCell = new TableCell();
            TableCell tieCell = new TableCell();
            TableCell totalScore = new TableCell();

            nameCell.BackColor = headerTableColor;
            winCell.BackColor = headerTableColor;
            lossCell.BackColor = headerTableColor;
            tieCell.BackColor = headerTableColor;
            totalScore.BackColor = headerTableColor;

            LinkButton nameButton = new LinkButton();
            nameButton.Command += NameButton_Command;
            nameButton.Text = "Name";

            LinkButton winButton = new LinkButton();
            winButton.Command += WinButton_Command;
            winButton.Text = "Wins";

            LinkButton lossButton = new LinkButton();
            lossButton.Command += LossButton_Command;
            lossButton.Text = "Losses";

            LinkButton tieButton = new LinkButton();
            tieButton.Command += TieButton_Command;
            tieButton.Text = "Ties";

            LinkButton totalScoreButton = new LinkButton();
            totalScoreButton.Command += TotalScoreButton_Command;
            totalScoreButton.Text = "Total Score";

            nameCell.Controls.Add(nameButton);
            winCell.Controls.Add(winButton);
            lossCell.Controls.Add(lossButton);
            tieCell.Controls.Add(tieButton);
            totalScore.Controls.Add(totalScoreButton);

            row.Cells.Add(nameCell);
            row.Cells.Add(winCell);
            row.Cells.Add(lossCell);
            row.Cells.Add(tieCell);
            row.Cells.Add(totalScore);

            return row;
        }

        private void TotalScoreButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.TotalScore);
        }

        private void TieButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.Ties);
        }

        private void LossButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.Losses);
        }

        private void WinButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.Wins);
        }

        private void NameButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(TeamOrderVar.Name);
        }

        private void RedirectWithParameters(TeamOrderVar teamOrderVar)
        {
            NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            if (teamOrder == teamOrderVar && order != OrderBy.Descending) //If team order is already by name, then we flip them.
            {
                queryValues.Set("OrderTeams", ((int)teamOrderVar).ToString());
                queryValues.Set("Order", ((int)OrderBy.Descending).ToString());
            }
            else
            {
                queryValues.Set("OrderTeams", ((int)teamOrderVar).ToString());
                queryValues.Set("Order", ((int)OrderBy.Ascending).ToString());
            }

            string url = Request.Url.AbsolutePath;
            Response.Redirect(url + "?" + queryValues);
        }

        private TableRow CreateTeamRow(Team t)
        {
            TableRow row = new TableRow();

            TableCell nameCell = new TableCell();
            TableCell winCell = new TableCell();
            TableCell lossCell = new TableCell();
            TableCell tieCell = new TableCell();
            TableCell totalScore = new TableCell();

            nameCell.Width = nameCellWidth;
            winCell.Width = statsCellWidth;
            lossCell.Width = statsCellWidth;
            tieCell.Width = statsCellWidth;
            totalScore.Width = statsCellWidth;

            nameCell.Text = t.Name;
            winCell.Text = t.Wins.ToString();
            lossCell.Text = t.Losses.ToString();
            tieCell.Text = t.Ties.ToString();
            totalScore.Text = t.TotalScore.ToString();

            row.Cells.Add(nameCell);
            row.Cells.Add(winCell);
            row.Cells.Add(lossCell);
            row.Cells.Add(tieCell);
            row.Cells.Add(totalScore);

            return row;
        }
        
    }
}