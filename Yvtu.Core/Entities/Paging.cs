using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class Paging
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling(Count / (double)PageSize);
            }
        }
        public bool HasNext
        {
            get
            {
                if (PageNo == TotalPages)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool HasPrevious {
            get
            {
                if (PageNo == 1)
                {
                    return false;
                } 
                else
                {
                    return true;
                }
            }
        }
    }
}
