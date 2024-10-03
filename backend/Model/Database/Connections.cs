using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApp.Model
{
    public class Connection
    (
        RegularUser sentBy, 
        RegularUser sentTo, 
        bool accepted,
        DateTime timestamp
    )
    {
        /// <summary>
        /// EF constructor
        /// </summary>
        protected Connection(bool accepted, DateTime timestamp) : this(
            null!,
            null!,
            accepted, 
            timestamp
        )
        {}

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long Id {get; set;}
        public virtual RegularUser SentBy {get; set;} = sentBy;
        public virtual RegularUser SentTo {get; set;} = sentTo;
        public DateTime Timestamp {get; set;} = timestamp;
        public bool Accepted {get; set;} = accepted;
    
        public void Update(Connection connection)
        {
            this.SentBy = connection.SentBy;
            this.SentTo = connection.SentTo;
            this.Timestamp = connection.Timestamp;
            this.Accepted = connection.Accepted;
        }

        public bool IsBetween(RegularUser userA, RegularUser userB)
            => this.SentBy.Id == userA.Id && this.SentTo.Id == userB.Id
                || this.SentBy.Id == userB.Id && this.SentTo.Id == userA.Id;

    }
}