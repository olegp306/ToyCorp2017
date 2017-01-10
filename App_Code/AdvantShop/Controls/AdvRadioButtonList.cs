//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Web.UI;
using System.Web.UI.WebControls;

namespace AdvantShop.Controls
{
    public class AdvRadioButtonList : RadioButtonList
    {
        protected override void RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo,
                                           HtmlTextWriter writer)
        {
            if (Items[repeatIndex].Value.Equals("%subheader%"))
            {
                writer.Write("<h3 class=\'shipsrvc\'>{0}</h3>", Items[repeatIndex].Text);
            }
            else
            {
                if (Items[repeatIndex].Value.Equals("%msg%"))
                {
                    writer.Write("<h4 class=\'shipsrvcmsg\'>{0}</h4>", Items[repeatIndex].Text);
                }
                else
                {
                    base.RenderItem(itemType, repeatIndex, repeatInfo, writer);
                }
            }
        }
    }
}