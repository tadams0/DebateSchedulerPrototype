using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateScheduler
{
    public partial class News : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            List<NewsPost> newsPosts = DatabaseHandler.GetNewsPosts(0, 1);

            foreach (NewsPost post in newsPosts)
            {
                LiteralControl control = new LiteralControl();
                control.Text= "<div id='thumbs'>" + post.Title + "<ul class='thumbs noscript'>";
                Panel resultPanel = CreateNewsPanel(post.Title);
                Controls.AddAt(Controls.Count - 1, control);
            }

        }

        private Panel CreateNewsPanel(string title)
        {
            Label titleLabel = new Label();
            titleLabel.Text = title;

            Panel panel = new Panel();
            panel.Controls.Add(titleLabel);

            return panel;
        }

    }
}