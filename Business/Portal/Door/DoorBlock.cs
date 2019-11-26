using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Door
{
    public class DoorBlock
    {    
        public string AllowTypes { get; set; }        
        public string AllowUserIds { get; set; }      
        public string AllowUserNames { get; set; }
        public string BlockImage { get; set; }        
        public string BlockKey { get; set; }        
        public string BlockName { get; set; }
        public string BlockTitle { get; set; }
        public string BlockType { get; set; }
        public string Color { get; set; }
        public string ColorValue { get; set; }
        public int? DelayLoadSecond { get; set; }
        public string FootHtml { get; set; }
        public string HeadHtml { get; set; }
        public string ID { get; set; }
        public string IsEdit { get; set; }
        public string IsHidden { get; set; }
        public string RelateScript { get; set; }
        public string Remark { get; set; }
        public string RepeatDataDataSql { get; set; }
        public int? RepeatItemCount { get; set; }
        public int? RepeatItemLength { get; set; }
        public string RepeatItemTemplate { get; set; }
        public double? SortIndex { get; set; }
        public string TemplateId { get; set; }
    }
}