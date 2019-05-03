using Awful.Parser.Core;
using Awful.Parser.Managers;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.PostIcons;
using Awful.Parser.Models.Threads;
using Awful.Parser.Models.Users;
using Awful.Web.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Awful.Core.Console
{
    class MainClass
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            Directory.CreateDirectory("test");
            WebClient WebClient = await AuthUserAsync();
            await TestTemplate(WebClient);
            //var thread = await SaveThreadAsync(WebClient);
            //var bookmarks = await SaveBookmarksAsync(WebClient);
            //var forumCategory = await SaveForumListAsync(WebClient);
            //var forum = forumCategory.First().ForumList.First();
            //var threads = await SaveThreadListAsync(forum, WebClient);
            //var pmIcons = await SavePMIconsAsync(WebClient);
            //var icons = await SavePostIconAsync(forum.ForumId, WebClient);
            //var smiliesCategories = await SaveSmileCategoriesAsync(WebClient);
            //var profile = await SaveProfileUserAsync(37588, WebClient);

            System.Console.WriteLine("done");
        }

        static async Task TestTemplate(WebClient WebClient)
        {
            ThreadManager threadManager = new ThreadManager(WebClient);
            var result = await threadManager.GetThreadAsync(3847930, true);
            var post = result.Posts.FirstOrDefault();
            var template = PostTemplate.Render(post);
        }

        static async Task<Thread> SaveThreadAsync(WebClient WebClient)
        {
            ThreadManager threadManager = new ThreadManager(WebClient);
            var result = await threadManager.GetThreadAsync(3847930, true);
            File.WriteAllText("test\\thread.json", JsonConvert.SerializeObject(result));
            System.Console.WriteLine("Saved Thread");
            return result;
        }

        static async Task<List<Thread>> SaveBookmarksAsync(WebClient WebClient)
        {
            BookmarkManager bookmarkManager = new BookmarkManager(WebClient);
            var result = await bookmarkManager.GetAllBookmarksAsync();
            File.WriteAllText("test\\bookmarks.json", JsonConvert.SerializeObject(result));
            System.Console.WriteLine("Saved Bookmarks List");
            return result;
        }

        static async Task<List<Thread>> SaveThreadListAsync(Forum forum, WebClient WebClient)
        {
            ThreadListManager threadManager = new ThreadListManager(WebClient);
            var result = await threadManager.GetForumThreadListAsync(forum, 1);
            File.WriteAllText("test\\threadlist.json", JsonConvert.SerializeObject(result));
            System.Console.WriteLine("Saved Thread List");
            return result;
        }

        static async Task<List<PostIcon>> SavePMIconsAsync(WebClient WebClient)
        {
            PostIconManager postIconManager = new PostIconManager(WebClient);
            var postIconResult = await postIconManager.GetPostIconsAsync(true);
            File.WriteAllText("test\\postpmiconlist.json", JsonConvert.SerializeObject(postIconResult));
            System.Console.WriteLine("Saved PM Icons");
            return postIconResult;
        }

        static async Task<List<PostIcon>> SavePostIconAsync(int forumId, WebClient WebClient)
        {
            PostIconManager postIconManager = new PostIconManager(WebClient);
            var postIconResult = await postIconManager.GetPostIconsAsync(false, forumId);
            File.WriteAllText("test\\posticonlist.json", JsonConvert.SerializeObject(postIconResult));
            System.Console.WriteLine("Saved Icons");
            return postIconResult;
        }

        static async Task<User> SaveProfileUserAsync(long profileId, WebClient WebClient)
        {
            UserManager userManager = new UserManager(WebClient);
            var userResult = await userManager.GetUserFromProfilePageAsync(profileId);
            var profile = JsonConvert.SerializeObject(userResult);
            File.WriteAllText("test\\profile.json", profile);
            System.Console.WriteLine("Saved Profile");
            return userResult;
        }

        static async Task<List<Awful.Parser.Models.Smilies.SmileCategory>> SaveSmileCategoriesAsync(WebClient WebClient)
        {
            SmileManager smileManager = new SmileManager(WebClient);
            var smileResult = await smileManager.GetSmileListAsync();
            File.WriteAllText("test\\smilies.json", JsonConvert.SerializeObject(smileResult));
            System.Console.WriteLine("Saved Smile List");
            return smileResult;
        }

        static async Task<List<Category>> SaveForumListAsync(WebClient WebClient)
        {
            ForumManager forumManager = new ForumManager(WebClient);
            var forumListJson = await forumManager.GetForumCategoriesAsync();
            File.WriteAllText("test\\forums.json", JsonConvert.SerializeObject(forumListJson));
            System.Console.WriteLine("Saved Forum List");
            return forumListJson;
        }

        static async Task<WebClient> AuthUserAsync(string username = "", string password = "")
        {
            if (!File.Exists("user.cookies"))
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new Exception("You must set the username and password to log in!");
                var WebClient = new WebClient();
                var authManager = new AuthenticationManager(WebClient);
                var result = await authManager.AuthenticateAsync(username, password);
                using (FileStream stream = File.Create("user.cookies"))
                {
                    var formatter = new BinaryFormatter();
                    System.Console.WriteLine("Serializing cookie container");
                    formatter.Serialize(stream, WebClient.CookieContainer);
                }
                return WebClient;
            }
            else
            {
                System.Net.CookieContainer cookieContainer;
                using (FileStream stream = File.OpenRead("user.cookies"))
                {
                    var formatter = new BinaryFormatter();
                    System.Console.WriteLine("Deserializing cookie container");
                    cookieContainer = (System.Net.CookieContainer)formatter.Deserialize(stream);
                    return new WebClient(cookieContainer);
                }
            }
        }
    }
}
