using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateScheduler
{
    public partial class RecoverPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        protected void Button_Submit0_Click(object sender, EventArgs e)
        {
            string question = DatabaseHandler.GetUserSecurityQuestion(TextBox_Username.Text);
            if (question == string.Empty)
            {
                Label_UsernameWrong.Visible = true;
            }
            else
            {
                Panel_QA.Visible = true;
                TextBox_Question.Text = question;
                Panel_UsernameSubmition.Visible = false;
            }
        }

        protected void Button_SubmitAnswer_Click(object sender, EventArgs e)
        {
            bool correctAnswer = DatabaseHandler.AuthenticateSecurityQuestion(TextBox_Username.Text, TextBox_Answer.Text);

            if (correctAnswer)
            {
                Panel_QA.Visible = false;
                Panel_AnswerCorrect.Visible = true;

                //Email password
                string result = DatabaseHandler.EmailUserPassword(TextBox_Username.Text); //TODO: Have this method email the password, and do nothing else.
                Label_EmailSent.Text = "Your password is: " + result; //Remove this once email works.
            }
            else
            {
                Label_AnswerWrong.Visible = true;
            }

        }
    }
}