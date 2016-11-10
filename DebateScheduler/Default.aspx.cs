using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Collections.Specialized;
using System.Globalization;

namespace DebateScheduler
{

    public partial class Default : System.Web.UI.Page
    {

        private const int nameCellWidth = 250;
        private const int statsCellWidth = 120;
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
            ((MasterPage)Master).SetPagePermissionLevel(2);
            User loggedUser = Help.GetUserSession(Session);
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

            int rowNum = 1; // row 0 will be the header row.
            foreach (Debate d in debates)
            {
                if (loggedUser.PermissionLevel == 2)
                {
                    if (d.Team1Score == -1 && d.Team2Score == -1)
                    {
                        TableRow debateRow = CreateDebateRow(d,rowNum);
                        Table1.Rows.Add(debateRow);
                        rowNum++;
                    }
                }
                else if (loggedUser.PermissionLevel == 3)
                {
                    TableRow debateRow = CreateDebateRow(d,rowNum);
                    Table1.Rows.Add(debateRow);
                    rowNum++;
                }
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
            TableCell idCell = new TableCell();

            team1Cell.BackColor = headerTableColor;
            team1Cell.Width = nameCellWidth;
            team1Cell.HorizontalAlign = HorizontalAlign.Center;
            team1Cell.Text = "Team 1";

            team2Cell.BackColor = headerTableColor;
            team2Cell.Width = nameCellWidth;
            team2Cell.HorizontalAlign = HorizontalAlign.Center;
            team2Cell.Text = "Team 2";

            team1ScoreCell.BackColor = headerTableColor;
            team1ScoreCell.Width = statsCellWidth;
            team1ScoreCell.Text = "Team 1 Score";

            team2ScoreCell.BackColor = headerTableColor;
            team2ScoreCell.Width = statsCellWidth;
            team2ScoreCell.Text = "Team 2 Score";

            dateCell.BackColor = headerTableColor;
            dateCell.Width = dateCellWidth;
            dateCell.Text = "Date";

            morningCell.BackColor = headerTableColor;
            morningCell.Width = dateCellWidth;
            morningCell.Text = "Time";

            vsCell.BackColor = headerTableColor;
            vsCell.Width = vsCellWidth;
            vsCell.HorizontalAlign = HorizontalAlign.Center;
            vsCell.Text = "vs.";

            idCell.BackColor = headerTableColor;
            idCell.Width = statsCellWidth;
            idCell.HorizontalAlign = HorizontalAlign.Center;
            idCell.Text = "ID";

            row.Cells.Add(team1Cell);
            if (includeVs)
                row.Cells.Add(vsCell);
            row.Cells.Add(team2Cell);
            row.Cells.Add(team1ScoreCell);
            row.Cells.Add(team2ScoreCell);
            row.Cells.Add(dateCell);
            row.Cells.Add(morningCell);
            row.Cells.Add(idCell);

            return row;
        }

        private TableRow CreateDebateRow(Debate d,int rowNum)
        {
            TableRow row = new TableRow();

            TableCell team1Cell = new TableCell();
            TableCell team2Cell = new TableCell();
            TableCell team1ScoreCell = new TableCell();
            TableCell team2ScoreCell = new TableCell();
            TableCell dateCell = new TableCell();
            TableCell morningCell = new TableCell();
            TableCell vsCell = new TableCell();
            TableCell idCell = new TableCell();

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
            idCell.Width = statsCellWidth;
            idCell.HorizontalAlign = HorizontalAlign.Center;

            team1Cell.Text = d.Team1.Name;
            team2Cell.Text = d.Team2.Name;
            DropDownList ddl = new DropDownList();
            ddl.ID = "ddl" + rowNum;
            ddl.Items.Add(new ListItem("", "-1")); //Unscored value
            ddl.Items.Add(new ListItem("Forfeit", "-99")); // add list items
            ddl.Items.Add(new ListItem("0", "0"));
            ddl.Items.Add(new ListItem("1", "1"));
            ddl.Items.Add(new ListItem("2", "2"));
            ddl.Items.Add(new ListItem("3", "3"));
            ddl.Items.Add(new ListItem("4", "4"));
            ddl.Items.Add(new ListItem("5", "5"));
            if (d.Team1Score == -1)
                ddl.SelectedIndex = 0;
            else if (d.Team1Score == -99)
                ddl.SelectedIndex = 1;
            else
                ddl.SelectedIndex = d.Team1Score + 2; //The + 2 is because of the 2 extra index items in ddl
            team1ScoreCell.Controls.Add(ddl);

            DropDownList ddl1 = new DropDownList();
            ddl1.ID = "ddl#" + rowNum;
            ddl1.Items.Add(new ListItem("", "-1")); //Unscored value
            ddl1.Items.Add(new ListItem("Forfeit", "-99")); // add list items
            ddl1.Items.Add(new ListItem("0", "0"));
            ddl1.Items.Add(new ListItem("1", "1"));
            ddl1.Items.Add(new ListItem("2", "2"));
            ddl1.Items.Add(new ListItem("3", "3"));
            ddl1.Items.Add(new ListItem("4", "4"));
            ddl1.Items.Add(new ListItem("5", "5"));
            if (d.Team2Score == -1)
                ddl1.SelectedIndex = 0;
            else if (d.Team2Score == -99)
                ddl1.SelectedIndex = 1;
            else
                ddl1.SelectedIndex = d.Team2Score + 2; //The + 2 is because of the 2 extra index items in ddl
            team2ScoreCell.Controls.Add(ddl1);
            dateCell.Text = d.Date.ToString("MM/dd/yy");

            vsCell.Text = "vs";

            if (d.MorningDebate)
                morningCell.Text = "Morning";
            else
                morningCell.Text = "Afternoon";

            idCell.Text = "" + d.ID;

            row.Cells.Add(team1Cell);
            if (includeVs)
                row.Cells.Add(vsCell);
            row.Cells.Add(team2Cell);
            row.Cells.Add(team1ScoreCell);
            row.Cells.Add(team2ScoreCell);
            row.Cells.Add(dateCell);
            row.Cells.Add(morningCell);
            row.Cells.Add(idCell);


            return row;
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            User loggedUser = Help.GetUserSession(Session);
            for (int rowNum = 1; rowNum < Table1.Rows.Count; rowNum++) //Starts at row 1 since row 0 is header row.
            {
                int id;
                bool success = int.TryParse(Table1.Rows[rowNum].Cells[7].Text, NumberStyles.Any, CultureInfo.InvariantCulture, out id);
                Debate debate = DatabaseHandler.GetDebate(id);
                DropDownList TeamScore1Control = Table1.Rows[rowNum].Cells[3].FindControl("ddl" + rowNum) as DropDownList;
                DropDownList TeamScore2Control = Table1.Rows[rowNum].Cells[4].FindControl("ddl#" + rowNum) as DropDownList;
                debate.Team1Score = Int32.Parse(TeamScore1Control.SelectedValue);
                debate.Team2Score = Int32.Parse(TeamScore2Control.SelectedValue);
                bool result = DatabaseHandler.UpdateDebate(Session, debate);                
            }
            if(loggedUser.PermissionLevel == 2)
                Response.Redirect(Request.RawUrl);
            else
            {
                //Do nothing because you should be the Super.
            }
            
        }
    }
}