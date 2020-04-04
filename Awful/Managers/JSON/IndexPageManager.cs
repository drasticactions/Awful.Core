using Awful.Core.Models.JSON;
using Awful.Parser.Core;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IndexPage> GetIndexPageAsync (bool addAdditionalMetadata = false, CancellationToken token = new CancellationToken())
        {
            var result = await _webManager.GetDataAsync(EndPoints.IndexPageUrl, token);
            if (!result.IsSuccess)
                throw new Exception("Could not get Index page JSON");

            if (!addAdditionalMetadata)
                return JsonSerializer.Deserialize<IndexPage>(result.ResultHtml);

            var data = JsonSerializer.Deserialize<IndexPage>(result.ResultHtml);

            foreach (var forum in data.Forums)
                UpdateForumMetadata(forum);

            return data;
        }

        private void UpdateForumMetadata (Forum forum, Forum parentForum = null)
        {
            if (parentForum != null)
                forum.ParentId = parentForum.Id;

            if (forum.SubForums == null)
                return;

            foreach (var subForum in forum.SubForums)
                UpdateForumMetadata(subForum, forum);
        }
    }
}
