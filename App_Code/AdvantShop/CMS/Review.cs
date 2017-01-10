//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using AdvantShop.Customers;

namespace AdvantShop.CMS
{
    public enum EntityType
    {
        Product = 0
    }

    public class Review
    {
        public int ReviewId { get; set; }
        public int EntityId { get; set; }
        public int ParentId { get; set; }
        public EntityType Type { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public DateTime AddDate { get; set; }
        public string Ip { get; set; }

        
        private Review parent;
        public Review Parent
        {
            get { return parent ?? (parent = ReviewService.GetReview(ParentId)); }
        }

        private List<Review> children;
        public List<Review> Children
        {
            get { return children ?? (children = ReviewService.GetReviewChildren(ReviewId)); }
        }

        private Customer customer;
        public Customer Customer
        {
            get { return customer ?? (customer = CustomerService.GetCustomer(CustomerId)); }
        }
    }
}