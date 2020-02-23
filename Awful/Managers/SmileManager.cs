using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Smilies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Awful.Parser.Managers
{
    public class SmileManager
    {
        private readonly WebClient _webManager;

        public SmileManager(WebClient webManager)
        {
            _webManager = webManager;
        }
        public async Task<List<SmileCategory>> GetSmileListAsync(CancellationToken token = new CancellationToken())
        {
            var result = await _webManager.GetDataAsync(EndPoints.SmileUrl, token);
            var document = await _webManager.Parser.ParseDocumentAsync(result.ResultHtml, token);
            return SmileHandler.ParseSmileList(document);
        }
    }
}
