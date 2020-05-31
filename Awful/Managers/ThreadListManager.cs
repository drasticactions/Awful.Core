using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.Threads;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thread = Awful.Parser.Models.Threads.Thread;

namespace Awful.Parser.Managers
{
    public class ThreadListManager
    {
        private readonly WebClient _webManager;

        public ThreadListManager(WebClient webManager)
        {
            _webManager = webManager;
        }

        public async Task<List<Thread>> GetForumThreadListAsync(Forum forum, int page, CancellationToken token = default)
        {
            var pageUrl = string.Format(EndPoints.ForumPage, forum.ForumId) + string.Format(EndPoints.PageNumber, page);
            var result = await _webManager.GetDataAsync(pageUrl, token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            ForumHandler.GetForumPageInfo(document, forum);
            return ThreadHandler.ParseForumThreadList(document);
        }
    }
}
