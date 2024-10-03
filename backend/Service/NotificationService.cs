using BackendApp.Data;
using BackendApp.Model;
using BackendApp.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackendApp.Service{
    public interface INotificationService {
        public Notification? GetNotificationById(long id);
        public Notification[] GetAllNotifications();
        public bool AddNotification(Notification notification);
        public bool RemoveNotifications(long id);
        public UpdateResult UpdateNotifications(long id, Notification notificationContent);
        public Notification[] GetNotificationsForUser(long userId);
        void SendNotificationTo(RegularUser user, string content, PostBase? associatedPost = null);
        bool MarkNotificationAsRead(long notificationId);
    }

    public class NotificationService(ApiContext context) : INotificationService
    {
        private readonly ApiContext context = context;
        public bool AddNotification(Notification notification)
        {
            if(this.GetNotificationById(notification.Id) is not null) return false;
            this.context.Notifications.Add(notification);
            this.context.SaveChanges();
            return true;
        }

        public Notification[] GetAllNotifications()
            => this.context.Notifications.ToArray();
        public Notification? GetNotificationById(long id)
            => this.context.Notifications.FirstOrDefault( 
                notif => notif.Id == id 
            );

        public bool RemoveNotifications(long id){
            Notification? notif = this.GetNotificationById(id);
            if( notif is null ) return false;

            this.context.Notifications.Remove(notif);
            this.context.SaveChanges();
            return true;
        }

        public UpdateResult UpdateNotifications(long id, Notification notif)
        {
            //Check if user exists
            Notification? notifInDb = this.GetNotificationById(id);
            if(notifInDb is null) return UpdateResult.NotFound;

            //Save new data
            notifInDb.Update(notif);
            this.context.SaveChanges();
            return UpdateResult.Ok;
        }

        public Notification[] GetNotificationsForUser(long userId)
            => this.context.Notifications
                .Where( notif => notif.ToUser.Id == userId)
                .OrderBy( notif => notif.Timestamp)
                .ToArray();
    
        public void SendNotificationTo(RegularUser user, string content, PostBase? associatedPost = null)
        {
            this.AddNotification(new Notification(content, false, user, DateTime.Now, associatedPost));
            this.context.SaveChanges();
        }

        public bool MarkNotificationAsRead(long notificationId)
        {
            Notification? notification = this.GetNotificationById(notificationId);
            if(notification is null) return false;
            notification.Read = true;
            this.context.SaveChanges();
            return true;
        }
    }
}