//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using System;
using AdvantShop.DownloadableContent.Interfaces;

namespace AdvantShop.DownloadableContent
{
    [Serializable]
    public class DownloadableContentObject : IDownloadableContent
    {
        public int Id { get; set; }
        public string StringId { get; set; }
        public string Version { get; set; }
        public bool Active { get; set; }

        public string Name { get; set; }
        public float Price { get; set; }
        public string Currency { get; set; }

        public string DetailsLink { get; set; }
        public string BriefDescription { get; set; }

        public int SortOrder { get; set; }
        public string Icon { get; set; }

        public string CurrentVersion { get; set; }
        public bool IsLocalVersion { get; set; }
        public bool IsInstall { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

    }
}