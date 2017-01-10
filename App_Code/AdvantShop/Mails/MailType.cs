//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Mails
{
    public enum MailType
    {
        None = 0,
        OnRegistration = 1,
        OnPwdRepair = 2,
        OnNewOrder = 3,
        OnChangeOrderStatus = 4,
        OnSendMessage = 5,
        OnSubscribeActivate = 6,
        OnSubscribeDeactivate = 7,
        OnFeedback = 8,
        OnSendFriend = 9,
        OnQuestionAboutProduct = 10,
        OnProductDiscuss = 11,
        OnOrderByRequest = 12,
        OnSendLinkByRequest = 13,
        OnSendFailureByRequest = 14,
        OnSendGiftCertificate = 15,
        OnBuyInOneClick = 16,
        OnBillingLink = 17
    }
}