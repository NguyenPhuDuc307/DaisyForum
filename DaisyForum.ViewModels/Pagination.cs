using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaisyForum.ViewModels
{
    public class Pagination<T>
    {
        public List<T>? Items { get; set; }
        public int TotalRecords { get; set; }
    }
}