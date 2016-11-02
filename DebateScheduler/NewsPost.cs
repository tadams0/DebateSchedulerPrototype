using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebateScheduler
{
    /// <summary>
    /// Creates a news post object that holds HTML code that can be embedded on the news page.
    /// </summary>
    public class NewsPost
    {

        /// <summary>
        /// The ID of the newspost in the database.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// The user who created the news post.
        /// </summary>
        public User Creator { get; private set; }

        /// <summary>
        /// The date the news post was created.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// The date the news post was last updated.
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the title of the news post.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The data within the news post.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// If true the news post has been updated at some point. If false it has not.
        /// </summary>
        public bool HasBeenUpdated { get { return Date != LastUpdateDate; } }

        /// <summary>
        /// Instantiates a news post object.
        /// </summary>
        /// <param name="id">The ID of the news post, this is only important if the news post is being retrieved from the database.</param>
        /// <param name="creator">The user that created this news post.</param>
        /// <param name="creationDate">The date this news post was created.</param>
        /// <param name="data">The data this news post holds.</param>
        public NewsPost(int id, User creator, DateTime creationDate, string title, string data)
        {
            ID = id;
            Creator = creator;
            Date = creationDate;
            LastUpdateDate = creationDate;
            Data = data;
            Title = title;
        }

        public override string ToString()
        {
            //string subData = "";
            //if (Data.Length > 100)
            //    subData = Data.Substring(0, 100);
            //else
            //    subData = Data;

            //return "{ Date: " + Date.ToString() + " First 100 Data Characters: " + subData.ToString() + "... }";
            return "{ News Post Created By: " + Creator.Username.ToLowerInvariant() + " on " + Date.ToString() + " }";
        }

    }
}