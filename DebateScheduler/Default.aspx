<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DebateScheduler.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <p style="height: 531px; width: 949px">
        <asp:GridView ID="GridView1" runat="server" AllowSorting="True" AutoGenerateColumns="False" BackColor="#CCCCCC" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2" DataSourceID="SqlDataSource1" ForeColor="Black" style="top: 191px; left: 16px; position: absolute; height: 209px; width: 238px; font-weight: 700">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:BoundField DataField="TotalScore" HeaderText="TotalScore" SortExpression="TotalScore" />
            </Columns>
            <EmptyDataTemplate>
                Rankings
            </EmptyDataTemplate>
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Left" />
            <PagerTemplate>
                Rankings
            </PagerTemplate>
            <RowStyle BackColor="White" />
            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#808080" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#383838" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:UserTable %>" SelectCommand="SELECT [Name], [Wins], [Ties], [Losses], [TotalScore] FROM [Teams] ORDER BY [Name]"></asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:UserTable %>" SelectCommand="SELECT [Name], [TotalScore] FROM [Teams] ORDER BY [TotalScore] DESC"></asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:UserTable %>" SelectCommand="SELECT DISTINCT (SELECT Name FROM Teams WHERE (Id = Debates.TID1)) AS Team_1, (SELECT Name FROM Teams AS Teams_2 WHERE (Id = Debates.TID2)) AS Team_2, Debates.Date, Debates.T1Score, Debates.T2Score FROM Teams, Debates ORDER BY Debates.Date DESC"></asp:SqlDataSource>
        <asp:GridView ID="GridView2" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" BackColor="#CCCCCC" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2" DataSourceID="SqlDataSource3" ForeColor="Black" style="top: 190px; left: 291px; position: absolute; height: 469px; width: 515px">
            <Columns>
                <asp:BoundField DataField="Team_1" HeaderText="Team_1" ReadOnly="True" SortExpression="Team_1" />
                <asp:BoundField DataField="Team_2" HeaderText="Team_2" ReadOnly="True" SortExpression="Team_2" />
                <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" />
                <asp:BoundField DataField="T1Score" HeaderText="T1Score" SortExpression="T1Score" />
                <asp:BoundField DataField="T2Score" HeaderText="T2Score" SortExpression="T2Score" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle BackColor="Black" BorderStyle="Solid" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Left" />
            <RowStyle BackColor="White" />
            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#808080" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#383838" />
        </asp:GridView>
        <asp:DetailsView ID="DetailsView1" runat="server" AllowPaging="True" AutoGenerateRows="False" BackColor="#CCCCCC" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2" DataSourceID="SqlDataSource2" ForeColor="Black" style="top: 422px; left: 14px; position: absolute; height: 104px; width: 241px">
            <EditRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            <Fields>
                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                <asp:BoundField DataField="Wins" HeaderText="Wins" SortExpression="Wins" />
                <asp:BoundField DataField="Ties" HeaderText="Ties" SortExpression="Ties" />
                <asp:BoundField DataField="Losses" HeaderText="Losses" SortExpression="Losses" />
                <asp:BoundField DataField="TotalScore" HeaderText="TotalScore" SortExpression="TotalScore" />
            </Fields>
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            <HeaderTemplate>
                Individual Team Stats
            </HeaderTemplate>
            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Left" />
            <RowStyle BackColor="White" />
        </asp:DetailsView>
</p>

</asp:Content>
