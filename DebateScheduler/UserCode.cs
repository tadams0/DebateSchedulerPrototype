using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebateScheduler
{
    public class UserCode
    {
        /// <summary>
        /// The code.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// If true the code is still active.
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// The date the user code was generated.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Instantiates a user code object that holds information pertaining to a user code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="active">True if the code is still active and can be redeemed, false otherwise.</param>
        /// <param name="date">The date the code was generated.</param>
        public UserCode(string code, bool active, DateTime date)
        {
            Code = code;
            Active = active;
        }

    }
}