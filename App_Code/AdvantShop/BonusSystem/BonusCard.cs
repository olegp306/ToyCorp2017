using System;

namespace AdvantShop.BonusSystem
{
    public class BonusCard
    {
        public long CardNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public bool Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string CellPhone { get; set; }
        public string Contacts { get; set; }
        public bool NewsSubscription { get; set; }
        public bool Blocked { get; set; }
        public string CityName { get; set; }

        public float BonusPercent { get; set; }
        public float BonusAmount { get; set; }
    }
}