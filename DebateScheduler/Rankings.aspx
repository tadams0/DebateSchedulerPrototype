<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Rankings.aspx.cs" Inherits="DebateScheduler.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table>
        <tr>
            <td>
   <asp:SqlDataSource ID="RankingDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:UserTable %>" SelectCommand="SELECT [Name], [Wins], [Losses], [TotalScore] FROM [Teams] ORDER BY [Wins] DESC, [TotalScore] DESC"></asp:SqlDataSource>
   <asp:GridView ID="GridView1" runat="server" AllowSorting="True" AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" DataSourceID="RankingDataSource" style="margin-top: 0px">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="Wins" HeaderText="Wins" SortExpression="Wins" />
            <asp:BoundField DataField="Losses" HeaderText="Losses" SortExpression="Losses" />
            <asp:BoundField DataField="TotalScore" HeaderText="TotalScore" SortExpression="TotalScore" />
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
            </td>
            <td style="width:300px"></td>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:DetailsView ID="DetailsView1" runat="server" AllowPaging="True" AutoGenerateRows="False" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" DataSourceID="TeamDetailSource" Height="254px" Width="270px">
                            <EditRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                            <Fields>
                                <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                                <asp:BoundField DataField="Wins" HeaderText="Wins" SortExpression="Wins" />
                                <asp:BoundField DataField="Losses" HeaderText="Losses" SortExpression="Losses" />
                                <asp:BoundField DataField="Ties" HeaderText="Ties" SortExpression="Ties" />
                                <asp:BoundField DataField="TotalScore" HeaderText="TotalScore" SortExpression="TotalScore" />
                            </Fields>
                            <FooterStyle BackColor="White" ForeColor="#000066" />
                            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                            <HeaderTemplate>
                                Team Details
                            </HeaderTemplate>
                            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                            <RowStyle ForeColor="#000066" />
                        </asp:DetailsView>
                    </ContentTemplate>
                </asp:UpdatePanel>
    <asp:SqlDataSource ID="TeamDetailSource" runat="server" ConnectionString="<%$ ConnectionStrings:UserTable %>" SelectCommand="SELECT [Name], [Wins], [Losses], [Ties], [TotalScore] FROM [Teams] ORDER BY [Id]"></asp:SqlDataSource>
    
            </td>
        </tr>
    
    </table>
</asp:Content>
