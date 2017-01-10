//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Admin.UserControls.ShippingMethods;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Shipping;
using AdvantShop.Configuration;
using AdvantShop.Trial;

namespace Admin
{
    public partial class EditShippingMethod : AdvantShopAdminPage
    {
        private int _shippingMethodID;
        protected int ShippingMethodID
        {
            get
            {
                if (_shippingMethodID != 0)
                    return _shippingMethodID;
                var intval = 0;
                int.TryParse(Request["shippingmethodid"], out intval);
                return intval;
            }
            set
            {
                _shippingMethodID = value;
            }
        }

        protected void Msg(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
        }

        protected void ClearMsg()
        {
            lblMessage.Visible = false;
        }

        protected static readonly Dictionary<ShippingType, string> UcIds = new Dictionary<ShippingType, string>
        {
            {ShippingType.FedEx, "ucFeedEx"},
            {ShippingType.Usps, "ucUsps"},
            {ShippingType.Ups, "ucUPS"},
            {ShippingType.FixedRate, "ucFixedRate"},
            {ShippingType.FreeShipping, "ucFreeShipping"},
            {ShippingType.ShippingByWeight, "ucByWeight"},
            {ShippingType.eDost, "ucEdost"},
            {ShippingType.ShippingByShippingCost, "ucByShippingCost"},
            {ShippingType.ShippingByOrderPrice, "ucByOrderPrice"},
            {ShippingType.ShippingByRangeWeightAndDistance, "ucShippingByRangeWeightAndDistance"},
            {ShippingType.ShippingNovaPoshta, "ucShippingNovaPoshta"},
            {ShippingType.SelfDelivery, "ucSelfDelivery"},
            {ShippingType.Cdek, "ucCdek"},            
            {ShippingType.Multiship, "ucMultiship"},
            {ShippingType.ShippingByProductAmount, "ucByProductAmount"},
            {ShippingType.ShippingByEmsPost, "ucByEmsPost"},
            {ShippingType.CheckoutRu, "ucCheckoutRu"}
        };


        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resources.Resource.Admin_ShippingMethod_Header));

            ClearMsg();
            if (!IsPostBack)
                LoadMethods();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ddlType.DataSource = AdvantShop.Core.AdvantshopConfigService.GetDropdownShippings();
            ddlType.DataBind();
        }

        protected void LoadMethods()
        {
            var methods = ShippingMethodService.GetAllShippingMethods().Where(item => item.ShippingMethodId != 1).ToList();
            if (methods.Count > 0)
            {
                if (ShippingMethodID == 0)
                    ShippingMethodID = methods.First().ShippingMethodId;
                rptTabs.DataSource = methods;
                rptTabs.DataBind();
            }
            else
                pnEmpty.Visible = true;

            ShowMethod(ShippingMethodID);
        }

        protected void ShowMethod(int methodId)
        {
            var method = ShippingMethodService.GetShippingMethod(methodId);
            foreach (var ucId in UcIds)
            {
                var uc = (MasterControl)pnMethods.FindControl(ucId.Value);
                if (method == null)
                {
                    uc.Visible = false;
                    continue;
                }
                if (ucId.Key == method.Type)
                    uc.Method = method;
                uc.Visible = ucId.Key == method.Type;
            }
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            var type = (ShippingType)int.Parse(ddlType.SelectedValue);
            var method = new ShippingMethod
            {
                Type = type,
                Name = txtName.Text,
                Description = txtDescription.Text,
                Enabled = type == ShippingType.FreeShipping,
                DisplayCustomFields = true,
                SortOrder = txtSortOrder.Text.TryParseInt(),
                ZeroPriceMessage = Resources.Resource.Admin_ShippingMethod_ZeroPriceMessage
            };

            TrialService.TrackEvent(TrialEvents.AddShippingMethod, method.Type.ToString());
            var id = ShippingMethodService.InsertShippingMethod(method);
            if (id != 0)
                Response.Redirect("~/Admin/ShippingMethod.aspx?ShippingMethodID=" + id);
        }

        protected void ShippingMethod_Saved(object sender, MasterControl.SavedEventArgs args)
        {
            LoadMethods();
            Msg(string.Format(Resources.Resource.Admin_ShippingMethod_Saved, args.Name));
        }
    }
}