using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReqresInApiClient.Models
{
    public class PagedResponse<T>
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; }
    }

    public class SingleUserResponse<T>
    {
        public T Data { get; set; }
    }
}
