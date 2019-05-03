using System;
using Awful.Parser.Models.Posts;
using HandlebarsDotNet;

namespace Awful.Web.Templates
{
    public class PostTemplate
    {
        public static string Template =
            @"
            <div id=""{{ PostID }}""
            class=""
                {{ #if HasSeen }} seen {{/if}}
                {{ User.Roles }}
                {{ #if User.AvatarLink }} avatar {{else}} no-avatar {{/if}}
                "">
                <header>
                    {{ #if User.AvatarLink }}
                        <img class=""avatar"" src=""{{ User.AvatarLink }}"" alt="""">
                    {{/if}}
                    <section class=""nameanddate"">
                    <h1 class=""username"">
                        {{ User.Username }}
                    </h1>
                    <span class=""regdate"">
                        Joined {{ User.DateJoined }}
                    </span>
                    </section>
                </header>
                <section class=""postbody"">
                    {{ PostHtml }}
                </section>
                <footer>
                    <span class=""postdate"">
                        {{ PostDate }}
                    </span>
                </footer>
            </div>
            ";

        public static string Render(Post post)
        {
            var roles = post.PostDate;
            var template = Handlebars.Compile(Template);
            var result = template(post);
            return result;
        }
    }
}
