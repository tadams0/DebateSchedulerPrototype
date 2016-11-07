using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.HtmlControls;

namespace DebateScheduler
{
    public partial class AdminPanel : System.Web.UI.Page
    {
        private NewsPost editingPost = null;
        private Color codeColor = Color.ForestGreen;

        protected void Page_Load(object sender, EventArgs e)
        {
            ((MasterPage)Master).SetPagePermissionLevel(3);
            User loggedUser = Help.GetUserSession(Session);
            if (loggedUser != null && loggedUser.PermissionLevel >= 3)
            {
                string[] logLines = DatabaseHandler.GetLog(Session);

                Label_LogTitle.Text = "Logs as of " + DateTime.Now;

                if (logLines != null)
                {
                    foreach (string s in logLines)
                    {
                        Label l = new Label();
                        l.Text = s;
                        Panel_ViewLog.Controls.AddAt(2, l);
                        Panel_ViewLog.Controls.AddAt(2, new LiteralControl("<br /> <br />"));
                    }
                }

                List<string> activeCodes = DatabaseHandler.GetActiveCodes(Session);

                foreach (string s in activeCodes)
                {
                    Label codeLabel = new Label();
                    codeLabel.ForeColor = codeColor;
                    codeLabel.Text = s;
                    Panel_ActiveCodes.Controls.Add(codeLabel);
                    Panel_ActiveCodes.Controls.Add(new LiteralControl("<br />"));
                }

                //Slow loading this is a temporary solution
                //System.Threading.Thread.Sleep(1000);

                //Checking if data needs to be filled in...
                string editID = Request.QueryString["editID"];
                if (editID != null)
                {
                    int postID = int.Parse(editID);
                    NewsPost post = DatabaseHandler.GetNewsPost(postID);
                    if (post != null && post.Creator.ID == loggedUser.ID)
                    {
                        editingPost = post;
                        FreeTextBox1.Text = post.Data; //Server.HtmlDecode(post.Data);
                        FreeTextBox1.UpdateToolbar = true;
                    }
                }
            }
        }


        private void ShowRefereeMakerInfo(string messsage, Color color)
        {
            Label_RefereeMakerInfo.ForeColor = color;
            Label_RefereeMakerInfo.Text = messsage;
            Label_RefereeMakerInfo.Visible = true;
        }

        private void ShowNewsInfo(string message, Color color)
        {
            Label_NewsInfo.ForeColor = color;
            Label_NewsInfo.Text = message;
            Label_NewsInfo.Visible = true;
        }

        protected void Button_RefereeMaker_Click(object sender, EventArgs e)
        {
            User user = DatabaseHandler.GetUser(Session, TextBox_RefereeMaker.Text);
            if (user != null)
            {
                int currentPermissionLevel = user.PermissionLevel;
                int refereePermissionLevel = Help.GetPermissionLevel("Referee");
                if (currentPermissionLevel < refereePermissionLevel)
                {
                    user.PermissionLevel = refereePermissionLevel;

                    bool result = DatabaseHandler.UpdateUser(Session, user);
                    if (result)
                    {
                        ShowRefereeMakerInfo(TextBox_RefereeMaker.Text + " is now a referee.", Color.Green);
                    }
                    else
                    {
                        ShowRefereeMakerInfo("An error occured while making the user " + TextBox_RefereeMaker.Text + " a referee.", Color.Red);
                    }
                }
                else// if (currentPermissionLevel == refereePermissionLevel)
                {
                    ShowRefereeMakerInfo(TextBox_RefereeMaker.Text + " is already a referee!", Color.Red);
                }
            }
            else
            {
                ShowRefereeMakerInfo("User does not exist!", Color.Red);
            }

        }

        protected void Button_RevokeReferee_Click(object sender, EventArgs e)
        {
            User user = DatabaseHandler.GetUser(Session, TextBox_RefereeMaker.Text);

            if (user != null)
            {
                if (user.PermissionLevel <= Help.GetPermissionLevel("Referee"))
                {
                    user.PermissionLevel = 0;
                    bool result = DatabaseHandler.UpdateUser(Session, user);
                    if (result)
                    {
                        ShowRefereeMakerInfo(TextBox_RefereeMaker.Text + " is no longer a referee.", Color.Green);
                    }
                    else
                    {
                        ShowRefereeMakerInfo("An error occured while revoking referee status of the user " + TextBox_RefereeMaker.Text + ".", Color.Red);
                    }
                }
                else
                {
                    ShowRefereeMakerInfo("Cannot revoke super referees.", Color.Red);
                }
                
            }
            else
            {
                ShowRefereeMakerInfo("User does not exist!", Color.Red);
            }
        }

        protected void Button_SubmitNews_Click(object sender, EventArgs e)
        {
            User creator = Help.GetUserSession(Session);
            if (creator != null)
            {
                NewsPost newPost = new NewsPost(0, creator, DateTime.Now, "", FreeTextBox1.Text);
                bool result = DatabaseHandler.AddNewsPost(Session, newPost);
                if (result)
                {
                    ShowNewsInfo("A new news post was created!", Color.Green);
                }
                else
                {
                    ShowNewsInfo("Error occured while trying to create a news post.", Color.Red);
                }
            }

        }

        protected void Button_RemoveNews_Click(object sender, EventArgs e)
        {
            User loggedUser = Help.GetUserSession(Session);

            if (loggedUser != null && editingPost != null && loggedUser.ID == editingPost.Creator.ID)
            { //If there is a logged in user, and a post being editted, and the person logged in created the post...
                bool result = DatabaseHandler.RemoveNewsPost(Session, editingPost.ID);
                if (result)
                {
                    ShowNewsInfo("News post has been permanently removed.", Color.Red);
                }
                else
                {
                    ShowNewsInfo("An error occured while removing the news post, it was not removed.", Color.Red);
                }
            }
            else
            {
                if (editingPost == null)
                {
                    ShowNewsInfo("This is not a post yet, press Submit New instead.", Color.Red);
                }
            }
        }

        protected void Button_UpdateNews_Click(object sender, EventArgs e)
        {
            User loggedUser = Help.GetUserSession(Session);

            if (loggedUser != null && editingPost != null && loggedUser.ID == editingPost.Creator.ID)
            { //If there is a logged in user, and a post being editted, and the person logged in created the post...
                editingPost.Data = FreeTextBox1.ViewStateText;
                bool result = DatabaseHandler.UpdateNewsPost(Session, editingPost);
                if (result)
                {
                    ShowNewsInfo("News post successfully updated!", Color.Green);
                }
                else
                {
                    ShowNewsInfo("Error occured while trying to update the news post.", Color.Red);
                }
            }
            else
            {
                if (editingPost == null)
                {
                    ShowNewsInfo("This is not a post yet, press Submit New instead.", Color.Red);
                }
            }
        }

        protected void Button_GenerateCode_Click(object sender, EventArgs e)
        {
            string code;
            bool result = DatabaseHandler.AddUserCode(Session, out code);
            if (result)
            {
                Label_CodeResult.Visible = true;
                Label_CodeResult.Text = "Your new code is: " + code;
                Label_CodeResult.ForeColor = Color.Green;
            }
            else
            {
                Label_CodeResult.Visible = true;
                Label_CodeResult.Text = "There has been an error generating a new code.";
                Label_CodeResult.ForeColor = Color.Red;
            }
        }
    }
}