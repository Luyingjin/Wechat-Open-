using System;

namespace Utility.Rss
{
    /**//// <summary>
    /// channel 
    /// </summary>
    [Serializable()]
    public class Channel
    {
        private string _title;
        private string _link;
        private string _description;
		private bool _isUpdated;
        private ItemCollection items = new ItemCollection();
		private ChannelImage image = new ChannelImage();

        #region 属性
        /**//// <summary>
        /// 标题
        /// </summary>
        public string title
        {
            get{return _title;}
            set{_title = value.ToString();}
        }
		/**//// <summary>
		/// 标题
		/// </summary>
		public bool IsUpdated
		{
			get{return _isUpdated;}
			set{_isUpdated = value;}
		}
        /// <summary>
        /// 链接
        /// </summary>
        public string link
       {
            get{return _link;}
            set{_link = value.ToString();}
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string description
        {
            get{return _description;}
            set{_description = value.ToString();}
        }
        public ItemCollection Items
        {
            get{ return items; }
        }
		public ChannelImage Image
		{
			get{ return image;}
		}
        #endregion
    }
}
