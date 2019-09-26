﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Awful.Parser.Models.Forums
{
	public class ForumGroup : List<Forum>
	{
		public Category Category { get; private set; }

		public ForumGroup(Category category, List<Forum> forums) : base(forums)
		{
			Category = category;
		}
	}
}
