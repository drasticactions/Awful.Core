// <copyright file="PrivateMessageManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Awful.Exceptions;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Messages;
using Awful.Parser.Models.Posts;
using Awful.Parser.Models.Web;

namespace Awful.Parser.Managers
{
    public class PrivateMessageManager
    {
        private readonly WebClient webManager;

        public PrivateMessageManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        public async Task<List<PrivateMessage>> GetAllPrivateMessageListAsync(CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var pmList = new List<PrivateMessage>();
            var page = 0;
            while (true)
            {
                var result = await this.GetPrivateMessageListAsync(page, token).ConfigureAwait(false);
                pmList.AddRange(result);
                if (!result.Any())
                {
                    break;
                }

                page++;
            }

            return pmList;
        }

        public async Task<List<PrivateMessage>> GetPrivateMessageListAsync(int page, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var url = EndPoints.PrivateMessages;
            if (page > 0)
            {
                url = EndPoints.PrivateMessages + string.Format(CultureInfo.InvariantCulture, EndPoints.PageNumber, page);
            }

            var result = await this.webManager.GetDataAsync(url, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return PrivateMessageHandler.ParseList(document);
        }

        public async Task<Post> GetPrivateMessageAsync(int id, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var pm = new PrivateMessage() { Id = id };
            await this.GetPrivateMessageAsync(pm, token).ConfigureAwait(false);
            return pm.Post;
        }

        public async Task<Post> GetPrivateMessageAsync(PrivateMessage message, CancellationToken token = default)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var result = await this.webManager.GetDataAsync(EndPoints.PrivateMessages + $"?action=show&privatemessageid={message.Id}", token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            message.Post = PostHandler.ParsePost(document, document.Body);
            return message.Post;
        }

        public async Task<Result> SendPrivateMessageAsync(NewPrivateMessage newPrivateMessageEntity, CancellationToken token = default)
        {
            if (newPrivateMessageEntity == null)
            {
                throw new ArgumentNullException(nameof(newPrivateMessageEntity));
            }

            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            using var form = new MultipartFormDataContent
            {
                { new StringContent("dosend"), "action" },
                { new StringContent(newPrivateMessageEntity.Receiver), "touser" },
                { new StringContent(HtmlHelpers.HtmlEncode(newPrivateMessageEntity.Title)), "title" },
                { new StringContent(HtmlHelpers.HtmlEncode(newPrivateMessageEntity.Body)), "message" },
                { new StringContent("yes"), "parseurl" },
                { new StringContent("yes"), "parseurl" },
                { new StringContent("Send Message"), "submit" },
            };
            if (newPrivateMessageEntity.Icon != null)
            {
                form.Add(new StringContent(newPrivateMessageEntity.Icon.Id.ToString(CultureInfo.InvariantCulture)), "iconid");
            }

            return await this.webManager.PostFormDataAsync(EndPoints.NewPrivateMessageBase, form, token).ConfigureAwait(false);
        }
    }
}
