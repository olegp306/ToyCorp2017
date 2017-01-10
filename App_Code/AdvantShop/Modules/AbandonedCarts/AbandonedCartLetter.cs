using System;

namespace AdvantShop.Modules
{
    public class AbandonedCartLetter
    {
        public int Id { get; set; }

        public int TemplateId { get; set; }

        public Guid CustomerId { get; set; }

        public string Email { get; set; }

        public DateTime SendingDate { get; set; }
    }
}