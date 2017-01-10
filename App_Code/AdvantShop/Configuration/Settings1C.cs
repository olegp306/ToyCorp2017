//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Configuration
{
    public class Settings1C
    {
        public static bool Enabled
        {
            get { return Convert.ToBoolean(SettingProvider.Items["1c_Enabled"]); }
            set { SettingProvider.Items["1c_Enabled"] = value.ToString(); }
        }

        public static bool OnlyUseIn1COrders
        {
            get { return Convert.ToBoolean(SettingProvider.Items["1c_OnlyUseIn1COrders"]); }
            set { SettingProvider.Items["1c_OnlyUseIn1COrders"] = value.ToString(); }
        }


        //public static string NotAssignedStatus
        //{
        //    get { return SettingProvider.Items["1c_NotAssignedStatus"]; }
        //    set { SettingProvider.Items["1c_NotAssignedStatus"] = value; }
        //}

        //public static string AssignedStatus
        //{
        //    get { return SettingProvider.Items["1c_AssignedStatus"]; }
        //    set { SettingProvider.Items["1c_AssignedStatus"] = value; }
        //}

        //public static string RezervStatus
        //{
        //    get { return SettingProvider.Items["1c_RezervStatus"]; }
        //    set { SettingProvider.Items["1c_RezervStatus"] = value; }
        //}

        //public static string ToShipStatus
        //{
        //    get { return SettingProvider.Items["1c_ToShipStatus"]; }
        //    set { SettingProvider.Items["1c_ToShipStatus"] = value; }
        //}

        //public static string ClosedStatus
        //{
        //    get { return SettingProvider.Items["1c_ClosedStatus"]; }
        //    set { SettingProvider.Items["1c_ClosedStatus"] = value; }
        //}

        //public static int GetStatusId(string status)
        //{
        //    var statusId = 0;

        //    switch (status.ToLower())
        //    {
        //        case "не согласован":
        //            statusId = NotAssignedStatus.TryParseInt();
        //            break;
        //        case "согласован":
        //            statusId = AssignedStatus.TryParseInt();
        //            break;
        //        case "к обеспечению":
        //            statusId = RezervStatus.TryParseInt();
        //            break;
        //        case "к отгрузке":
        //            statusId = ToShipStatus.TryParseInt();
        //            break;
        //        case "закрыт":
        //            statusId = ClosedStatus.TryParseInt();
        //            break;
        //    }

        //    return statusId;
        //}
        
        //public static string GetStatusName(int orderStatusId)
        //{
        //    var statusId = orderStatusId.ToString();

        //    if (statusId == NotAssignedStatus)
        //        return "Не согласован";

        //    if (statusId == AssignedStatus)
        //        return "Согласован";

        //    if (statusId == RezervStatus)
        //        return "К обеспечению";

        //    if (statusId == ToShipStatus)
        //        return "К отгрузке";

        //    if (statusId == ClosedStatus)
        //        return "Закрыт";
            
        //    return string.Empty;
        //}
    }
}