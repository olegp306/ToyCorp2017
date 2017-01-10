//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;

namespace AdvantShop.Modules
{
    public class StoreReview
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string ReviewerEmail { get; set; }
        public string ReviewerName { get; set; }
        public string Review { get; set; }
        public int Rate { get; set; }
        public bool Moderated { get; set; }
        public bool HasChild { get; set; }
        public DateTime DateAdded { get; set; }
        public List<StoreReview> ChildrenReviews { get; set; }
        public int Level { get; set; }
    }
}