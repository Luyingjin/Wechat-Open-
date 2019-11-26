using System;

namespace Utility.Rss
{
    /**//// <summary>
    /// rssItem 的摘要说明。
    /// </summary>
    public class Item
    {
        private string _title;
        private string _link;
        private string _description;
        private string _pubDate;
		 private string _pubUser;

        #region 属性

        /**//// <summary>
        /// 标题
        /// </summary>
        public string title
        {
            get{return _title;}
            set{_title=value.ToString();}
        }
        /**//// <summary>
        /// 链接
        /// </summary>
        public string link
        {
            get{return _link;}
            set{_link=value.ToString();}
        }
        /**//// <summary>
        /// 描述
        /// </summary>
        public string description
        {
            get{return _description;}
            set{_description=value.ToString();}
        }
        /**//// <summary>
        /// 频道内容发布日期
        /// </summary>
        public string pubDate
        {
            get{return _pubDate;}
            set{_pubDate=C_Date(value);}
        }
		/**//// <summary>
		/// 作者
		/// </summary>
		public string pubUser
		{
			get{return _pubUser;}
			set{_pubUser=value.ToString();}
		}

        #endregion

        public Item(){}

        private string C_Date(string input)
        {
            System.DateTime dt;
            try
            {
                dt=Convert.ToDateTime(input);
            }
            catch
            {
                dt=System.DateTime.Now;
            }
            return dt.ToString();
        }

    }
	public class ChannelImage
	{
		private string _title;
		private string _link;
		private string _imageUrl;
		public ChannelImage(){}
		public string Title
		{
			get{return _title;}
			set{_title=value;}
		}
		public string Link
		{
			get{return _link;}
			set{_link=value;}
		}
		public string ImageUrl
		{
			get{return _imageUrl;}
			set{_imageUrl=value;}
		}

	}
}
