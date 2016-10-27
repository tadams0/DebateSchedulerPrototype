using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateScheduler
{
    public partial class SuperView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_SelectedIndexChanged(Object sender, EventArgs e)
        {
            GridViewRow row = GridView1.SelectedRow;
            Label5.Text = row.Cells[5].Text;
        }

    }
}