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

        #region ����
        /**//// <summary>
        /// ����
        /// </summary>
        public string title
        {
            get{return _title;}
            set{_title = value.ToString();}
        }
		/**//// <summary>
		/// ����
		/// </summary>
		public bool IsUpdated
		{
			get{return _isUpdated;}
			set{_isUpdated = value;}
		}
        /// <summary>
        /// ����
        /// </summary>
        public string link
       {
            get{return _link;}
            set{_link = value.ToString();}
        }
        /// <summary>
        /// ����
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
