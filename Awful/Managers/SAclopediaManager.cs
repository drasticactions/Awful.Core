﻿using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.SAclopedia;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Awful.Parser.Managers
{
    public class SAclopediaManager
    {
        private readonly WebClient _webManager;

        public SAclopediaManager(WebClient webManager)
        {
            _webManager = webManager;
        }

        public async Task<List<SAclopediaCategory>> GetCategoryListAsync(CancellationToken token = new CancellationToken())
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");
            var result = await _webManager.GetDataAsync(EndPoints.SAclopediaBase);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return SAclopediaHandler.ParseCategoryList(document);
        }

        public async Task<List<SAclopediaEntryItem>> GetEntryItemListAsync(int id, CancellationToken token = new CancellationToken())
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");
            var result = await _webManager.GetDataAsync(EndPoints.SAclopediaBase + $"?act=5&i={id}");
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return SAclopediaHandler.ParseEntryItemList(document);
        }

        public async Task<SAclopediaEntry> GetEntryAsync(int id, CancellationToken token = new CancellationToken())
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");
            var result = await _webManager.GetDataAsync(EndPoints.SAclopediaBase + $"?act=3&topicid={id}");
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return SAclopediaHandler.ParseEntry(document, id);
        }
    }
}
