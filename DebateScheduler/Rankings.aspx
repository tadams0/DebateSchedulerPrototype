<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Rankings.aspx.cs" Inherits="DebateScheduler.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div id ="debateDisplay"> 
    <asp:Panel ID="Panel_NoDebates" runat="server" HorizontalAlign="Center">
        <asp:Label ID="Label1" runat="server" Font-Size="X-Large" Text="Rankings"></asp:Label>
        <br />
        <br />
        <asp:Table ID="Table1" runat="server" BorderStyle="Solid" GridLines="Both" HorizontalAlign="Center">
        </asp:Table>
        
        <br />
    </asp:Panel>
    </div>
    <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Center" Visible="False">
            <asp:Label ID="Label_NoDebates" runat="server" Font-Size="X-Large" Text="There is currently no ongoing debate season."></asp:Label>
        </asp:Panel>

</asp:Content>
