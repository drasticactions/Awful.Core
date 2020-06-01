// <copyright file="SAclopediaManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Awful.Exceptions;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.SAclopedia;

namespace Awful.Parser.Managers
{
    public class SAclopediaManager
    {
        private readonly WebClient webManager;

        public SAclopediaManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        public async Task<List<SAclopediaCategory>> GetCategoryListAsync(CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var result = await this.webManager.GetDataAsync(EndPoints.SAclopediaBase, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return SAclopediaHandler.ParseCategoryList(document);
        }

        public async Task<List<SAclopediaEntryItem>> GetEntryItemListAsync(int id, int act = 5, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var result = await this.webManager.GetDataAsync(EndPoints.SAclopediaBase + $"?act={act}&i={id}", token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return SAclopediaHandler.ParseEntryItemList(document);
        }

        public async Task<SAclopediaEntry> GetEntryAsync(int id, int act = 3, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var result = await this.webManager.GetDataAsync(EndPoints.SAclopediaBase + $"?act={act}& topicid={id}", token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return SAclopediaHandler.ParseEntry(document, id);
        }
    }
}
