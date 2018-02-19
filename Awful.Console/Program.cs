using Awful.Managers;
using Awful.Models.Forums;
using Awful.Models.PostIcons;
using Awful.Models.Smilies;
using Awful.Models.Threads;
using Awful.Models.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Awful.Console
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
            WebManager webManager = await AuthUserAsync();
            var thread = await SaveThreadAsync(webManager);
            //var bookmarks = await SaveBookmarksAsync(webManager);
            //var forumCategory = await SaveForumListAsync(webManager);
            //var forum = forumCategory.First().ForumList.First();
            //var threads = await SaveThreadListAsync(forum, webManager);
            //var pmIcons = await SavePMIconsAsync(webManager);
            //var icons = await SavePostIconAsync(forum.ForumId, webManager);
            //var smiliesCategories = await SaveSmileCategoriesAsync(webManager);
            //var profile = await SaveProfileUserAsync(37588, webManager);
            
            System.Console.WriteLine("done");
        }

        static async Task<Thread> SaveThreadAsync(WebManager webManager)
        {
            ThreadManager threadManager = new ThreadManager(webManager);
            var result = await threadManager.GetThreadInfoAsync(3847930, true);
            var thread = JsonConvert.DeserializeObject<Thread>(result.ResultJson);
            File.WriteAllText("test\\thread.json", result.ResultJson);
            System.Console.WriteLine("Saved Thread");
            return thread;
        }

        static async Task<List<Thread>> SaveBookmarksAsync(WebManager webManager)
        {
            ThreadManager threadManager = new ThreadManager(webManager);
            var result = await threadManager.GetBookmarksAsync(1);
            var threadList = JsonConvert.DeserializeObject<List<Thread>>(result.ResultJson);
            File.WriteAllText("test\\bookmarks.json", result.ResultJson);
            System.Console.WriteLine("Saved Bookmarks List");
            return threadList;
        }

        static async Task<List<Thread>> SaveThreadListAsync(Forum forum, WebManager webManager)
        {
            ThreadManager threadManager = new ThreadManager(webManager);
            var result = await threadManager.GetForumThreadsAsync(forum, 1);
            var threadList = JsonConvert.DeserializeObject<List<Thread>>(result.ResultJson);
            File.WriteAllText("test\\threadlist.json", result.ResultJson);
            System.Console.WriteLine("Saved Thread List");
            return threadList;
        }

        static async Task<List<PostIcon>> SavePMIconsAsync(WebManager webManager)
        {
            PostIconManager postIconManager = new PostIconManager(webManager);
            var postIconResult = await postIconManager.GetPostIconsAsync(true);
            var postIcons = JsonConvert.DeserializeObject<List<PostIcon>>(postIconResult.ResultJson);
            File.WriteAllText("test\\postpmiconlist.json", postIconResult.ResultJson);
            System.Console.WriteLine("Saved PM Icons");
            return postIcons;
        }

        static async Task<List<PostIconCategory>> SavePostIconAsync(int forumId, WebManager webManager)
        {
            PostIconManager postIconManager = new PostIconManager(webManager);
            var postIconResult = await postIconManager.GetPostIconsAsync(false, forumId);
            var postIcons = JsonConvert.DeserializeObject<List<PostIconCategory>>(postIconResult.ResultJson);
            File.WriteAllText("test\\posticons.json", postIconResult.ResultJson);
            System.Console.WriteLine("Saved Icons");
            return postIcons;
        }

        static async Task<ProfileUser> SaveProfileUserAsync(long profileId, WebManager webManager)
        {
            UserManager userManager = new UserManager(webManager);
            var userResult = await userManager.GetUserFromProfilePageAsync(profileId);
            var profile = JsonConvert.DeserializeObject<ProfileUser>(userResult.ResultJson);
            File.WriteAllText("test\\profile.json", userResult.ResultJson);
            System.Console.WriteLine("Saved Profile");
            return profile;
        }

        static async Task<List<SmileCategory>> SaveSmileCategoriesAsync(WebManager webManager)
        {
            SmileManager smileManager = new SmileManager(webManager);
            var smileResult = await smileManager.GetSmileListAsync();
            var smiliesList = JsonConvert.DeserializeObject<List<SmileCategory>>(smileResult.ResultJson);
            File.WriteAllText("test\\smilies.json", smileResult.ResultJson);
            System.Console.WriteLine("Saved Smile List");
            return smiliesList;
        }

        static async Task<List<Category>> SaveForumListAsync(WebManager webManager)
        {
            ForumManager forumManager = new ForumManager(webManager);
            var forumListJson = await forumManager.GetForumCategoriesAsync();
            var forumCategoryList = JsonConvert.DeserializeObject<List<Category>>(forumListJson.ResultJson);
            File.WriteAllText("test\\forums.json", forumListJson.ResultJson);
            System.Console.WriteLine("Saved Forum List");
            return forumCategoryList;
        }

        static async Task<WebManager> AuthUserAsync(string username = "", string password = "")
        {
            if (!File.Exists("user.cookies"))
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new Exception("You must set the username and password to log in!");
                var webManager = new WebManager();
                var authManager = new Managers.AuthenticationManager(webManager);
                var result = await authManager.AuthenticateAsync(username, password);
                using (FileStream stream = File.Create("user.cookies"))
                {
                    var formatter = new BinaryFormatter();
                    System.Console.WriteLine("Serializing cookie container");
                    formatter.Serialize(stream, webManager.CookieContainer);
                }
                return webManager;
            }
            else
            {
                CookieContainer cookieContainer;
                using (FileStream stream = File.OpenRead("user.cookies"))
                {
                    var formatter = new BinaryFormatter();
                    System.Console.WriteLine("Deserializing cookie container");
                    cookieContainer = (CookieContainer)formatter.Deserialize(stream);
                    return new WebManager(cookieContainer);
                }
            }
        }
    }
}
