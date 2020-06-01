using System;
using System.Collections.Generic;
using System.ComponentModel;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.Posts;

namespace Awful.Parser.Models.Threads
{
    public class ThreadList
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public List<Thread> Threads { get; set; }
    }
}
