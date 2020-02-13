using System.Collections.Generic;
using System.IO;
using System.Linq;
using AngleSharp.Html.Dom;
using Awful.Parser.Models.Smilies;

namespace Awful.Parser.Handlers
{
    public class SmileHandler
    {
        public static List<SmileCategory> ParseSmileList(IHtmlDocument document)
        {
            var innerList = document.QuerySelector(".inner");
            var categoriesHeader = innerList.QuerySelectorAll("h3").Select(n => new SmileCategory() { Name = n.TextContent }).ToList();
            var smileGroups = document.QuerySelectorAll(".smilie_group");
            for(var i = 0; i < smileGroups.Count(); i++)
            {
                var smileGroup = smileGroups[i];
                var smileCategory = categoriesHeader[i];
                var smiles = smileGroup.QuerySelectorAll(".smilie");
                foreach(var smile in smiles)
                {
                    var image = smile.QuerySelector("img").GetAttribute("src");
                    smileCategory.SmileList.Add(new Smile()
                    {
                        Category = smileCategory.Name,
                        Title = smile.TextContent.Trim(),
                        ImageUrl = image,
                        ImageLocation = Path.GetFileNameWithoutExtension(image)
                    });
                }
            }
            return categoriesHeader;
        }
    }
}
