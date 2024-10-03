


using BackendApp.auth;
using BackendApp.Data;
using BackendApp.Model;

namespace Util.DataFeeding;


public static class DummyDataFeeder
{
    public static void FillWithDummyData(ApiContext context)
    {
        var adminUser = new AdminUser("a@emailer.com", EncryptionUtility.HashPassword("bigchungusplayer6969f12"));
        var user1 = new RegularUser(
                "b@emailer.com",
                EncryptionUtility.HashPassword("bigchungusplayer6969f12"), 
                "boop", "poop", null, new(
                    "6900000000", "My House :D", 
                    ["Fullstack Dotnet Developer at Microsoft: 4 years", "Game Developer at Microsoft: 2 years"], 
                    "Dead inside", 
                    ["Senior Software Developer", "Junior Software Engineer", "ASP .NET Developer"], 
                    ["Bachelor's degree in Computer Science from National Kapodistrian University of Athens"]));
        var user2 = new RegularUser(
                "c@emailer.gr",
                EncryptionUtility.HashPassword("bigchungusplayer6969f12"), 
                "Ninja", "Blevins", null, new(
                    "6950000769", "His House :D", 
                    ["Fullstack Dotnet Developer at Microsoft: 4 years", "Game Developer at Microsoft: 2 years"], 
                    "Dead inside", 
                    ["Junior Software Developer", "Gamer", "Streamer"], 
                    ["High School Graduate", "Academy player at G2", "Online Course Programming Certificate"]));
        var user3 = new RegularUser(
                "d@emailer.com",
                EncryptionUtility.HashPassword("bigchungusplayer6969f12"), 
                "namasteel", "babies", null, new(
                    "6900000000", "In hiding (I am convicted of illegal marijuana possession)", 
                    ["Hitman for the Russian Government: 7 years experience"], 
                    "Unemployed", 
                    ["Senior Hitman", "Junior Software Developer"], 
                    ["Certificate of Excellence in Service to the People"]));
        var user4 = new RegularUser(
                "e@emailer.com",
                EncryptionUtility.HashPassword("bigchungusplayer6969f12"), 
                "Person", "Eater", null, new(
                    "6900000000", "Belarus", 
                    ["Personal Chef for Hannibal Lecter: 3 years"], 
                    "Unemployed", 
                    ["Chef", "Cook", "Head Chef"], 
                    ["Doctorate In Cooking Somehow I don't know."]));
        var user5 = new RegularUser(
                "f@emailer.com",
                EncryptionUtility.HashPassword("bigchungusplayer6969f12"), 
                "Jean", "Schena", null, new(
                    "6900000000", "Florida", 
                    ["Professional Wrestling: 10 years"], 
                    "Women's World Heavyweight Champion in WUUE", 
                    ["Karate", "Olympic Wrestler", "Professional Wrestler", "Athletics"], 
                    ["Community Colledge Graduate"]));

        var post1 = new Post(
            user1, [user2, user3], 
            DateTime.Now, 
            [], 
            "I'm starting to have a lot of fun building posts in Rust.",
            [],
            false
        );
        var post2 = new Post(
            user1, [], 
            DateTime.Now, 
            [], 
            "I hate Javascript. It's the worst language ever!",
            [],
            false
        );
        var post3 = new Post(
            user1, [user2], 
            DateTime.Now, 
            [], 
            "Elixir is starting to really grow on me...",
            [],
            false
        );
        var post4 = new Post(
            user3, [user1, user2],
            DateTime.Now,
            [],
            "Killed the president of Cuba today. Hard times ahead, have to be on the lookout for Cuban Intelligence.... But hard times make better men.",
            [],
            false
        );
        var post5 = new Post(
            user4, [user3, user2],
            DateTime.Now,
            [],
            "I cooked some liver today. Was pretty good. I have the urge to cry in the bathroom for the next few hours now.",
            [
                new Post(
                    user3, [user4],
                    DateTime.Now,
                    [],
                    "Damn, I'd personally be proud if I managed to make liver tasty. Anything wrong? Who do you... Oh.",
                    [],
                    true
                )
            ],
            false
        );
        var post6 = new Post(
            user4, [user3],
            DateTime.Now,
            [],
            "I hate my job.",
            [],
            false
        );
        var post7 = new Post(
            user5, [user1, user2, user3],
            DateTime.Now,
            [],
            "I JUST WON A TITLE LOL!",
            [],
            false
        );
        var post8 = new Post(
            user1, [user2],
            DateTime.Now,
            [],
            "DO NOT DO THIS IN .NET: IEnumerables are not to be confused with IQueryables! Make sure to turn your Queryables into Enumberables before passing the around!",
            [],
            false
        );
        var post9 = new Post(
            user1, [user2, user3, user4, user5],
            DateTime.Now,
            [],
            "DO NOT DO THIS IN JAVASCRIPT: Code.",
            [],
            false
        );
        var post10 = new Post(
            user3, [],
            DateTime.Now,
            [],
            "Killed the president of Cuba today. Hard times ahead, have to be on the lookout for Cuban Intelligence.... But hard times make better men.",
            [],
            false
        );
        var jobPost1 = new JobPost(
            user1, [], 
            DateTime.Now, 
            [],
            "Needed: ASP .NET developer",
            "Our company needs an ASP .NET developer for a project. Could you help?",
            ["ASP .NET Developer", "Junior Software Developer", "Senior Software Developer"]
        );
        var jobPost2 = new JobPost(
            user2, [], 
            DateTime.Now, 
            [],
            "Searching for Morning Shift Security Officer",
            "I have noticed strange men with pagers walking around the front of my apartment complex and I have begun to worry about the safety of my neighbours. " 
            + "I need someone to cover the morning shift (4AM to 12PM). Just be like, very big. Like very very big. And know how to use a gun, hopefully",
            ["Security", "Bodyguard", "Police Officer", "Private Security"]
        );

        var jobPost3 = new JobPost(
            user3, [], 
            DateTime.Now, 
            [],
            "Expert baby Thief Position",
            "There's too many babies around. I want to steel them! Don't contact me after midnight, I'm stealing babies.",
            ["Theft", "Larsony", "Lawyer", "Athletics"]
        );
        var jobPost4 = new JobPost(
            user4, [], 
            DateTime.Now, 
            [],
            "Looking for head chef!",
            "Looking for someone who can cook delicious meals using meat. Preference for people with zero moral conscience.",
            ["Chef", "Head Chef", "Cook"]
        );
        var jobPost5 = new JobPost(
            user2, [], 
            DateTime.Now, 
            [],
            "Maid Position Open",
            "Looking for a maid. Preferably a cute girl. I don't go out much so I don't have time to socialise...",
            ["Model", "Athletics"]
        );
        var jobPost6 = new JobPost(
            user4, [], 
            DateTime.Now, 
            [],
            "Somebody sued me, now I'm looking for a laywer...",
            "Someone is baselessly accusing me of eating their parents. I need a laywer who can prove that I didn't, and that even if I did, it wasn't a big deal.",
            ["Lawyer", "Legal Counselor"]
        );


        
        var connection1 = new Connection(user1, user3, false, DateTime.Now);
        var connection2 = new Connection(user1, user2, true, DateTime.Now);
        var connection3 = new Connection(user1, user5, true, DateTime.Now);
        var connection4 = new Connection(user3, user4, true, DateTime.Now);

        context.AdminUsers.Add(adminUser);
        context.RegularUsers.Add(user1);
        context.RegularUsers.Add(user2);
        context.RegularUsers.Add(user3);
        context.RegularUsers.Add(user4);
        context.RegularUsers.Add(user5);
        context.SaveChanges();
        context.Posts.Add(post1);
        context.Posts.Add(post2);
        context.Posts.Add(post3);
        context.Posts.Add(post4);
        context.Posts.Add(post5);
        context.Posts.Add(post6);
        context.Posts.Add(post7);
        context.Posts.Add(post8);
        context.Posts.Add(post9);
        context.Posts.Add(post10);
        context.JobPosts.Add(jobPost1);
        context.JobPosts.Add(jobPost2);
        context.JobPosts.Add(jobPost3);
        context.JobPosts.Add(jobPost4);
        context.JobPosts.Add(jobPost5);
        context.JobPosts.Add(jobPost6);
        context.Connections.Add(connection1);
        context.Connections.Add(connection2);
        context.Connections.Add(connection3);
        context.Connections.Add(connection4);
        context.SaveChanges();
    }
}