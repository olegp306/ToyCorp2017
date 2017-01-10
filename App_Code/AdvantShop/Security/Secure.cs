//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Permission;

namespace AdvantShop.Security
{
    public class Secure
    {
        public static void AddUserLog(string login, bool isSuccess, bool isAdmin)
        {
            SQLDataAccess.ExecuteNonQuery("[Settings].[sp_AddAuthorizeLog]", CommandType.StoredProcedure,
                                        new SqlParameter("@Login", login),
                                        new SqlParameter("@isAdmin", isAdmin),
                                        new SqlParameter("@UserIP", HttpContext.Current.Request.UserHostAddress),
                                        new SqlParameter("@isSuccess", isSuccess));
        }

        public static bool IsDebugAccount(string strLogin, string strPassword)//, bool isPassHash)
        {
            if (string.IsNullOrEmpty(strLogin) || string.IsNullOrEmpty(strPassword))
                return false;

            try
            {
                return PermissionAccsess.CheckPermission(strLogin, strPassword);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        public static void VerifySessionForErrors()
        {
            if (HttpContext.Current.Session["errOnAsax"] != null && (bool)HttpContext.Current.Session["errOnAsax"])
            {
                HttpContext.Current.Response.Redirect("~/info/SessionError.aspx");
            }
        }


        public static bool VerifyAccessLevel()
        {
            if (Trial.TrialService.IsTrialEnabled)
            {
                return true;
            }

            bool allow;

            switch (CustomerContext.CurrentCustomer.CustomerRole)
            {
                case Role.Administrator:
                    allow = true;
                    break;
                case Role.Moderator:
                    allow = RoleAccess.Check(CustomerContext.CurrentCustomer, HttpContext.Current.Request.Url.ToString().ToLower());
                    break;
                default:
                    allow = false;
                    break;
            }
            if (!allow)
                HttpContext.Current.Response.Redirect("~/AccessDenied.aspx");
            return allow;
        }

        public static void DeleteExpiredAuthorizeLog()
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[AuthorizeLog] where GetDate() > DATEADD(year, 1, AddDate);", CommandType.Text);
        }
    }
}