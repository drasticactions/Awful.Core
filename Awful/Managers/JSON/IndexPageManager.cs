using Awful.Core.Models.JSON;
using Awful.Parser.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Awful.Core.Managers.JSON
{
    public class IndexPageManager
    {
        private readonly WebClient _webManager;

        public IndexPageManager(WebClient webManager)
        {
            _webManager = webManager;
        }

        public async Task<IndexPage> GetIndexPageAsync (CancellationToken token = new CancellationToken())
        {
            var result = await _webManager.GetDataAsync(EndPoints.IndexPageUrl, token);
            if (!result.IsSuccess)
                throw new Exception("Could not get Index page JSON");

            return JsonConvert.DeserializeObject<IndexPage>(result.ResultHtml);
        }
    }
}
