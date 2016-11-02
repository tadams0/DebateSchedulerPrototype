using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace DebateScheduler
{
    public partial class News : System.Web.UI.Page
    {
        private const int postsPerPage = 5;

        private int pageNumber = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            int page;
            bool result = int.TryParse(Request.QueryString["page"], out page);
            if (result)
            {
                pageNumber = page;
            }

            int startIndex = pageNumber * postsPerPage + 1;
            List<NewsPost> newsPosts = DatabaseHandler.GetNewsPosts(startIndex, startIndex + postsPerPage);

            if (newsPosts.Count > 0)
            {
                User sessionUser = Help.GetUserSession(Session);

                int pageCount = postsPerPage;
                if (pageCount > newsPosts.Count)
                    pageCount = newsPosts.Count;

                for (int i = 0; i < pageCount; i++)
                {
                    LiteralControl control = new LiteralControl();
                    control.Text = newsPosts[i].Data;
                    Panel resultPanel = CreateNewsPanel("");
                    //resultPanel.Style["Position"] = "Absolute";
                    resultPanel.Controls.AddAt(0, control);
                    resultPanel.Attributes.Add("style", "border:solid 1px black;");
                    resultPanel.BackColor = System.Drawing.Color.LightGray;
                    resultPanel.Controls.AddAt(1, new LiteralControl("<br /> <br />"));

                    //Creating the panel that contains the date.
                    Panel infoPanel = new Panel();
                    Label creatorDateLabel = new Label();
                    creatorDateLabel.Text = "Created by: " + newsPosts[i].Creator.Username.ToLower() + " on " + newsPosts[i].Date;
                    if (newsPosts[i].HasBeenUpdated)
                        creatorDateLabel.Text += " last updated on: " + newsPosts[i].LastUpdateDate;
                    infoPanel.Controls.Add(creatorDateLabel);
                    infoPanel.HorizontalAlign = HorizontalAlign.Right;

                    if (sessionUser != null && sessionUser.ID == newsPosts[i].Creator.ID) //If the creator and this user have the same ID then...
                    {
                        Button editButton = new Button();
                        editButton.Text = "Edit";
                        editButton.Command += EditButton_Command;
                        editButton.CommandArgument = newsPosts[i].ID.ToString();
                        infoPanel.Controls.Add(editButton);
                    }

                    resultPanel.Controls.Add(infoPanel);
                    Panel_News.Controls.AddAt(0, resultPanel);
                    Panel_News.Controls.AddAt(1, new LiteralControl("<br /> <br />"));
                }

                Panel nextPanel = new Panel();
                nextPanel.HorizontalAlign = HorizontalAlign.Right;

                if (pageNumber > 0)
                {
                    Button backButton = new Button();
                    backButton.Command += BackButton_Command;
                    backButton.Text = "Back";
                    nextPanel.Controls.Add(backButton);
                }

                if (newsPosts.Count > postsPerPage)
                {
                    Button nextButton = new Button();
                    nextButton.Command += NextButton_Command;
                    nextButton.Text = "Next";
                    nextPanel.Controls.Add(nextButton);
                }

                Panel_News.Controls.Add(nextPanel);

            }

        }

        private void BackButton_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("News.aspx?page=" + (pageNumber - 1));
        }

        private void NextButton_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("News.aspx?page=" + (pageNumber + 1));
        }

        private void EditButton_Command(object sender, CommandEventArgs e)
        {
            int postID;
            bool result = int.TryParse(e.CommandArgument as string, out postID);
            if (result)
            {
                Response.Redirect("AdminPanel.aspx?editID=" + postID);
            }
           
        }

        private Panel CreateNewsPanel(string title)
        {
            Label titleLabel = new Label();
            titleLabel.Text = title;

            Panel panel = new Panel();
            panel.Controls.Add(titleLabel);

            return panel;
        }

    }
}