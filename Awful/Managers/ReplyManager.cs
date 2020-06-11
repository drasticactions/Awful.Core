// <copyright file="ReplyManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Posts;
using Awful.Parser.Models.Replies;
using Awful.Parser.Models.Web;

namespace Awful.Core.Managers
{
    public class ReplyManager
    {
        private readonly WebClient webManager;

        public ReplyManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        public async Task<ForumReply> GetReplyCookiesForEditAsync(long postId, CancellationToken token = default)
        {
            if (postId <= 0)
            {
                throw new FormatException(Awful.Core.Resources.ExceptionMessages.PostIdMissing);
            }

            string url = string.Format(CultureInfo.InvariantCulture, EndPoints.EditBase, postId);
            var result = await this.webManager.GetDataAsync(url, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            var inputs = document.QuerySelectorAll("input");
            var forumReplyEntity = new ForumReply();
            var bookmarks = inputs["bookmark"].HasAttribute("checked") ? "yes" : "no";
            string quote = System.Net.WebUtility.HtmlDecode(document.QuerySelector("textarea").TextContent);
            forumReplyEntity.MapEditPostInformation(
                quote,
                postId,
                bookmarks);
            return forumReplyEntity;
        }

        public async Task<ForumReply> GetReplyCookiesAsync(long threadId = 0, long postId = 0, CancellationToken token = default)
        {
            if (threadId == 0 && postId == 0)
            {
                throw new FormatException(Awful.Core.Resources.ExceptionMessages.ThreadAndPostIdMissing);
            }

            string url;
            url = threadId > 0 ? string.Format(CultureInfo.InvariantCulture, EndPoints.ReplyBase, threadId) : string.Format(CultureInfo.InvariantCulture, EndPoints.QuoteBase, postId);
            var result = await this.webManager.GetDataAsync(url, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            var inputs = document.QuerySelectorAll("input");
            var posts = ThreadHandler.ParsePreviousPosts(document);
            var forumReplyEntity = new ForumReply();
            string quote = System.Net.WebUtility.HtmlDecode(document.QuerySelector("textarea").TextContent);
            forumReplyEntity.MapThreadInformation(
                inputs["formkey"].GetAttribute("value"),
                inputs["form_cookie"].GetAttribute("value"),
                quote,
                inputs["threadid"].GetAttribute("value"));
            forumReplyEntity.ForumPosts.AddRange(posts);
            return forumReplyEntity;
        }

        public async Task<Result> SendPostAsync(ForumReply forumReplyEntity, CancellationToken token = default)
        {
            if (forumReplyEntity == null)
            {
                throw new ArgumentNullException(nameof(forumReplyEntity));
            }

            using var form = new MultipartFormDataContent
            {
                { new StringContent("postreply"), "action" },
                { new StringContent(forumReplyEntity.ThreadId), "threadid" },
                { new StringContent(forumReplyEntity.FormKey), "formkey" },
                { new StringContent(forumReplyEntity.FormCookie), "form_cookie" },
                { new StringContent(HtmlHelpers.HtmlEncode(forumReplyEntity.Message)), "message" },
                { new StringContent(forumReplyEntity.ParseUrl.ToString()), "parseurl" },
                { new StringContent("2097152"), "MAX_FILE_SIZE" },
                { new StringContent("Submit Reply"), "submit" },
            };
            return await this.webManager.PostFormDataAsync(EndPoints.NewReply, form, token).ConfigureAwait(false);
        }

        public async Task<Result> SendUpdatePostAsync(ForumReply forumReplyEntity, CancellationToken token = default)
        {
            if (forumReplyEntity == null)
            {
                throw new ArgumentNullException(nameof(forumReplyEntity));
            }

            using var form = new MultipartFormDataContent
            {
                { new StringContent("updatepost"), "action" },
                { new StringContent(forumReplyEntity.PostId.ToString(CultureInfo.InvariantCulture)), "postid" },
                { new StringContent(HtmlHelpers.HtmlEncode(forumReplyEntity.Message)), "message" },
                { new StringContent(forumReplyEntity.ParseUrl.ToString(CultureInfo.InvariantCulture)), "parseurl" },
                { new StringContent(forumReplyEntity.Bookmark), "bookmark" },
                { new StringContent("2097152"), "MAX_FILE_SIZE" },
                { new StringContent("Save Changes"), "submit" },
            };
            return await this.webManager.PostFormDataAsync(EndPoints.EditPost, form, token).ConfigureAwait(false);
        }

        public async Task<Post> CreatePreviewPostAsync(ForumReply forumReplyEntity, CancellationToken token = default)
        {
            if (forumReplyEntity == null)
            {
                throw new ArgumentNullException(nameof(forumReplyEntity));
            }

            using var form = new MultipartFormDataContent
            {
                { new StringContent("postreply"), "action" },
                { new StringContent(forumReplyEntity.ThreadId), "threadid" },
                { new StringContent(forumReplyEntity.FormKey), "formkey" },
                { new StringContent(forumReplyEntity.FormCookie), "form_cookie" },
                { new StringContent(HtmlHelpers.HtmlEncode(forumReplyEntity.Message)), "message" },
                { new StringContent(forumReplyEntity.ParseUrl.ToString()), "parseurl" },
                { new StringContent("2097152"), "MAX_FILE_SIZE" },
                { new StringContent("Submit Reply"), "submit" },
                { new StringContent("Preview Reply"), "preview" },
            };

            // We post to SA the same way we would for a normal reply, but instead of getting a redirect back to the
            // thread, we'll get redirected to back to the reply screen with the preview message on it.
            // From here we can parse that preview and return it to the user.
            var result = await this.webManager.PostFormDataAsync(EndPoints.NewReply, form).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return new Post { PostHtml = document.QuerySelector(".postbody").InnerHtml };
        }

        public async Task<Post> CreatePreviewEditPostAsync(ForumReply forumReplyEntity, CancellationToken token = default)
        {
            if (forumReplyEntity == null)
            {
                throw new ArgumentNullException(nameof(forumReplyEntity));
            }

            using var form = new MultipartFormDataContent
            {
                { new StringContent("updatepost"), "action" },
                { new StringContent(forumReplyEntity.PostId.ToString(CultureInfo.InvariantCulture)), "postid" },
                { new StringContent(HtmlHelpers.HtmlEncode(forumReplyEntity.Message)), "message" },
                { new StringContent(forumReplyEntity.ParseUrl.ToString()), "parseurl" },
                { new StringContent("2097152"), "MAX_FILE_SIZE" },
                { new StringContent("Preview Post"), "preview" },
            };
            var result = await this.webManager.PostFormDataAsync(EndPoints.EditPost, form, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return new Post { PostHtml = document.QuerySelector(".postbody").InnerHtml };
        }

        public async Task<string> GetQuoteStringAsync(long postId, CancellationToken token = default)
        {
            if (postId <= 0)
            {
                throw new FormatException(Awful.Core.Resources.ExceptionMessages.PostIdMissing);
            }

            string url = string.Format(CultureInfo.InvariantCulture, EndPoints.QuoteBase, postId);
            var result = await this.webManager.GetDataAsync(url, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return System.Net.WebUtility.HtmlDecode(System.Net.WebUtility.HtmlDecode(document.QuerySelector("textarea").TextContent));
        }
    }
}
