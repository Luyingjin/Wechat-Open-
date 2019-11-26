using System;

namespace Utility.Rss
{
    /**//// <summary>
    /// rssChannelCollection ��ժҪ˵����
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
