<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DebateScheduler.Default" MaintainScrollPositionOnPostback="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">

    <div id ="debateDisplay"> 
    <asp:Panel ID="Panel1" runat="server">
        <br />
            <asp:Table ID="Table1" runat="server" BorderStyle="Solid" GridLines="Both" HorizontalAlign="Center">
                </asp:Table>
        <br />
    </asp:Panel>
    </div>
    <table style="margin:0 auto;width:100%">
        <tr>
            <td style="width:49%"></td>
            <td style="width:150px;height:50px">
                <asp:Button ID="UpdateButton" runat="server" Text="Update Scores"  Width="150px" Height="50px" OnClick="UpdateButton_Click"/>
            </td>
            <td style="width:49%"></td>
        </tr>
    </table>

</asp:Content>
