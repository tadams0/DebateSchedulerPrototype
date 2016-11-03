using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateScheduler
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Makes the day unselectable and its back color "Ghost White" if it is not a Saturday.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void DayRender(Object source, DayRenderEventArgs e)
        {

            if (e.Day.Date.DayOfWeek != DayOfWeek.Saturday)
            {
                e.Day.IsSelectable = false;
                e.Cell.BackColor = System.Drawing.Color.GhostWhite;
            }
        }
    }
}