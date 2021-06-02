using System.Collections.Generic;
using SugarChat.Core.Domain;

namespace SugarChat.Core.IRepositories
{
    public class PagedResult<T> where T : class, IEntity
    {
        public IEnumerable<T> Result { get; set; }
        public int Total { get; set; }
    }
}