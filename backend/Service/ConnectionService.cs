using BackendApp.Data;
using BackendApp.Model;
using BackendApp.Model.Enums;

namespace BackendApp.Service{
    public interface IConnectionService {
        Connection? GetConnectionById(long id);
        Connection[] GetAllConnections();
        Connection[] GetAllConnectionsSentBy(long userId);
        Connection[] GetAllConnectionsSentTo(long userId);
        bool AreConnected(RegularUser userA, RegularUser userB);
        bool AddConnection(Connection Connection);
        bool RemoveConnection(long id);
        UpdateResult UpdateConnection(long id, Connection Connection);
        bool SendConnectionRequest(RegularUser from, RegularUser to);
        bool SendConnectionRequest(uint from, uint to);
        bool DeclineConnectionRequest(RegularUser user, uint connectionId);
        bool AcceptConnectionRequest(uint connectionId);
        RegularUser[] GetUsersConnectedTo(RegularUser user);
        public Connection[] GetConnectionRequestsSentBy(RegularUser user);
        public Connection[] GetConnectionRequestsReceivedBy(RegularUser user);
    }

    public class ConnectionService(ApiContext context, IRegularUserService userService) : IConnectionService
    {
        private readonly ApiContext context = context;
        private readonly IRegularUserService userService = userService;
        public bool AddConnection(Connection connection)
        {
            if(this.GetConnectionById(connection.Id) is not null || this.AreConnected(connection.SentTo, connection.SentBy)) 
                return false;
            this.context.Connections.Add(connection);
            this.context.SaveChanges();
            return true;
        }

        public Connection[] GetAllConnections()
            => [.. this.context.Connections];
        
        public Connection[] GetAllConnectionsSentBy(long userId)
            => this.context.Connections
                .Where(Connection => Connection.SentBy.Id == userId)
                .OrderBy(Connection => Connection.Timestamp)
                .ToArray();
        public Connection[] GetAllConnectionsSentTo(long userId)
            => this.context.Connections
                .Where(Connection => Connection.SentTo.Id == userId)
                .OrderBy(Connection => Connection.Timestamp)
                .ToArray();
        
        public bool ConnectionHasBeenRequested(RegularUser by, RegularUser to)
        {
            return this.GetAllConnectionsSentBy(by.Id)
                .FirstOrDefault(user => user.Id == to.Id) is not null;
        }

        public bool AreConnected(RegularUser userA, RegularUser userB)
        {
            return this.context.Connections
                .Where(
                    con => con.IsBetween(userA, userB) && con.Accepted
                )
                .Any();
        }
        public Connection? GetConnectionById(long id)
            => this.context.Connections.FirstOrDefault( 
                Connection => Connection.Id == id 
            );

        public bool RemoveConnection(long id){
            Connection? Connection = this.GetConnectionById(id);
            if( Connection is null ) return false;

            this.context.Connections.Remove(Connection);
            this.context.SaveChanges();
            return true;
        }

        public UpdateResult UpdateConnection(long id, Connection Connection)
        {
            //Check if user exists
            Connection? ConnectionInDb = this.GetConnectionById(id);
            if(ConnectionInDb is null) return UpdateResult.NotFound;

            //Save new data
            ConnectionInDb.Update(Connection);
            this.context.SaveChanges();
            return UpdateResult.Ok;
        }

        public bool SendConnectionRequest(RegularUser from, RegularUser to)
        {   
            if(
                this.ConnectionHasBeenRequested(from, to)
                || this.ConnectionHasBeenRequested(to, from)
            ) return false;
            var Connection = new Connection
            (
                sentBy: from,
                sentTo: to,
                accepted: false,
                timestamp: DateTime.Now
            );
            this.context.Connections.Add(Connection);
            this.context.SaveChanges();
            return true;
        }
        public bool SendConnectionRequest(uint fromId, uint toId)
        {
            var from = this.userService.GetUserById(fromId);
            if(from is null) return false;
            var to = this.userService.GetUserById(toId);
            if(to is null) return false;
            return this.SendConnectionRequest(from, to);
        }

        public bool AcceptConnectionRequest(uint connectionId)
        {
            Connection? connection = this.GetConnectionById(connectionId);
            if(connection is null) return false;
            connection.Accepted = true;
            this.context.SaveChanges();
            return true;
        }

        public bool DeclineConnectionRequest(RegularUser user, uint connectionId)
        {
            Connection? connection = this.GetConnectionById(connectionId);
            if(connection is null || connection.SentBy.Id != user.Id) return false;
            this.RemoveConnection(connection.Id);
            return true;
        }
        public bool AcceptConnectionRequest(uint userId, uint connectionId)
        {
            var to = this.userService.GetUserById(userId);
            if(to is null) return false;
            return this.AcceptConnectionRequest(connectionId);
        }

        public bool DeclineConnectionRequest(uint userId, uint connectionId)
        {
            var to = this.userService.GetUserById(userId);
            if(to is null) return false;
            return this.DeclineConnectionRequest(to, connectionId);
        }

        public RegularUser[] GetUsersConnectedTo(RegularUser user)
        {
            return this.context.Connections
                .Where((con) => (user == con.SentBy || user == con.SentTo) && con.Accepted)
                .Select((con) => con.SentBy == user ? con.SentTo : con.SentBy)
                .ToArray();
        }

        public Connection[] GetConnectionRequestsSentBy(RegularUser user)
        {
            return this.context.Connections
                .Where( (con) => user == con.SentBy ) 
                .ToArray();
        }

        public Connection[] GetConnectionRequestsReceivedBy(RegularUser user)
        {
            return this.context.Connections
                .Where( (con) => user == con.SentTo )
                .ToArray();
        }
    }
}