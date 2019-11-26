using System;
using System.Xml;
using System.IO;
using System.Net;
using System.Text;

namespace Utility.Rss
{
    /**//// <summary>
    /// rssFeed 的摘要说明。
    /// </summary>
    public class Feed
    {
        private string _url;
        private System.DateTime _lastModified;
        private System.DateTime _lastRssDate;
        private Channel channel = new Channel();

        #region 公共属性
        public string url
        {
            get{return _url;}
            set{_url=value;}
        }
        public System.DateTime lastModified
        {
            get{return _lastModified;}
        }
        public System.DateTime lstRssDate
        {
            set{_lastRssDate=value;}
        }
        public Channel Channel
        {
            get { return channel; }
        }        
        #endregion


        public Feed()
        {
        }

        public Feed(string url,System.DateTime dt)
        {
            this._url=url;
            this._lastRssDate=dt;
        }

        public void Read()
        {
            XmlDocument xDoc=new XmlDocument();
			xDoc.Load(_url);
			channel.title = xDoc.DocumentElement["channel"].SelectSingleNode("title").InnerText;
			XmlNode node = xDoc.DocumentElement["channel"].SelectSingleNode("image");
			if(node!=null)
			{
				channel.Image.Title = node.SelectSingleNode("title").InnerText;
				channel.Image.Link = node.SelectSingleNode("link").InnerText;
				channel.Image.ImageUrl = node.SelectSingleNode("url").InnerText;
			}
            XmlNodeList xnList=xDoc.DocumentElement["channel"].SelectNodes("item");

            int a= xnList.Count;
            foreach(XmlNode xNode in xnList)
            {                
                Item rt=new Item();
                rt.title=xNode.SelectSingleNode("title").InnerText.Replace("'","''");
                rt.link=xNode.SelectSingleNode("link").InnerText.Replace("'","''");
				rt.pubUser=xNode.SelectSingleNode("source")==null?"":xNode.SelectSingleNode("source").InnerText.Replace("'","''");
                rt.description=xNode.SelectSingleNode("description").InnerText.Replace("'","''");
                try
                {
                    rt.pubDate=xNode.SelectSingleNode("pubDate").InnerText;
                }
                catch
                {
                    rt.pubDate=this._lastModified.ToString();
                }
                channel.Items.Add(rt);
            }
        }



        public string Create()
        {
            return "";
        }

        private string Get_CH(string s)
        {
            int l=s.IndexOf("charset=")+8;
            return s.Substring(l,s.Length-l);
        }

    }
}
