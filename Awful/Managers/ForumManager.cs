using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Awful.Parser.Managers
{
    public class ForumManager
    {
        private readonly WebClient _webManager;

        public ForumManager(WebClient webManager)
        {
            _webManager = webManager;
        }

        public async Task<List<Category>> GetForumCategoriesViaSelectAsync(CancellationToken token = new CancellationToken())
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");
            var result = await _webManager.GetDataAsync(EndPoints.ForumListPage, token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return ForumHandler.ParseCategoryList(document);
        }

        public async Task<Category> GetForumDescriptionsFromCategoryPageAsync(Category category, CancellationToken token = new CancellationToken())
        {
            var result = await _webManager.GetDataAsync(string.Format(EndPoints.ForumPage, category.Id, token));
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return ForumHandler.ParseForumDescriptions(document, category);
        }

        public async Task<Forum> GetForumDescriptionsFromForumPageAsync(Forum forum, CancellationToken token = new CancellationToken())
        {
            if (forum.SubForums.Count <= 0)
                return forum;
            var result = await _webManager.GetDataAsync(string.Format(EndPoints.ForumPage, forum.ForumId, token));
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return ForumHandler.ParseSubForumDescriptions(document, forum);
        }

        public async Task<List<Category>> GetForumCategoriesAsync(CancellationToken token = new CancellationToken())
        {
            var result = await _webManager.GetDataAsync(EndPoints.BaseUrl, token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return ForumHandler.ParseCategoryList(document);
        }
    }
}
