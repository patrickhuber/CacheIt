using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Collections
{
    public class CacheLinkedListNode<T>
    {        
        public string Previous { get; set; }
        public string Next { get; set; }
        public T Data { get; set; }

        public bool HasNext()
        {
            return !string.IsNullOrWhiteSpace(Next);
        }

        public bool HasPrevious()
        {
            return !string.IsNullOrWhiteSpace(Previous);
        }
    }
}
