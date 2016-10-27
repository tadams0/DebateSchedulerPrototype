using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        /*protected void Button1_Click(object sender, EventArgs e)
        {
            string connection = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Database.mdf;Integrated Security=False; User Id=right; password= click";
            
            int DebateID;
            int Score1;
            int Score2;
            bool TestBool = Int32.TryParse(DropDownList3.SelectedValue, out DebateID);
            bool Test1Bool = Int32.TryParse(DropDownList1.SelectedValue, out Score1);
            bool Test2Bool = Int32.TryParse(DropDownList2.SelectedValue, out Score2);
            SqlParameter[] param = { Score1, Score2, DebateID };


            string updateQuery = "UPDATE Debates SET Team1Score =@Score1, Team2Score =@Score2 WHERE Id=@DebateID";
            DebateHandler.ExecuteSqlQuery(connection, updateQuery, param);
        }*/
    }
}