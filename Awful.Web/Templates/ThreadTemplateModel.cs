using System;
using System.Linq;
using Awful.Parser.Models.Threads;

namespace Awful.Web.Templates
{
    public class ThreadTemplateModel
    {
        public ThreadTemplateModel()
        {
            //Thread.Posts.First().User.Id
        }

        public Thread Thread { get; set; }

        public string Css { get; set; }
    }
}
