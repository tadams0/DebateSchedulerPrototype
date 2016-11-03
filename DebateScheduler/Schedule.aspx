<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Schedule.aspx.cs" Inherits="DebateScheduler.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <div id ="debateDisplay"> 
    <asp:Panel ID="Panel1" runat="server">
        <asp:Panel ID="Panel_Title" runat="server" HorizontalAlign="Center">
            <asp:Label ID="Label_Title" runat="server" Font-Size="X-Large" Text="Schedule"></asp:Label>
        </asp:Panel>
        <br />
        <asp:Table ID="Table1" runat="server" BorderStyle="Solid" GridLines="Both" HorizontalAlign="Center">
        </asp:Table>
        
        <br />
    </asp:Panel>
    </div>

    <asp:Panel ID="Panel_NoDebate" runat="server" HorizontalAlign="Center" Visible="False">
        <asp:Label ID="Label_NoSchedule" runat="server" Font-Size="X-Large" Text="There is currently no ongoing debate season."></asp:Label>
        <br />
        </asp:Panel>
</asp:Content>
