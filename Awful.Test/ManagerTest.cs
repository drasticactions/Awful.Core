using Awful.Parser.Core;
using Awful.Parser.Managers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Awful.Test
{
    public class ManagerTest
    {
        WebClient WebClient;

        public ManagerTest ()
        {
            WebClient = Setup.SetupWebClient().Result;
        }

        [Fact]
        public async Task GetThreadAsync_Test()
        {
            ThreadManager threadManager = new ThreadManager(WebClient);
            var result = await threadManager.GetThreadAsync(3847930, true);
            Assert.NotNull(result);
            Assert.True(result.Posts.Any());
        }

        [Fact]
        public async Task GetBookmarkAsync_Test()
        {
            BookmarkManager bookmarkManager = new BookmarkManager(WebClient);
            var result = await bookmarkManager.GetAllBookmarksAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetForumCategoriesAsync_Test()
        {
            ForumManager forumManager = new ForumManager(WebClient);
            var forumCatList = await forumManager.GetForumCategoriesAsync();
            Assert.NotNull(forumCatList);
            Assert.True(forumCatList.Any());
        }

        [Fact]
        public async Task GetThreadListAsync_Test()
        {
            ForumManager forumManager = new ForumManager(WebClient);
            var forumCatList = await forumManager.GetForumCategoriesAsync();
            Assert.NotNull(forumCatList);
            Assert.True(forumCatList.Any());
            
            ThreadListManager threadManager = new ThreadListManager(WebClient);
            var result = await threadManager.GetForumThreadListAsync(forumCatList.First().ForumList.First(), 1);
            Assert.NotNull(result);
            Assert.True(result.Any());
        }
    }
}
