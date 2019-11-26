using System;

namespace Utility.Rss
{
    /// <summary>
    /// rssChannelCollection 的摘要说明。
    /// </summary>
    public class ChannelCollection : System.Collections.CollectionBase
    {
        public Channel this[int index]
        {
            get 
            { 
                return ((Channel)(List[index])); 
            }
            set 
            { 
                List[index] = value;
            }
        }

        public int Add(Channel item)
        {
            return List.Add(item);
        }
    }
}
