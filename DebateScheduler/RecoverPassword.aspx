<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="RecoverPassword.aspx.cs" Inherits="DebateScheduler.RecoverPassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <link rel ="stylesheet" href ="RecoveryStyle.css" type ="text/css" /> <!--This is the stylesheet for the register page.-->
    <div id ="PasswordRecovery">

            <asp:Label ID="Label_PasswordRecovery" runat="server" Font-Size="22pt" Text="Password Recovery"></asp:Label>

    </div>
    <div id ="RecoveryContent">

        <asp:Panel ID="Panel1" runat="server">
            <asp:Panel ID="Panel_UsernameSubmition" runat="server">
                <asp:Label ID="Label_Username0" runat="server" Text="Enter a username: "></asp:Label>
                <asp:TextBox ID="TextBox_Username" runat="server"></asp:TextBox>
                <asp:Button ID="Button_Submit0" runat="server" OnClick="Button_Submit0_Click" Text="Submit" />
                <br />
                <asp:Label ID="Label_UsernameWrong" runat="server" ForeColor="Red" Text="Username does not exist." Visible="False"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="Panel_QA" runat="server" Visible="False">
                <asp:Label ID="Label_Question" runat="server" Text="Question: "></asp:Label>
                <asp:TextBox ID="TextBox_Question" runat="server" OnTextChanged="TextBox3_TextChanged"></asp:TextBox>
                <br />
                <asp:Label ID="Label_Answer" runat="server" Text="Answer: "></asp:Label>
                <asp:TextBox ID="TextBox_Answer" runat="server"></asp:TextBox>
                <asp:Button ID="Button_SubmitAnswer" runat="server" OnClick="Button_SubmitAnswer_Click" Text="Submit" />
                <br />
                <asp:Label ID="Label_AnswerWrong" runat="server" ForeColor="Red" Text="Incorrect Answer" Visible="False"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="Panel_AnswerCorrect" runat="server" Visible="False">
                <asp:Label ID="Label_EmailSent" runat="server" Text="An email containing the password has been sent to the email associated with this account."></asp:Label>
            </asp:Panel>
        </asp:Panel>

    </div>
</asp:Content>
