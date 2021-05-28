using System.Collections.Generic;

namespace SugarChat.Shared.Paging
{
    public class PagedResult<T> where T : class
    {
        public IEnumerable<T> Result { get; set; }
        public int Total { get; set; }
    }
}