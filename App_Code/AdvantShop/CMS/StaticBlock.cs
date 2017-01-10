//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.CMS
{
    public class StaticBlock
    {
        private readonly int _id;
        public int StaticBlockId
        {
            get { return _id; }
        }

        public string Key { get; set; }

        public string InnerName { get; set; }

        public string Content { get; set; }

        public DateTime Added { get; set; }

        public DateTime Modified { get; set; }

        public bool Enabled { get; set; }

        public StaticBlock()
        {
            _id = 0;
            Key = string.Empty;
            InnerName = string.Empty;
            Content = string.Empty;
            Added = DateTime.Now;
            Modified = DateTime.Now;
            Enabled = false;
        }

        public StaticBlock(int id)
        {
            _id = id;
            Key = string.Empty;
            InnerName = string.Empty;
            Content = string.Empty;
            Added = DateTime.Now;
            Modified = DateTime.Now;
            Enabled = false;
        }
    }
}