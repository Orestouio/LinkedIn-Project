using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApp.Model
{
    public class Notification
    (string content, bool read, RegularUser toUser, DateTime timestamp, PostBase? associatedPost)
    {
        protected Notification(string content, bool read, DateTime timestamp)
        : this(content, read, null!, timestamp, null)
        {}
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long Id {get; set;}
        public string Content {get; set;} = content;
        public bool Read {get; set;} = read;
        public virtual RegularUser ToUser {get; set;} = toUser; 
        public DateTime Timestamp {get; set;} = timestamp;
        public virtual PostBase? AssociatedPost {get; set;} = associatedPost;

        public void Update(Notification notification){
            this.Content = notification.Content;
            this.Id = notification.Id;
            this.Read = notification.Read;
            this.ToUser = notification.ToUser;
        }
    }
}