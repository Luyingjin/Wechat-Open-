using System;

namespace Utility.Rss
{
    /**//// <summary>
    /// rssChannelCollection 的摘要说明。
    /// </summary>
    public class ItemCollection : System.Collections.CollectionBase
    {
        public Item this[int index]
        {
            get { return ((Item)(List[index])); }
            set 
            { 
                List[index] = value;
            }
        }
        public int Add(Item item)
        {
            return List.Add(item);
        }
    }
}
