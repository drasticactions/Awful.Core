using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.Posts;
using Awful.Parser.Models.Threads;
using Awful.Parser.Models.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Awful.Parser.Managers
{
    public class ThreadManager
    {
        private readonly WebClient _webManager;

        public ThreadManager(WebClient webManager)
        {
            _webManager = webManager;
        }

        public async Task<Result> MarkThreadUnreadAsync(long threadId, CancellationToken token = new CancellationToken())
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");
            var result = new Result();
            try
            {
                var dic = new Dictionary<string, string>
                {
                    ["json"] = "1",
                    ["action"] = "resetseen",
                    ["threadid"] = threadId.ToString()
                };
                var header = new FormUrlEncodedContent(dic);
                result = await _webManager.PostDataAsync(EndPoints.ShowThreadBase, header, token);
                return result;
            }
            catch (Exception ex)
            {
                return ErrorHandler.CreateErrorObject(result, ex.Message, ex.StackTrace);
            }
        }

        public async Task<Models.Threads.Thread> GetThreadAsync(Models.Threads.Thread thread, bool goToNewestPost = false, CancellationToken token = new CancellationToken())
        {
            thread.Posts.Clear();
            var baseUri = string.Format(EndPoints.ThreadPage, thread.ThreadId);
            if (goToNewestPost)
                baseUri += string.Format(EndPoints.GotoNewPost);
            else if (thread.CurrentPage > 1)
                baseUri += string.Format(EndPoints.PageNumber, thread.CurrentPage);
            var result = await _webManager.GetDataAsync(baseUri, token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return ThreadHandler.ParseThread(document, thread);
        }

        public async Task<Models.Threads.Thread> GetThreadAsync (int threadId, bool goToNewestPost = false, CancellationToken token = new CancellationToken())
        {
            return await GetThreadAsync(new Models.Threads.Thread { ThreadId = threadId }, goToNewestPost, token);
        }

        public async Task<Post> GetPostAsync(int postId, CancellationToken token = new CancellationToken())
        {
            var baseUri = string.Format(EndPoints.ShowPost, postId);
            var result = await _webManager.GetDataAsync(baseUri, token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            var post = document.QuerySelector("table.post");
            return PostHandler.ParsePost(document, post);
        }

        public async Task<NewThread> GetThreadCookiesAsync(int forumId, CancellationToken token = new CancellationToken())
        {
            string url = string.Format(EndPoints.NewThread, forumId);
            var result = await _webManager.GetDataAsync(url, token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return ThreadHandler.ParseNewThread(document);
        }

        public async Task<Result> CreateNewThreadAsync(NewThread newThreadEntity, CancellationToken token = new CancellationToken())
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");
            var form = new MultipartFormDataContent
            {
                {new StringContent("postthread"), "action"},
                {new StringContent(newThreadEntity.ForumId.ToString(CultureInfo.InvariantCulture)), "forumid"},
                {new StringContent(newThreadEntity.FormKey), "formkey"},
                {new StringContent(newThreadEntity.FormCookie), "form_cookie"},
                {new StringContent(newThreadEntity.PostIcon.Id.ToString(CultureInfo.InvariantCulture)), "iconid"},
                {new StringContent(HtmlHelpers.HtmlEncode(newThreadEntity.Subject)), "subject"},
                {new StringContent(HtmlHelpers.HtmlEncode(newThreadEntity.Content)), "message"},
                {new StringContent(newThreadEntity.ParseUrl.ToString()), "parseurl"},
                {new StringContent("Submit Reply"), "submit"}
            };
            return await _webManager.PostFormDataAsync(EndPoints.NewThreadBase, form, token);
        }

        public async Task<Post> CreateNewThreadPreviewAsync(NewThread newThreadEntity, CancellationToken token = new CancellationToken())
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");
            var result = new Result();

            // We post to SA the same way we would for a normal reply, but instead of getting a redirect back to the
            // thread, we'll get redirected to back to the reply screen with the preview message on it.
            // From here we can parse that preview and return it to the user.
            var form = new MultipartFormDataContent
            {
                {new StringContent("postthread"), "action"},
                {new StringContent(newThreadEntity.ForumId.ToString(CultureInfo.InvariantCulture)), "forumid"},
                {new StringContent(newThreadEntity.FormKey), "formkey"},
                {new StringContent(newThreadEntity.FormCookie), "form_cookie"},
                {new StringContent(newThreadEntity.PostIcon.Id.ToString(CultureInfo.InvariantCulture)), "iconid"},
                {new StringContent(HtmlHelpers.HtmlEncode(newThreadEntity.Subject)), "subject"},
                {new StringContent(HtmlHelpers.HtmlEncode(newThreadEntity.Content)), "message"},
                {new StringContent(newThreadEntity.ParseUrl.ToString()), "parseurl"},
                {new StringContent("Submit Post"), "submit"},
                {new StringContent("Preview Post"), "preview"}
            };
            result = await _webManager.PostFormDataAsync(EndPoints.NewThreadBase, form, token);
            return PostHandler.ParsePostPreview(await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token));
        }

        public async Task<Result> MarkPostAsLastReadAsAsync(long threadId, int index, CancellationToken token = new CancellationToken())
        {
            try
            {
                return await _webManager.GetDataAsync(string.Format(EndPoints.LastRead, index, threadId), token);
            }
            catch (Exception ex)
            {
                return ErrorHandler.CreateErrorObject(new Result(false), ex.Message, ex.StackTrace);
            }
        }
    }
}
