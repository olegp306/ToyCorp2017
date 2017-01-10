//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Controls;

namespace AdvantShop.Payment
{
    public enum FormMethod
    {
        POST,
        GET
    }
    /// <summary>
    /// Summary description for PaymentFormHandler
    /// </summary>
    public class PaymentFormHandler
    {
        public PaymentFormHandler()
        {
            Method = FormMethod.POST;
            FormName = "Pay";
        }
        public string Url { get; set; }
        public FormMethod Method { get; set; }
        public string FormName { get; set; }
        public PaymentService.PageWithPaymentButton Page { get; set; }
        private Dictionary<string, string> _inputValues = new Dictionary<string, string>();
        public Dictionary<string, string> InputValues
        {
            get { return _inputValues; }
            set { _inputValues = value; }
        }
        public void Add(string key, string value)
        {
            InputValues.Add(key, value);
        }
        public void AddRange(IEnumerable<KeyValuePair<string, string>> values, bool overwrite)
        {
            foreach (KeyValuePair<string, string> keyValuePair in values)
            {
                if (!InputValues.ContainsKey(keyValuePair.Key))
                    InputValues.Add(keyValuePair.Key, keyValuePair.Value);
                else if (overwrite)
                    InputValues[keyValuePair.Key] = keyValuePair.Value;
            }
        }
        public void AddRange(IEnumerable<KeyValuePair<string, string>> values)
        {
            AddRange(values, false);
        }
        public void AddRangeOverwrite(IEnumerable<KeyValuePair<string, string>> values)
        {
            AddRange(values, true);
        }

        public string ProcessRequest()
        {
            return ProcessRequest(false);
        }

        public string ProcessRequest(bool useWindows1251)
        {
            var context = HttpContext.Current;
            context.Response.Clear();

            string str = string.Empty;

            str += (string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" accept-charset=\"{3}\">", FormName, Method, Url, useWindows1251 ? "windows-1251" : "utf-8"));
            foreach (KeyValuePair<string, string> inputValue in InputValues)
            {
                str +=
                    (string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\" />",
                                   HttpUtility.HtmlEncode(inputValue.Key), HttpUtility.HtmlEncode(inputValue.Value)));
            }
            str += "</form>";

            if (Page == PaymentService.PageWithPaymentButton.myaccount)
            {
                str += Button.RenderHtml(Resources.Resource.Client_MyAccount_PayOrder,
                                         Button.eType.Confirm,
                                         Button.eSize.Middle,
                                         onClientClick: string.Format("document.{0}.submit();", FormName));
            }
            else if (Page == PaymentService.PageWithPaymentButton.orderconfirmation)
            {
                str += Button.RenderHtml(Resources.Resource.Client_MyAccount_PayOrder,
                                                     Button.eType.Submit,
                                                     Button.eSize.Middle,
                                                     onClientClick: string.Format("document.{0}.submit();", FormName));
            }
            return str;
        }

        public void Post()
        {
            Post(false);
        }

        public void Post(bool useWindows1251)
        {
            var context = HttpContext.Current;
            context.Response.Clear();

            //var rgAnsii = new System.Text.RegularExpressions.Regex("[�-��-߹]");

            //if (InputValues.Keys.Any(inputKey => rgAnsii.IsMatch(InputValues[inputKey])))
            //{
            //    context.Response.ContentType = "text/html; windows-1251";
            //    context.Response.Charset = "Windows-1251";
            //    context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("Windows-1251");
            //}
            if (useWindows1251)
            {
                context.Response.ContentType = "text/html; windows-1251";
                context.Response.Charset = "Windows-1251";
                context.Response.ContentEncoding = System.Text.Encoding.GetEncoding("Windows-1251");
            }
            else
            {
                context.Response.ContentType = "text/html; utf-8";
                context.Response.Charset = "Utf-8";
                context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            }
            //default
            //context.Response.ContentType = "text/html; utf-8";
            //context.Response.Charset = "Utf-8";
            //context.Response.ContentEncoding = System.Text.Encoding.UTF8;

            context.Response.Write(string.Format("<html><head><title>Connecting to payment server...</title><script src='js/jq/jquery-1.7.1.min.js'></script></head><body>"));
            //accept-charset=\"utf-8\"
            context.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
            foreach (KeyValuePair<string, string> inputValue in InputValues)
            {
                context.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", HttpUtility.HtmlEncode(inputValue.Key), HttpUtility.HtmlEncode(inputValue.Value)));
            }
            context.Response.Write(string.Format("</form><script>$(document).ready(function(){{document.{0}.submit();}});</script>", FormName));
            context.Response.Write("<center><br /><b>Please wait...</b><br />Connecting to payment server...</center></body></html>");
            context.Response.End();
        }
    }
}