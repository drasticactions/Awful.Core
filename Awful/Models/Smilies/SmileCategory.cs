﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Awful.Models.Smilies
{
    public class SmileCategory
    {
        public SmileCategory()
        {
            SmileList = new List<Smile>();
        }

        public List<Smile> SmileList { get; set; }

        public string Name { get; set; }
    }

    public class Smile
    {
        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public void Parse(HtmlNode smileNode)
        {
            Title = smileNode.Descendants("div").First().InnerText;
            ImageUrl = smileNode.Descendants("img").First().GetAttributeValue("src", string.Empty);
        }
    }
}
