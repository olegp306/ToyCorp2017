//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Modules
{
    public class UniSenderMessage
    {
        public string Message_id { get; set; }
    }

    public class UniSenderResponse
    {
        public List<KeyValuePair<string, string>> Warnings { get; set; }

        public string Error { get; set; }

        public string Code { get; set; }
    }

    public class UniSenderGetListsResponse : UniSenderResponse
    {
        public List<UniSenderList> Result { get; set; }
    }

    public class UniSenderSubscribeResponse : UniSenderResponse
    {
        public UniSenderListMember Result { get; set; }
    }

    public class UniSenderUnsubscribeResponse : UniSenderResponse
    {
        public string Result { get; set; }
    }

    public class UniSenderCreateCampaignResponse : UniSenderResponse
    {
        public UniSenderCampaign Result { get; set; }
    }

    public class UniSenderCreateMessageResponse : UniSenderResponse
    {
        public UniSenderMessage Result { get; set; }
    }
}