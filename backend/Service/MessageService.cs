using BackendApp.Data;
using BackendApp.Model;
using BackendApp.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackendApp.Service{
    public interface IMessageService {
        public Message? GetMessageById(long id);
        public Message[] GetAllMessages();
        public Message[] GetAllMessagesSentBy(uint userId);
        public Message[] GetAllMessagesSentTo(uint userId);
        public Message[] GetConversationBetween(uint userA, uint userB);
        public bool AddMessage(Message message);
        public bool RemoveMessage(long id);
        public UpdateResult UpdateMessage(long id, Message message);
        public bool SendMessage(RegularUser from, RegularUser to, string content);
        public bool SendMessage(uint from, uint to, string content);
        public Message[] GetRangeOfConversationBetween(RegularUser userA, RegularUser userB, int skip, int take);
        public RegularUser[] GetMembersOfChatsWith(RegularUser user);
    }

    public class MessageService(ApiContext context, IRegularUserService userService) : IMessageService
    {
        private readonly ApiContext context = context;
        private readonly IRegularUserService userService = userService;
        public bool AddMessage(Message message)
        {
            if(this.GetMessageById(message.Id) is not null) return false;
            this.context.Messages.Add(message);
            this.context.SaveChanges();
            return true;
        }

        public Message[] GetAllMessages()
            => [.. this.context.Messages];
        
        public Message[] GetAllMessagesSentBy(uint userId)
            => this.context.Messages
                .Where(message => message.SentBy.Id == userId)
                .OrderBy(message => message.Timestamp)
                .ToArray();
        public Message[] GetAllMessagesSentTo(uint userId)
            => this.context.Messages
                .Where(message => message.SentTo.Id == userId)
                .OrderBy(message => message.Timestamp)
                .ToArray();
        
        public Message[] GetConversationBetween(uint userAId, uint userBId)
            => this.context.Messages
                .Where(
                    message => 
                        (message.SentBy.Id == userAId && message.SentTo.Id == userBId)
                        || (message.SentBy.Id == userBId && message.SentTo.Id == userAId)
                )
                .OrderByDescending(message => message.Timestamp)
                .ToArray();

        public Message[] GetRangeOfConversationBetween(RegularUser userA, RegularUser userB, int skip, int take)
        {
            if(skip < 0 || take < 0) return [];
            return this.context.Messages
                .Where(
                    message => 
                        (message.SentBy == userA && message.SentTo == userB)
                        || (message.SentBy == userB && message.SentTo == userA)
                )
                .OrderByDescending(message => message.Timestamp)
                .Skip(skip)
                .Take(take)
                .ToArray();
        }
        

        public Message? GetMessageById(long id)
            => this.context.Messages.FirstOrDefault( 
                message => message.Id == id 
            );

        public bool RemoveMessage(long id){
            Message? message = this.GetMessageById(id);
            if( message is null ) return false;

            this.context.Messages.Remove(message);
            this.context.SaveChanges();
            return true;
        }

        public UpdateResult UpdateMessage(long id, Message message)
        {
            //Check if user exists
            Message? messageInDb = this.GetMessageById(id);
            if(messageInDb is null) return UpdateResult.NotFound;

            //Save new data
            messageInDb.SentBy = message.SentBy;
            messageInDb.SentTo = message.SentTo;
            messageInDb.Content = message.Content;
            messageInDb.Timestamp = message.Timestamp;
            this.context.SaveChanges();
            return UpdateResult.Ok;
        }

        public bool SendMessage(RegularUser from, RegularUser to, string content)
        {
            var message = new Message
            (
                content: content,
                sentBy: from,
                sentTo: to,
                DateTime.Now
            );
            this.context.Messages.Add(message);
            this.context.SaveChanges();
            return true;
        }

        public bool SendMessage(uint fromId, uint toId, string content)
        {
            var from = this.userService.GetUserById(fromId);
            if(from is null) return false;
            var to = this.userService.GetUserById(toId);
            if(to is null) return false;
            return this.SendMessage(from, to, content);
        }

        public RegularUser[] GetMembersOfChatsWith(RegularUser user)
        {
            var data = this.context.Messages
                .Include(message => message.SentBy)
                .Include(message => message.SentTo)
                .Where( message => message.SentBy == user || message.SentTo == user )
                .Select( message => message.SentBy == user ? message.SentTo : message.SentBy )
                .ToArray();
            return data.DistinctBy(user => user.Id).ToArray();
        }

    }
}