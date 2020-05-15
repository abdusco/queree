using System;
using Microsoft.AspNetCore.Mvc;

namespace Queree.DynamicQuery
{
    public class Query
    {
        private const int MaxPageSize = 50;
        private int _top = MaxPageSize;
        [FromQuery(Name = "$skip")] public int Skip { get; set; } = 0;

        [FromQuery(Name = "$top")]
        public int Top
        {
            get => _top;
            set => _top = Math.Min(MaxPageSize, value);
        }

        [FromQuery(Name = "$filter")] public string Filter { get; set; }
        [FromQuery(Name = "$orderby")] public string OrderBy { get; set; }
    }
}