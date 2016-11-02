using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateScheduler
{
    public partial class DebateCreator : System.Web.UI.Page
    {
        private int currentTeam = 2;
        private bool nameError = false;

        List<TextBox> textBoxes = new List<TextBox>();
        List<Label> infoLabels = new List<Label>();

        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage)Master).SetPagePermissionLevel(3);


            string teams = Request.QueryString["teams"];
            int numbTeams = -1;
            bool result = int.TryParse(teams, out numbTeams);

            if (numbTeams <= 2)
            {
                numbTeams = 2;
                Button_Remove.Visible = false;
            }
            if (numbTeams >= Help.GetMaximumTeams())
            {
                numbTeams = Help.GetMaximumTeams();
                Button_AddTeam.Visible = false;
            }

            currentTeam = numbTeams;
            for (int i = 1; i <= currentTeam; i++)
            {
                AddTeamButton(i);
            }
        }

        protected void Calendar_Start_DayRender(object sender, DayRenderEventArgs e)
        {
            if (e.Day.Date.DayOfWeek != DayOfWeek.Saturday)
            {
                e.Day.IsSelectable = false;
                e.Cell.Enabled = false;
                e.Cell.BackColor = Color.Gray;
            }
        }

        protected void Calendar_End_DayRender(object sender, DayRenderEventArgs e)
        {
            if (e.Day.Date.DayOfWeek != DayOfWeek.Saturday)
            {
                e.Day.IsSelectable = false;
                e.Cell.Enabled = false;
                e.Cell.BackColor = Color.Gray;
            }
        }

        private void AddTeamButton(int teamNumber)
        {
            Label label = new Label();
            label.Text = "Team " + teamNumber + ": ";
            TextBox textBox = new TextBox();
            textBox.Width = 225;
            Label infoLabel = new Label();
            infoLabel.Text = "Team names cannot repeat!";
            infoLabel.ForeColor = Color.Red;
            infoLabel.Visible = false;
            Panel_Teams.Controls.AddAt(Panel_Teams.Controls.Count - 5, label);
            Panel_Teams.Controls.AddAt(Panel_Teams.Controls.Count - 5, textBox);
            Panel_Teams.Controls.AddAt(Panel_Teams.Controls.Count - 5, infoLabel);
            Panel_Teams.Controls.AddAt(Panel_Teams.Controls.Count - 5, new LiteralControl("<br />"));

            textBoxes.Add(textBox);
            infoLabels.Add(infoLabel);
        }

        private List<Team> GetTeams()
        {
            List<Team> teams = new List<Team>();

            for (int i = 0; i < textBoxes.Count; i++)
            {
                string teamName = textBoxes[i].Text;
                Team newTeam = new Team(teamName, 0, 0, 0, 0, 0);
                teams.Add(newTeam);

                if (string.IsNullOrWhiteSpace(teamName))
                {
                    ShowTeamInfoLabel("Invalid team name. A team name cannot be empty or just spaces.", Color.Red, i );
                    nameError = true;
                }
                else if (teamName.Length > Help.GetMaximumTeamNameSize())
                {
                    ShowTeamInfoLabel("Invalid team name. The team name is too long.", Color.Red, i);
                    nameError = true;
                }
                else if (teamName.Length < Help.GetMinimumTeamNameSize())
                {
                    ShowTeamInfoLabel("Invalid team name. The team name is too short, team names contain at least " + Help.GetMinimumTeamNameSize() + " characters.", Color.Red, i);
                    nameError = true;
                }
                else
                {
                    infoLabels[i].Visible = false;
                }
            }

            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    if (teams[i].Name.ToUpperInvariant() == teams[j].Name.ToUpperInvariant())
                    {
                        nameError = true;
                        if (!infoLabels[i].Visible)
                            ShowTeamInfoLabel("This team name conflicts with another.", Color.Red, i);
                        if (!infoLabels[j].Visible)
                            ShowTeamInfoLabel("This team name conflicts with another.", Color.Red, j);
                    }
                }
            }

            return teams;
        }

        private void ShowTeamInfoLabel(string message, Color color, int controlIndex)
        {
            Label label = infoLabels[controlIndex];
            label.Text = message;
            label.ForeColor = color;
            label.Visible = true;

        }

        protected void Button_AddTeam_Click(object sender, EventArgs e)
        {
            currentTeam++;
            Response.Redirect("DebateCreator.aspx?teams=" + currentTeam);
        }

        protected void Button_RemoveTeam_Click(object sender, EventArgs e)
        {
            currentTeam--;
            Response.Redirect("DebateCreator.aspx?teams=" + currentTeam);
        }

        protected void Button_CreateSchedule_Click(object sender, EventArgs e)
        {
            bool errorOccured = false;

            List<Team> teams = GetTeams();
            if (nameError)
            {
                Label_ScheduleError.Text = "There are errors with the info given. Some team names are invalid.";
                Label_ScheduleError.Visible = true;
                errorOccured = true;
            }

            if (Calendar_Start.SelectedDates.Count <= 0 && Calendar_End.SelectedDates.Count <= 0)
            {
                Label_ScheduleError.Text = "There are errors with the info given. There is no start or end date specified.";
                Label_ScheduleError.Visible = true;
                errorOccured = true;
            }
            else if (Calendar_Start.SelectedDates.Count <= 0)
            {
                Label_ScheduleError.Text = "There are errors with the info given. There is no start date specified.";
                Label_ScheduleError.Visible = true;
                errorOccured = true;
            }
            else if (Calendar_End.SelectedDates.Count <= 0)
            {
                Label_ScheduleError.Text = "There are errors with the info given. There is no end date specified.";
                Label_ScheduleError.Visible = true;
                errorOccured = true;
            }

            if (!errorOccured)
            {
                Label_ScheduleError.Visible = false;
                //Generate schedule:
                DateTime startDate = Calendar_Start.SelectedDate;
                DateTime endDate = Calendar_End.SelectedDate;
                if (startDate > endDate)
                {
                    DateTime temp = endDate;
                    endDate = startDate;
                    startDate = temp;
                }

                //Adding the teams to the database
                foreach (Team t in teams)
                {
                    DatabaseHandler.AddTeam(Session, t);
                }

                //Creating the actual debates
                List<DateTime> saturdays = Help.SatBetween(startDate, endDate);
                List<TeamPair> pairs = Help.MatchMake(saturdays, teams);
                
                DatabaseHandler.ClearDebates(Session);
                foreach (TeamPair p in pairs)
                {
                    Debate debate = p as Debate;
                    if (p != null)
                    {
                        DatabaseHandler.AddDebate(Session, p);
                    }
                }
            }
        }


    }
}