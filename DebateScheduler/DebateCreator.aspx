<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="DebateCreator.aspx.cs" Inherits="DebateScheduler.DebateCreator" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style3 {
            width: 644px;
        }
        .auto-style4 {
            width: 643px;
            height: 27px;
        }
        .auto-style5 {
            width: 250px;
        }
        .auto-style6 {
            height: 27px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <link rel ="stylesheet" href ="NewsStyle.css" type ="text/css" /> <!--This is the stylesheet which is used by the debate scheduling page.-->
    <div id ="MainPanel">
        <asp:Panel ID="Panel_Main" runat="server" HorizontalAlign="Center">
            <asp:Label ID="Label_ScheduleGenerator" runat="server" Font-Size="X-Large" Text="Schedule Generator" ></asp:Label>
            <div id ="mainTable">
                <table border="1" style="width: 90%;" id ="tableHeader">
                <tr>
                    <td class="auto-style4">Schedule Info</td>
                    <td class="auto-style6">Teams</td>
                </tr>
            </table>
            <table border="1" style="width: 90%;" id ="tableContent">
                <tr>
                    <td class="auto-style3">
                        <div id="calendars" class="auto-style5" style="margin-left:auto; margin-right:auto; ">
                            <br />
                            <asp:Label ID="Label_StartDate" runat="server" Text="Start Date"></asp:Label>
                            <asp:Calendar ID="Calendar_Start" runat="server" OnDayRender="Calendar_Start_DayRender"></asp:Calendar>
                            <br />
                            <asp:Label ID="Label_EndDate" runat="server" Text="End Date"></asp:Label>
                            <br />
                            <asp:Calendar ID="Calendar_End" runat="server" OnDayRender="Calendar_End_DayRender"></asp:Calendar>
                            <br />
                        </div>
                    </td>
                    <td valign="top" align ="left">
                        <asp:Panel ID="Panel_Teams" runat="server">
                            <br />
                            <asp:Button ID="Button_AddTeam" runat="server" OnClick="Button_AddTeam_Click" Text="Add Team" />
                            <asp:Button ID="Button_Remove" runat="server" OnClick="Button_RemoveTeam_Click" Text="Remove Team" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            </div>
            
            <asp:Label ID="Label_ScheduleError" runat="server" ForeColor="Red" Text="There are errors with the info given." Visible="False"></asp:Label>
            <br />
            
            <asp:Button ID="Button_CreateSchedule" runat="server" Text="Create Schedule" OnClick="Button_CreateSchedule_Click" />
            <br />
            <br />
    </asp:Panel>
    </div>
    
</asp:Content>
