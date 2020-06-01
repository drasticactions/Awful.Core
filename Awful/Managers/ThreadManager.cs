// <copyright file="ThreadManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Awful.Exceptions;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.Posts;
using Awful.Parser.Models.Threads;
using Awful.Parser.Models.Web;

namespace Awful.Parser.Managers
{
    /// <summary>
    /// Manager for handling Threads on Something Awful.
    /// </summary>
    public class ThreadManager
    {
        private readonly WebClient webManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadManager"/> class.
        /// </summary>
        /// <param name="webManager">The SA WebClient.</param>
        public ThreadManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        /// <summary>
        /// Mark a thread as 'Unread'.
        /// </summary>
        /// <param name="threadId">The Thread Id.</param>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A Task.</returns>
        public async Task<Result> MarkThreadUnreadAsync(long threadId, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var dic = new Dictionary<string, string>
            {
                ["json"] = "1",
                ["action"] = "resetseen",
                ["threadid"] = threadId.ToString(CultureInfo.InvariantCulture),
            };
            using var header = new FormUrlEncodedContent(dic);
            return await this.webManager.PostDataAsync(EndPoints.ShowThreadBase, header, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a thread. Can be used with or without authentication, but depending on the thread it may be behind a paywall.
        /// This should be wrapped to check for <see cref="PaywallException"/>.
        /// </summary>
        /// <param name="threadId">A Thread Id.</param>
        /// <param name="pageNumber">The page number. Defaults to 1.</param>
        /// <param name="goToNewestPost">Goes to the newest page and post in a thread. Overrides pageNumber if set to True.</param>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A Thread.</returns>
        public async Task<Models.Threads.Thread> GetThreadAsync(int threadId, int pageNumber = 1, bool goToNewestPost = false, CancellationToken token = default)
        {
            var baseUri = string.Format(CultureInfo.InvariantCulture, EndPoints.ThreadPage, threadId);
            if (goToNewestPost)
            {
                baseUri += string.Format(CultureInfo.InvariantCulture, EndPoints.GotoNewPost);
            }
            else if (pageNumber > 1)
            {
                baseUri += string.Format(CultureInfo.InvariantCulture, EndPoints.PageNumber, pageNumber);
            }

            var result = await this.webManager.GetDataAsync(baseUri, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return ThreadHandler.ParseThread(document);
        }

        public async Task<Post> GetPostAsync(int postId, CancellationToken token = default)
        {
            var baseUri = string.Format(CultureInfo.InvariantCulture, EndPoints.ShowPost, postId);
            var result = await this.webManager.GetDataAsync(baseUri, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            var post = document.QuerySelector("table.post");
            return PostHandler.ParsePost(document, post);
        }

        public async Task<NewThread> GetThreadCookiesAsync(int forumId, CancellationToken token = default)
        {
            string url = string.Format(CultureInfo.InvariantCulture, EndPoints.NewThread, forumId);
            var result = await this.webManager.GetDataAsync(url, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return ThreadHandler.ParseNewThread(document);
        }

        public async Task<Result> CreateNewThreadAsync(NewThread newThreadEntity, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            using var form = new MultipartFormDataContent
            {
                {new StringContent("postthread"), "action"},
                {new StringContent(newThreadEntity.ForumId.ToString(CultureInfo.InvariantCulture)), "forumid" },
                {new StringContent(newThreadEntity.FormKey), "formkey" },
                {new StringContent(newThreadEntity.FormCookie), "form_cookie" },
                {new StringContent(newThreadEntity.PostIcon.Id.ToString(CultureInfo.InvariantCulture)), "iconid" },
                {new StringContent(HtmlHelpers.HtmlEncode(newThreadEntity.Subject)), "subject" },
                {new StringContent(HtmlHelpers.HtmlEncode(newThreadEntity.Content)), "message" },
                {new StringContent(newThreadEntity.ParseUrl.ToString()), "parseurl" },
                {new StringContent("Submit Reply"), "submit" },
            };
            return await this.webManager.PostFormDataAsync(EndPoints.NewThreadBase, form, token).ConfigureAwait(false);
        }

        public async Task<Post> CreateNewThreadPreviewAsync(NewThread newThreadEntity, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            // We post to SA the same way we would for a normal reply, but instead of getting a redirect back to the
            // thread, we'll get redirected to back to the reply screen with the preview message on it.
            // From here we can parse that preview and return it to the user.
            using var form = new MultipartFormDataContent
            {
                {new StringContent("postthread"), "action" },
                {new StringContent(newThreadEntity.ForumId.ToString(CultureInfo.InvariantCulture)), "forumid" },
                {new StringContent(newThreadEntity.FormKey), "formkey" },
                {new StringContent(newThreadEntity.FormCookie), "form_cookie" },
                {new StringContent(newThreadEntity.PostIcon.Id.ToString(CultureInfo.InvariantCulture)), "iconid" },
                {new StringContent(HtmlHelpers.HtmlEncode(newThreadEntity.Subject)), "subject" },
                {new StringContent(HtmlHelpers.HtmlEncode(newThreadEntity.Content)), "message" },
                {new StringContent(newThreadEntity.ParseUrl.ToString()), "parseurl" },
                {new StringContent("Submit Post"), "submit" },
                {new StringContent("Preview Post"), "preview" },
            };

            var result = await this.webManager.PostFormDataAsync(EndPoints.NewThreadBase, form, token).ConfigureAwait(false);
            return PostHandler.ParsePostPreview(await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false));
        }

        public async Task<Result> MarkPostAsLastReadAsAsync(long threadId, int index, CancellationToken token = default)
        {
            return await this.webManager.GetDataAsync(string.Format(CultureInfo.InvariantCulture, EndPoints.LastRead, index, threadId), token).ConfigureAwait(false);
        }
    }
}
