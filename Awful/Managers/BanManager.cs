// <copyright file="BanManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Bans;

namespace Awful.Parser.Managers
{
    public class BanManager
    {
        private readonly WebClient _webManager;

        public BanManager(WebClient webManager)
        {
            _webManager = webManager;
        }

        public async Task<BanPage> GetBanPageAsync(int page = 1, CancellationToken token = default)
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");

            var result = await _webManager.GetDataAsync(string.Format(EndPoints.RapSheet, page), token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return BanHandler.ParseBanPage(document);
        } 

        public async Task<ProbationItem> CheckForProbation(CancellationToken token = default)
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");

            var result = await _webManager.GetDataAsync(EndPoints.BaseUrl, token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            var prob = BanHandler.ParseForProbation(document);
            _webManager.Probation = prob;
            return prob;
        }
    }
}
