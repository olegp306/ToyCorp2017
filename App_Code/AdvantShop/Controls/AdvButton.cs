//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;

namespace AdvantShop.Controls
{
    public enum AdvButtonType
    {
        Gray,
        Orange
    }

    public class AdvButton : System.Web.UI.WebControls.Button
    {
        public AdvButton()
        {
            Type = AdvButtonType.Gray;
        }

        #region  Properties

        public string CssInputMozz { get; set; }

        public string CssCenterDiv { get; set; }

        public string CssMain { get; set; }

        public string CssLeftDiv { get; set; }

        public string CssRightDiv { get; set; }

        public string CssInput { get; set; }

        public AdvButtonType _type;

        [TypeConverterAttribute(typeof(AdvButtonType))]
        public AdvButtonType Type
        {
            set
            {
                _type = value;

                switch (_type)
                {
                    case AdvButtonType.Gray:
                        CssMain = "adv_RoundButton_Main";
                        CssLeftDiv = "adv_RoundButton_LeftDiv";
                        CssRightDiv = "adv_RoundButton_RightDiv";
                        CssInput = "adv_RoundButton_Input";
                        CssCenterDiv = "adv_RoundButton_CenterDiv";
                        CssInputMozz = "adv_RoundButton_Input-mozz";
                        break;

                    case AdvButtonType.Orange:
                        CssMain = "adv_OrangeButton_Main";
                        CssInput = "adv_OrangeButton_Input";
                        CssInputMozz = "adv_OrangeButton_Input-mozz";
                        CssLeftDiv = "adv_OrangeButton_LeftDiv";
                        CssRightDiv = "adv_OrangeButton_RightDiv";
                        CssCenterDiv = "adv_OrangeButton_CenterDiv";
                        break;
                }
            }
            get
            {
                return _type;
            }
        }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssMain);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            // -------
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssLeftDiv);
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                string.Format("document.getElementById(\'{0}\').click()", ClientID));
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();
            // -------
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssCenterDiv);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class,
                                HttpContext.Current.Request.Browser.Browser.ToLower() == "firefox"
                                    ? CssInputMozz
                                    : CssInput);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssInput);
            base.Render(writer);

            writer.RenderEndTag();
            // -------
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssRightDiv);
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                string.Format("document.getElementById(\'{0}\').click()", ClientID));
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();
            // -------
            writer.RenderEndTag();
        }
    }
}