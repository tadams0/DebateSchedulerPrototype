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
        protected void Page_Load(object sender, EventArgs e)
        {

            List<NewsPost> newsPosts = DatabaseHandler.GetNewsPosts(0, 100);

            if (newsPosts.Count > 0)
            {
                User sessionUser = Help.GetUserSession(Session);
                
                foreach (NewsPost post in newsPosts)
                {
                    LiteralControl control = new LiteralControl();
                    control.Text = post.Data;
                    Panel resultPanel = CreateNewsPanel("");
                    //resultPanel.Style["Position"] = "Absolute";
                    resultPanel.Controls.AddAt(0, control);
                    resultPanel.Attributes.Add("style", "border:solid 1px black;");
                    resultPanel.BackColor = System.Drawing.Color.LightGray;
                    resultPanel.Controls.AddAt(1, new LiteralControl("<br /> <br />"));

                    //Creating the panel that contains the date.
                    Panel infoPanel = new Panel();
                    Label creatorDateLabel = new Label();
                    creatorDateLabel.Text = "Created by: " + post.Creator.Username.ToLower() + " on " + post.Date;
                    if (post.HasBeenUpdated)
                        creatorDateLabel.Text += " last updated on: " + post.LastUpdateDate;
                    infoPanel.Controls.Add(creatorDateLabel);
                    infoPanel.HorizontalAlign = HorizontalAlign.Right;

                    if (sessionUser != null && sessionUser.ID == post.Creator.ID) //If the creator and this user have the same ID then...
                    {
                        Button editButton = new Button();
                        editButton.Text = "Edit";
                        editButton.Command += EditButton_Command; 
                        editButton.CommandArgument = post.ID.ToString();
                        infoPanel.Controls.Add(editButton);
                    }
                    
                    resultPanel.Controls.Add(infoPanel);
                    Panel_News.Controls.AddAt(0, resultPanel);
                    Panel_News.Controls.AddAt(1, new LiteralControl("<br /> <br />"));
                }
            }

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