using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackendApp.Model;
using BackendApp.Data;
using BackendApp.Model.Requests;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using BackendApp.Model.Enums;
using BackendApp.auth;
using Utilities;
using Util;

namespace BackendApp.Service
{
    public interface IRegularUserService{
        public RegularUser? GetUserByEmail(string email);
        public RegularUser? GetUserById(long id);
        public RegularUser? GetUserByName(string name);
        public RegularUser? GetUserBySurname(string surname);
        public RegularUser[] GetAllUsers();
        public bool AddUser(RegularUser user);
        public bool RemoveUser(long id);
        public UpdateResult Update(long id, RegularUser user);
        public RegularUser[] SearchByUsernameFuzzy(string searchString); 
        public UpdateResult ChangePassword(long userId, string oldPassword, string newPassword);
    }
    public class RegularUserService(ApiContext context) : IRegularUserService
    {
        private readonly ApiContext context = context;

        public bool AddUser(RegularUser user)
        {
            if(this.AdminWithEmailExists(user.Email)) return false;
            if(this.GetUserByEmail(user.Email) != null) return false;
            this.context.RegularUsers.Add(user);
            this.context.SaveChanges();
            return true;
        }

        public RegularUser[] GetAllUsers()
            => this.context.RegularUsers.Include(user => user.HideableInfo).ToArray();

        public RegularUser? GetUserByEmail(string email) 
            => this.context.RegularUsers.FirstOrDefault( userInDb => userInDb.Email == email );

        public RegularUser? GetUserById(long id)
            => this.context.RegularUsers.FirstOrDefault( userInDb => userInDb.Id == id );

        public RegularUser? GetUserByName(string name)
            => this.context.RegularUsers.FirstOrDefault( userInDb => userInDb.Name == name );

        public RegularUser? GetUserBySurname(string surname)
            => this.context.RegularUsers.FirstOrDefault( userInDb => userInDb.Surname == surname );

        public UpdateResult Update(long id, RegularUser user)
        {
            //Check if admin with same email exists
            if(this.AdminWithEmailExists(user.Email)) 
                return UpdateResult.KeyAlreadyExists;

            //Check if user exists
            RegularUser? userInDb = this.GetUserById(id);
            if(userInDb is null) return UpdateResult.NotFound;
            
            //Check if user with same email exists
            RegularUser? userWithEmail = this.GetUserByEmail(user.Email);
            if(userWithEmail is not null && userWithEmail.Id != id) return UpdateResult.KeyAlreadyExists;

            //Save new data (except for password, dedicated endpoint for that)
            userInDb.Email = user.Email;
            userInDb.Name = user.Name;
            userInDb.Surname = user.Surname;
            userInDb.ImagePath = user.ImagePath;
            this.context.Remove(userInDb.HideableInfo);
            userInDb.HideableInfo = new(user.HideableInfo);
            this.context.SaveChanges();
            return UpdateResult.Ok;
        }

        public bool RemoveUser(long id){
            RegularUser? user = this.GetUserById(id);
            if( user is null ) return false;
            this.RemoveDataAssociatedWith(user);
            return true;
        }
        public RegularUser[] SearchByUsernameFuzzy(string searchString) 
        {
            int maxDistance = 20;
            return this.context.RegularUsers
                .AsEnumerable()
                .Select(user => new {User = user, Distance = FuzzySearch.LevenshteinDistance(searchString, user.FullName)})
                .Where( pair => pair.Distance <= maxDistance)
                .OrderBy( pair => pair.Distance)
                .Select(pair => pair.User)
                .ToArray();
        }

        public UpdateResult ChangePassword(long userId, string oldPassword, string newPassword)
        {
            var user = this.GetUserById(userId);
            if(user is null) return UpdateResult.NotFound;
            if(EncryptionUtility.HashPassword(oldPassword) != user.PasswordHash)
                return UpdateResult.Unauthorised;
            user.PasswordHash = EncryptionUtility.HashPassword(newPassword);
            this.context.SaveChanges();
            return UpdateResult.Ok;
        }  

        private bool AdminWithEmailExists(string email)
        {
            //Check if admin with same email exists
            AppUser? admin = this.context.AdminUsers
                .FirstOrDefault(admin => admin.Email == email);
            return admin is not null;
        }



        private void RemoveDataAssociatedWith(RegularUser user)
        {
            var connectionsToRemove = this.context.Connections
                .Where( con => con.SentBy == user || con.SentTo == user);
            this.context.Connections.RemoveRange(connectionsToRemove);
            var postsToRemove = this.context.Posts
                .Where(post => post.PostedBy == user);
            this.context.Posts.RemoveRange(postsToRemove);
            var jobsToRemove = this.context.JobPosts
                .Where(job => job.PostedBy == user);
            this.context.JobPosts.RemoveRange(jobsToRemove);
            var messagesToRemove = this.context.Messages
                .Where(message => message.SentBy == user || message.SentTo == user);
            this.context.Messages.RemoveRange(messagesToRemove);            
            var notificationsToRemove = this.context.Notifications
                .Where(notification => notification.ToUser == user);
            this.context.Notifications.RemoveRange(notificationsToRemove);

            this.context.Remove(user);
            this.context.SaveChanges();
        }
    }
}




