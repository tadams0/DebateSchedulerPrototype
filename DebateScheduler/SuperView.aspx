<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="SuperView.aspx.cs" Inherits="DebateScheduler.SuperView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <p style="height: 515px; width: 931px;">
        <asp:Label ID="Label3" runat="server" style="top: 400px; left: 546px; position: absolute; height: 21px; width: 105px" Text="Team 2 Score:"></asp:Label>
        <asp:DropDownList ID="DropDownList1" runat="server" style="top: 354px; left: 678px; position: absolute; height: 16px; width: 77px">
            <asp:ListItem>Forfeit</asp:ListItem>
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem Value="2"></asp:ListItem>
            <asp:ListItem Value="3"></asp:ListItem>
            <asp:ListItem>4</asp:ListItem>
            <asp:ListItem>5</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="DropDownList2" runat="server" style="top: 399px; left: 677px; position: absolute; height: 22px; width: 77px">
            <asp:ListItem>Forfeit</asp:ListItem>
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem>2</asp:ListItem>
            <asp:ListItem>3</asp:ListItem>
            <asp:ListItem>4</asp:ListItem>
            <asp:ListItem>5</asp:ListItem>
        </asp:DropDownList>
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" BackColor="#CCCCCC" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2" DataKeyNames="Id" DataSourceID="SqlDataSource1" ForeColor="Black" style="top: 219px; left: 2px; position: absolute; height: 397px; width: 412px">
            <Columns>
                <asp:CommandField ShowSelectButton="True" />
                <asp:BoundField DataField="Team_1" HeaderText="Team_1" ReadOnly="True" SortExpression="Team_1" />
                <asp:BoundField DataField="Team_2" HeaderText="Team_2" ReadOnly="True" SortExpression="Team_2" />
                <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" />
                <asp:BoundField DataField="T1Score" HeaderText="T1Score" SortExpression="T1Score" />
                <asp:BoundField DataField="T2Score" HeaderText="T2Score" SortExpression="T2Score" />
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" />
            <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Left" />
            <RowStyle BackColor="White" />
            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F1F1F1" />
            <SortedAscendingHeaderStyle BackColor="#808080" />
            <SortedDescendingCellStyle BackColor="#CAC9C9" />
            <SortedDescendingHeaderStyle BackColor="#383838" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:UserTable %>" SelectCommand="SELECT DISTINCT (SELECT Name FROM Teams WHERE (Id = Debates.TID1)) AS Team_1, (SELECT Name FROM Teams AS Teams_2 WHERE (Id = Debates.TID2)) AS Team_2, Debates.Date, Debates.T1Score, Debates.T2Score, Debates.Id FROM Teams, Debates ORDER BY Debates.Date DESC"></asp:SqlDataSource>
        <asp:Label ID="Label4" runat="server" style="top: 305px; left: 548px; position: absolute; height: 21px; width: 77px" Text="Debate ID:"></asp:Label>
        <asp:Label ID="Label2" runat="server" style="top: 357px; left: 544px; position: absolute; height: 14px; width: 111px" Text="Team 1 Score:"></asp:Label>
        <asp:DropDownList ID="DropDownList3" runat="server" DataSourceID="SqlDataSource2" DataTextField="Id" DataValueField="Id" style="top: 307px; left: 679px; position: absolute; height: 20px; width: 76px">
        </asp:DropDownList>
        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:UserTable %>" SelectCommand="SELECT [Id] FROM [Debates] WHERE ([Id] = @Id)">
            <SelectParameters>
                <asp:ControlParameter ControlID="GridView1" Name="Id" PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:Button ID="Button1" runat="server" style="top: 494px; left: 681px; position: absolute; height: 26px; width: 56px" Text="Submit" />
    </p>
</asp:Content>
