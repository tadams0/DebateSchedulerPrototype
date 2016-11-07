using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace DebateScheduler
{
    public partial class Register : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            DebateScheduler.User user = Help.GetUserSession(Session);
            if (user != null)
            {
                CreateUserWizard.Visible = false;
                Label_Title.Visible = false;
                Label_Code.Visible = false;
                TextBox1.Visible = false;
            }
            Label_UserCreated.Visible = false;
            Label_ErrorMessage.Visible = false;
        }

        /// <summary>
        /// Determines if a given string is a valid email destination.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>Returns true if the email is valid, false otherwise.</returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                MailAddress realAddress = new MailAddress(email);
                return realAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected void CreateUserWizard_CreatingUser(object sender, LoginCancelEventArgs e)
        {

            e.Cancel = true;

            if (CreateUserWizard.UserName.Contains('%'))
            {
                Label_ErrorMessage.Text = "Username cannot contain %";
                Label_ErrorMessage.Visible = true;
            }
            else
            {
                //Email verification:
                if (IsValidEmail(CreateUserWizard.Email))
                {
                    string ipAddress = Request.UserHostAddress;

                    bool addUserResult = DatabaseHandler.AddUser(ipAddress, new User(0, CreateUserWizard.UserName, CreateUserWizard.Email, CreateUserWizard.Question, 0), CreateUserWizard.Password, CreateUserWizard.Answer);

                    if (addUserResult)
                    {
                        string code = TextBox1.Text.Trim();
                        UserCodeError codeError;
                        bool result = DatabaseHandler.DeactivateUserCode(CreateUserWizard.UserName, code, out codeError);
                        if (result)
                        {
                            Label_UserCreated.Visible = true;
                            CreateUserWizard.Visible = false;
                            Label_ErrorMessage.Visible = false;
                            Label_Title.Visible = false;
                            Label_Code.Visible = false;
                            TextBox1.Visible = false;
                        }
                        else if (codeError == UserCodeError.CodeExpired)
                        {
                            Label_ErrorMessage.Text = "The given code has expired.";
                            Label_ErrorMessage.Visible = true;
                        }
                        else if (codeError == UserCodeError.CodeUsed)
                        {
                            Label_ErrorMessage.Text = "The given code has already been used.";
                            Label_ErrorMessage.Visible = true;
                        }
                        else if (codeError == UserCodeError.CodeDoesntExist)
                        {
                            Label_ErrorMessage.Text = "The given code does not exist.";
                            Label_ErrorMessage.Visible = true;
                        }
                    }
                    else
                    {
                        Label_ErrorMessage.Text = "Username already exists.";
                        Label_ErrorMessage.Visible = true;
                    }
                }
                else
                {
                    Label_ErrorMessage.Text = "Email is invalid.";
                    Label_ErrorMessage.Visible = true;
                }
                
            }
            
        }
        
    }
}