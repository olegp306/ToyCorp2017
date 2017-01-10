//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Web.UI;
using System.Web.UI.WebControls;

namespace AdvantShop.Controls
{
    public class OrangeRoundedButton :System.Web.UI.WebControls. Button
    {
        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:inline-block");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            writer.RenderBeginTag(HtmlTextWriterTag.Tbody);

            writer.AddAttribute(HtmlTextWriterAttribute.Style, "height:27px");

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            writer.AddAttribute(HtmlTextWriterAttribute.Style,
                                "background-image:url(\'images/or_buttonbgleft.gif\');width:5px;border:none;cursor: pointer;");
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                string.Format("document.getElementById(\'{0}\').click()", ClientID));
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Style,
                                "background-image:url(images/or_buttonbg.gif);border:none;");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            writer.AddAttribute(HtmlTextWriterAttribute.Style,
                                "background-image:url(images/or_buttonbg.gif);border-width:0px;height:27px;color:white;cursor: pointer;");
            base.Render(writer);

            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Style,
                                "background-image:url(\'images/or_buttonbgright.gif\');width:5px;border:none;cursor: pointer;");
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                string.Format("document.getElementById(\'{0}\').click()", ClientID));
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.RenderEndTag();

            writer.RenderEndTag();

            writer.RenderEndTag();

            writer.RenderEndTag();
        }
    }
}