namespace AdvantShop.Repository
{
    public class IpZoneModel
    {
        public int countryId { get; set; }
        public string country { get; set; }
        public string region { get; set; }
        public string city { get; set; }
        public int cityId { get; set; }
        public string phone { get; set; }
    }
}