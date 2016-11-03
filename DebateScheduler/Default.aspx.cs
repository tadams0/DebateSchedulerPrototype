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
        private const int statsCellWidth = 80;
        private const int dateCellWidth = 150;
        private const int vsCellWidth = 30;

        private bool includeVs = true;

        private readonly Color headerTableColor = Color.CornflowerBlue;
        private readonly Color headerTableTextColor = Color.White;

        private OrderBy order = OrderBy.Ascending;
        private DebateOrderVar dOrder = DebateOrderVar.Date;

        private List<Debate> debates = new List<Debate>();

        protected void Page_Load(object sender, EventArgs e)
        {
            //Gathering query values
            NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            string orderString = queryValues.Get("Order");
            string debateOrderString = queryValues.Get("dOrder");

            if (orderString != null)
            {
                order = (OrderBy)(int.Parse(orderString));
            }
            if (debateOrderString != null)
            {
                dOrder = (DebateOrderVar)(int.Parse(debateOrderString));
            }

            int currentDebateID = Help.GetDebateSeasonID(Application);
            debates = DatabaseHandler.GetDebateSeasonDebates(currentDebateID);

            debates = Help.OrderDebates(order, dOrder, debates);

            TableRow header = CreateHeaderRow();
            Table1.Rows.Add(header);

            foreach (Debate d in debates)
            {
                TableRow debateRow = CreateDebateRow(d);
                Table1.Rows.Add(debateRow);

            }


        }

        private TableRow CreateHeaderRow()
        {
            TableRow row = new TableRow();

            TableCell team1Cell = new TableCell();
            TableCell team2Cell = new TableCell();
            TableCell team1ScoreCell = new TableCell();
            TableCell team2ScoreCell = new TableCell();
            TableCell dateCell = new TableCell();
            TableCell morningCell = new TableCell();
            TableCell vsCell = new TableCell();
            
            team1Cell.BackColor = headerTableColor;
            team1Cell.Width = nameCellWidth;
            team1Cell.HorizontalAlign = HorizontalAlign.Center;

            team2Cell.BackColor = headerTableColor;
            team2Cell.Width = nameCellWidth;
            team2Cell.HorizontalAlign = HorizontalAlign.Center;

            team1ScoreCell.BackColor = headerTableColor;
            team1ScoreCell.Width = statsCellWidth;

            team2ScoreCell.BackColor = headerTableColor;
            team2ScoreCell.Width = statsCellWidth;

            dateCell.BackColor = headerTableColor;
            dateCell.Width = dateCellWidth;

            morningCell.BackColor = headerTableColor;
            morningCell.Width = dateCellWidth;

            vsCell.BackColor = headerTableColor;
            vsCell.Width = vsCellWidth;
            vsCell.HorizontalAlign = HorizontalAlign.Center;

            LinkButton team1Button = new LinkButton();
            team1Button.Command += Team1Button_Command;
            team1Button.ForeColor = headerTableTextColor;
            team1Button.Text = "Team 1";

            LinkButton team2Button = new LinkButton();
            team2Button.Command += Team2Button_Command;
            team2Button.ForeColor = headerTableTextColor;
            team2Button.Text = "Team 2";

            LinkButton team1ScoreButton = new LinkButton();
            team1ScoreButton.Command += Team1ScoreButton_Command;
            team1ScoreButton.ForeColor = headerTableTextColor;
            team1ScoreButton.Text = "Team 1 Score";

            LinkButton team2ScoreButton = new LinkButton();
            team2ScoreButton.Command += Team2ScoreButton_Command;
            team2ScoreButton.ForeColor = headerTableTextColor;
            team2ScoreButton.Text = "Team 2 Score";

            LinkButton dateButton = new LinkButton();
            dateButton.Command += DateButton_Command;
            dateButton.ForeColor = headerTableTextColor;
            dateButton.Text = "Date";

            LinkButton morningButton = new LinkButton();
            morningButton.Command += MorningButton_Command;
            morningButton.ForeColor = headerTableTextColor;
            morningButton.Text = "Time";

            vsCell.ForeColor = headerTableColor;
            vsCell.Text = "Versus";

            team1Cell.Controls.Add(team1Button);
            team2Cell.Controls.Add(team2Button);
            team1ScoreCell.Controls.Add(team1ScoreButton);
            team2ScoreCell.Controls.Add(team2ScoreButton);
            dateCell.Controls.Add(dateButton);
            morningCell.Controls.Add(morningButton);

            row.Cells.Add(team1Cell);
            if (includeVs)
                row.Cells.Add(vsCell);
            row.Cells.Add(team2Cell);
            row.Cells.Add(team1ScoreCell);
            row.Cells.Add(team2ScoreCell);
            row.Cells.Add(dateCell);
            row.Cells.Add(morningCell);
            
            return row;
        }

        private TableRow CreateDebateRow(Debate d)
        {
            TableRow row = new TableRow();

            TableCell team1Cell = new TableCell();
            TableCell team2Cell = new TableCell();
            TableCell team1ScoreCell = new TableCell();
            TableCell team2ScoreCell = new TableCell();
            TableCell dateCell = new TableCell();
            TableCell morningCell = new TableCell();
            TableCell vsCell = new TableCell();

            team1Cell.Width = nameCellWidth;
            team1Cell.HorizontalAlign = HorizontalAlign.Center;
            team2Cell.Width = nameCellWidth;
            team2Cell.HorizontalAlign = HorizontalAlign.Center;
            team1ScoreCell.Width = statsCellWidth;
            team2ScoreCell.Width = statsCellWidth;
            dateCell.Width = dateCellWidth;
            morningCell.Width = dateCellWidth;
            vsCell.Width = vsCellWidth;
            vsCell.HorizontalAlign = HorizontalAlign.Center;

            team1Cell.Text = d.Team1.Name;
            team2Cell.Text = d.Team2.Name;
            team1ScoreCell.Text = d.Team1Score.ToString();
            team2ScoreCell.Text = d.Team2Score.ToString();
            dateCell.Text = d.Date.ToString("MM/dd/yy");

            vsCell.Text = "vs";

            if (d.MorningDebate)
                morningCell.Text = "Morning";
            else
                morningCell.Text = "Afternoon";

            row.Cells.Add(team1Cell);
            if (includeVs)
                row.Cells.Add(vsCell);
            row.Cells.Add(team2Cell);
            row.Cells.Add(team1ScoreCell);
            row.Cells.Add(team2ScoreCell);
            row.Cells.Add(dateCell);
            row.Cells.Add(morningCell);
            

            return row;
        }

        private void MorningButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.MorningDebate);
        }

        private void DateButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.Date);
        }

        private void Team2ScoreButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.Team2Score);
        }

        private void Team1ScoreButton_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.Team1Score);
        }

        private void Team2Button_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.Team2Name);
        }

        private void Team1Button_Command(object sender, CommandEventArgs e)
        {
            RedirectWithParameters(DebateOrderVar.Team1Name);
        }

        private void RedirectWithParameters(DebateOrderVar debateOrderVar)
        {
            NameValueCollection queryValues = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            if (dOrder == debateOrderVar && order != OrderBy.Descending) //If team order is already by name, then we flip them.
            {
                queryValues.Set("dOrder", ((int)debateOrderVar).ToString());
                queryValues.Set("Order", ((int)OrderBy.Descending).ToString());
            }
            else
            {
                queryValues.Set("dOrder", ((int)debateOrderVar).ToString());
                queryValues.Set("Order", ((int)OrderBy.Ascending).ToString());
            }

            string url = Request.Url.AbsolutePath;
            Response.Redirect(url + "?" + queryValues);
        }
        

    }
}