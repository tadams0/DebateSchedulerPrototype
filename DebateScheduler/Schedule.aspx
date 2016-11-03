<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Schedule.aspx.cs" Inherits="DebateScheduler.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <table>
        <tr style="width:100%;margin:0 auto">
            <td class="auto-style1">
                <asp:Calendar ID="Calendar1" runat="server" Width="350px" BackColor="White" BorderColor="White" BorderWidth="1px" Font-Names="Verdana" Font-Size="9pt" ForeColor="Black" Height="190px" NextPrevFormat="FullMonth" OnSelectionChanged="Calendar1_SelectionChanged" OnDayRender="DayRender">
                    <DayHeaderStyle Font-Bold="True" Font-Size="8pt" />
                    <NextPrevStyle Font-Bold="True" Font-Size="8pt" ForeColor="#333333" VerticalAlign="Bottom" />
                    <OtherMonthDayStyle ForeColor="#999999" />
                    <SelectedDayStyle BackColor="#333399" ForeColor="White" />
                    <TitleStyle BackColor="White" BorderColor="Black" BorderWidth="4px" Font-Bold="True" Font-Size="12pt" ForeColor="#333399" />
                    <TodayDayStyle BackColor="#CCCCCC" />
                </asp:Calendar>
            </td>
            <td style="width:300px"></td>
            <td>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" DataSourceID="SqlDataSource1">
            <Columns>
                <asp:BoundField DataField="Team_1" HeaderText="Team_1" ReadOnly="True" SortExpression="Team_1" />
                <asp:BoundField DataField="Team_2" HeaderText="Team_2" ReadOnly="True" SortExpression="Team_2" />
                <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" />
                <asp:BoundField DataField="T1Score" HeaderText="T1Score" SortExpression="T1Score" />
                <asp:BoundField DataField="T2Score" HeaderText="T2Score" SortExpression="T2Score" />
            </Columns>
            <FooterStyle BackColor="White" ForeColor="#000066" />
            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
            <RowStyle ForeColor="#000066" />
            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#007DBB" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#00547E" />
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:UserTable %>" SelectCommand="SELECT DISTINCT (SELECT Name FROM Teams WHERE (Id = Debates.TID1)) AS Team_1, (SELECT Name FROM Teams AS Teams_2 WHERE (Id = Debates.TID2)) AS Team_2, Debates.Date, Debates.T1Score, Debates.T2Score FROM Teams, Debates WHERE ([Date] = @Date) ORDER BY Debates.Date DESC">
            <SelectParameters>
                <asp:ControlParameter ControlID="Calendar1" Name="Date" PropertyName="SelectedDate" />
            </SelectParameters>
            </asp:SqlDataSource>
            </td>
        </tr>
        
    </table>
</asp:Content>
