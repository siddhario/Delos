using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Model
{
    public class QueryResult
    {
        public IEnumerable<object> data { get; set; }
        public int pageCount { get; set; }
        public int resultCount { get; set; }
    }
}
