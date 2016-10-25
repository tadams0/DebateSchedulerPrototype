<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="DebateScheduler.Register" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <link rel ="stylesheet" href ="RegisterStyle.css" type ="text/css" /> <!--This is the stylesheet for the register page.-->
    <div id = "Register">
        <asp:CreateUserWizard ID="CreateUserWizard" runat="server" CancelDestinationPageUrl="Default.aspx" OnCreatedUser="CreateUserWizard_CreatedUser" Width="337px">
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server" Title="Register New Account" />
                <asp:CompleteWizardStep runat="server" />
            </WizardSteps>
        </asp:CreateUserWizard>
    </div>

</asp:Content>

