<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="News.aspx.cs" Inherits="DebateScheduler.News" MaintainScrollPositionOnPostback="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <link rel ="stylesheet" href ="NewsStyle.css" type ="text/css" /> <!--This is the stylesheet which is used by the news page.-->

    <div id ="NewsArea" runat ="server">

        <br />
        <br />

        <asp:Panel ID="Panel_News" runat="server">
        </asp:Panel>

    </div>
</asp:Content>
