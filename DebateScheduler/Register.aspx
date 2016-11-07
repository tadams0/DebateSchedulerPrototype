<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="DebateScheduler.Register" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <link rel ="stylesheet" href ="RegisterStyle.css" type ="text/css" /> <!--This is the stylesheet for the register page.-->
    <div id = "Register">
        <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
            <asp:Label ID="Label_Title" runat="server" Text="Create New Account"></asp:Label>
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="Label_Code" runat="server" Text="Code:"></asp:Label>
            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        </asp:Panel>

        <asp:CreateUserWizard ID="CreateUserWizard" runat="server" CancelDestinationPageUrl="Default.aspx" Width="337px" OnCreatingUser="CreateUserWizard_CreatingUser">
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server" Title="" />
                <asp:CompleteWizardStep runat="server" />
            </WizardSteps>
        </asp:CreateUserWizard>
        <asp:Label ID="Label_ErrorMessage" runat="server" ForeColor="Red" Text="Error Label" Visible="False"></asp:Label>
        <asp:Panel ID="Panel_CreatedUser" runat="server" Height="27px" Width="344px">
            <asp:Label ID="Label_UserCreated" runat="server" Text="User Created Successfully!"></asp:Label>
        </asp:Panel>
        
    </div>

</asp:Content>

