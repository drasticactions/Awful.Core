using Awful.Parser.Core;
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
            var result = await _webManager.GetDataAsync(EndPoints.SAclopediaBase, token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return SAclopediaHandler.ParseCategoryList(document);
        }

        public async Task<List<SAclopediaEntryItem>> GetEntryItemListAsync(int id, int act = 5, CancellationToken token = new CancellationToken())
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");
            var result = await _webManager.GetDataAsync(EndPoints.SAclopediaBase + $"?act={act}&i={id}", token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return SAclopediaHandler.ParseEntryItemList(document);
        }

        public async Task<SAclopediaEntry> GetEntryAsync(int id, int act = 3, CancellationToken token = new CancellationToken())
        {
            if (!_webManager.IsAuthenticated)
                throw new Exception("User must be authenticated before using this method.");
            var result = await _webManager.GetDataAsync(EndPoints.SAclopediaBase + $"?act={act}& topicid={id}", token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return SAclopediaHandler.ParseEntry(document, id);
        }
    }
}
