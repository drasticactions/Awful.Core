using Awful.Core.Managers.JSON;
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

        public ManagerTest ()
        {
          
        }

        [Fact]
        public async Task GetForumListAsync_Test()
        {
            var WebClient = Setup.SetupWebClient().Result;
            IndexPageManager indexManager = new IndexPageManager(WebClient);
            var result = await indexManager.GetForumListAsync();
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.True(result.TrueForAll(n => n.Id != 0));
        }

        [Fact]
        public async Task GetIndexPageAsyncWithMetadata_Test()
        {
            var WebClient = Setup.SetupWebClient().Result;
            IndexPageManager indexManager = new IndexPageManager(WebClient);
            var result = await indexManager.GetIndexPageAsync(true);
            Assert.NotNull(result);
            Assert.True(result.Forums.Any());
            Assert.True(result.Forums.TrueForAll(n => n.Id != 0));
        }

        [Fact]
        public async Task GetIndexPageAsync_Test()
        {
            var WebClient = Setup.SetupWebClient().Result;
            IndexPageManager indexManager = new IndexPageManager(WebClient);
            var result = await indexManager.GetIndexPageAsync();
            Assert.NotNull(result);
            Assert.True(result.Forums.Any());
        }

        [Fact]
        public async Task GetThreadAsync_Test()
        {
            var WebClient = Setup.SetupWebClient().Result;
            // If we're not authed, this will fail with paywall.
            // Need a good account to handle this in CI...
            if (!WebClient.IsAuthenticated)
                return;
            ThreadManager threadManager = new ThreadManager(WebClient);
            var result = await threadManager.GetThreadAsync(3847930, true);
            Assert.NotNull(result);
            Assert.True(result.Posts.Any());
        }

        [Fact]
        public async Task GetBookmarkAsync_Test()
        {
            var WebClient = Setup.SetupWebClient().Result;
            // If we're not authed, this will fail with paywall.
            // Need a good account to handle this in CI...
            if (!WebClient.IsAuthenticated)
                return;
            BookmarkManager bookmarkManager = new BookmarkManager(WebClient);
            var result = await bookmarkManager.GetAllBookmarksAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetForumCategoriesAsync_Test()
        {
            var WebClient = Setup.SetupWebClient().Result;
            ForumManager forumManager = new ForumManager(WebClient);
            var forumCatList = await forumManager.GetForumCategoriesAsync();
            Assert.NotNull(forumCatList);
            Assert.True(forumCatList.Any());
        }

        [Fact]
        public async Task GetThreadListAsync_Test()
        {
            var WebClient = Setup.SetupWebClient().Result;
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
