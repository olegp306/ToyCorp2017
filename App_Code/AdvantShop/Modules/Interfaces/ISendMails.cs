//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Customers;

namespace AdvantShop.Modules.Interfaces
{
    /// <summary>
    /// Mail recipient type
    /// </summary>
    [Flags]
    public enum MailRecipientType
    {
        // важно: значения - степени двойки
        None = 0,
        Subscriber = 1,
        OrderCustomer = 2
    }

    public interface ISendMails : IModule
    {
        bool SendMails(string title, string message, MailRecipientType recipientType);

        void SubscribeEmail(ISubscriber subscriber);

        void UnsubscribeEmail(string email);
    }
}