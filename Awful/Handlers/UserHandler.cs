using System;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Awful.Parser.Models.Users;
namespace Awful.Parser.Handlers
{
    public class UserHandler
    {
        public static User ParseUserFromProfilePage(long userId, IHtmlDocument doc)
        {
            var user = new User();
            user.Id = userId;

            var authorTd = doc.QuerySelector(@"[class*=""userinfo""]");
            ParseUserInfoElement(user, authorTd);
            // If we're trying to get the current user,
            // get the real ID from the page.
            if (userId == 0)
                ParseUserId(user, doc);
            return user;
        }

        public static void ParseUserId (User user, IHtmlDocument doc)
        {
            var input = doc.QuerySelector(@"input[name=""userid""]");
            if (input != null)
            {
                var inputNum = input.GetAttribute("value");
                if (!string.IsNullOrEmpty(inputNum))
                    user.Id = Convert.ToInt64(inputNum);
            }
        }

        public static User ParseUserFromPost(IElement doc)
        {
            var user = new User();

            var authorTd = doc.QuerySelector(@"[class*=""userid""]");

            var userId = authorTd.ClassList.First(n => n.Contains("userid-")).Trim().Replace("userid-", string.Empty);
            user.Id = Convert.ToInt64(userId);

            ParseUserInfoElement(user, authorTd);

            return user;
        }

        private static void ParseUserInfoElement (User user, IElement authorTd)
        {
            var authorTitle = authorTd.QuerySelector(".author");
            user.Username = authorTitle.TextContent;
            user.Roles = authorTitle.ClassName;
            user.Title = authorTitle.GetAttribute("title");
            var userTitleHtml = authorTd.QuerySelector(".title");

            var registered = authorTd.QuerySelector(".registered");

            if (registered != null)
            {
                DateTime reg = new DateTime();
                DateTime.TryParse(registered.TextContent, out reg);
                user.DateJoined = reg;
            }

            if (userTitleHtml == null)
                return;

            user.AvatarHtml = userTitleHtml.InnerHtml;
            user.AvatarTitle = userTitleHtml.TextContent.Trim();

            var userImgs = userTitleHtml.QuerySelectorAll(@"img");
            if (userImgs != null && userImgs.Any())
                user.AvatarLink = userImgs.First().GetAttribute("src");

            if (user.AvatarLink == null)
            {
                var userImgAv = userTitleHtml.QuerySelector(@"img[src*=""titles""]");
                if (userImgAv != null)
                    user.AvatarLink = userImgAv.GetAttribute("src");
            }

            var gangTagImg = userTitleHtml.QuerySelector(@"img[src*=""gangtags""]");
            if (gangTagImg != null)
                user.AvatarGangTagLink = gangTagImg.GetAttribute("src");

            if (user.Id == 0)
            {

            }
        }
    }
}
