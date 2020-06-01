// <copyright file="ForumManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Awful.Exceptions;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.Web;

namespace Awful.Parser.Managers
{
    public class ForumManager
    {
        private readonly WebClient webManager;

        public ForumManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        public async Task<List<Category>> GetForumCategoriesViaSelectAsync(CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var result = await this.webManager.GetDataAsync(EndPoints.ForumListPage, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return ForumHandler.ParseCategoryList(document);
        }

        public async Task<Category> GetForumDescriptionsFromCategoryPageAsync(Category category, CancellationToken token = default)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var result = await this.webManager.GetDataAsync(string.Format(CultureInfo.InvariantCulture, EndPoints.ForumPage, category.Id, token)).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return ForumHandler.ParseForumDescriptions(document, category);
        }

        public async Task<Forum> GetForumDescriptionsFromForumPageAsync(Forum forum, CancellationToken token = default)
        {
            if (forum == null)
            {
                throw new ArgumentNullException(nameof(forum));
            }

            if (forum.SubForums.Count <= 0)
            {
                return forum;
            }

            var result = await this.webManager.GetDataAsync(string.Format(CultureInfo.InvariantCulture, EndPoints.ForumPage, forum.ForumId, token)).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return ForumHandler.ParseSubForumDescriptions(document, forum);
        }

        public async Task<List<Category>> GetForumCategoriesAsync(CancellationToken token = default)
        {
            var result = await this.webManager.GetDataAsync(EndPoints.BaseUrl, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return ForumHandler.ParseCategoryList(document);
        }
    }
}
