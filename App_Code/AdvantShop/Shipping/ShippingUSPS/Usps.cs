//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository;

namespace AdvantShop.Shipping
{
    public class Usps : IShippingMethod
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public float Rate { get; set; }
        public float Extracharge { get; set; }

        public string PostalCodeFrom { get; set; }
        public string CountryTo { get; set; }
        public string CountryToIso2 { get; set; }
        public string PostalCodeTo { get; set; }

        public PackageType Container { get; set; }
        public PackageSize Size { get; set; }
        public ServiceType Service { get; set; }

        private const string UrlDomenic = "http://production.shippingapis.com/ShippingAPI.dll?API=RateV4&XML=";
        private const string UrlInternational = "http://production.shippingapis.com/ShippingAPI.dll?API=IntlRateV2&XML=";

        private float _weight;
        private bool _isDomenic;

        public List<string> EnabledService { get; private set; }

        public ShoppingCart ShoppingCart { get; set; }
        public float TotalPrice { get; set; }

        public Usps(Dictionary<string, string> parameters)
        {
            try
            {
                Rate = parameters.ElementOrDefault(UspsTemplate.Rate).TryParseFloat();
                Extracharge = parameters.ElementOrDefault(UspsTemplate.Extracharge).TryParseFloat();

                UserId = parameters.ElementOrDefault(UspsTemplate.UserId);
                Password = parameters.ElementOrDefault(UspsTemplate.Password);
                PostalCodeFrom = parameters.ElementOrDefault(UspsTemplate.PostalCode);

                EnabledService = new List<string>();
                EnabledService.AddRange(GetEnabledDomesticService(parameters));
                EnabledService.AddRange(GetEnabledInternationalService(parameters));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// Don't use this for Usps
        /// </summary>
        /// <returns></returns>
        public float GetRate()
        {
            throw new Exception("GetRate method isnot avalible for Usps");
        }

        public List<ShippingOption> GetShippingOptions()
        {
            try
            {
                _weight = MeasureUnits.ConvertWeight(ShoppingCart.TotalShippingWeight, MeasureUnits.WeightUnit.Kilogramm, MeasureUnits.WeightUnit.Pound);

                _isDomenic = CountryToIso2.ToUpper().Trim() == "US";
                return GetShippingOption();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return new List<ShippingOption>();
            }
        }

        public enum PackageType { None, Flat_Rate_Envelope, Flat_Rate_Box };
        public enum PackageSize { None, Regular, Large, Oversize };
        public enum LabelImageType { TIF, PDF, None };
        public enum ServiceType { Priority, First_Class, Parcel_Post, Bound_Printed_Matter, Media_Mail, Library_Mail };
        public enum LabelType { FullLabel = 1, DeliveryConfirmationBarcode = 2 };

        #region PrivateMethods
        private static IEnumerable<string> GetEnabledDomesticService(Dictionary<string, string> items)
        {
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.FirstClass))
            {
                yield return UspsTemplate.FirstClass;

            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.ExpressMailSundayHolidayGuarantee))
            {
                yield return UspsTemplate.ExpressMailSundayHolidayGuarantee;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.ExpressMailFlatRateEnvelopeSundayHolidayGuarantee))
            {
                yield return UspsTemplate.ExpressMailFlatRateEnvelopeSundayHolidayGuarantee;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.ExpressMailHoldForPickup))
            {
                yield return UspsTemplate.ExpressMailHoldForPickup;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.ExpressMailFlatRateEnvelopeHoldForPickup))
            {
                yield return UspsTemplate.ExpressMailFlatRateEnvelopeHoldForPickup;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.ExpresMail))
            {
                yield return UspsTemplate.ExpresMail;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.ExpressMailFlatRateEnvelope))
            {
                yield return UspsTemplate.ExpressMailFlatRateEnvelope;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.PriorityMail))
            {
                yield return UspsTemplate.PriorityMail;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.PriorityMailFlatRateEnvelope))
            {
                yield return UspsTemplate.PriorityMailFlatRateEnvelope;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.PriorityMailSmallFlatRateBox))
            {
                yield return UspsTemplate.PriorityMailSmallFlatRateBox;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.PriorityMailMediumFlatRateBox))
            {
                yield return UspsTemplate.PriorityMailMediumFlatRateBox;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.PriorityMailLargeFlatRateBox))
            {
                yield return UspsTemplate.PriorityMailLargeFlatRateBox;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.ParcelPost))
            {
                yield return UspsTemplate.ParcelPost;
            }
            
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.BoundPrintedMatter))
            {
                yield return UspsTemplate.BoundPrintedMatter;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.MediaMail))
            {
                yield return UspsTemplate.MediaMail;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.LibraryMail))
            {
                yield return UspsTemplate.LibraryMail;
            }
        }

        private static IEnumerable<string> GetEnabledInternationalService(Dictionary<string, string> items)
        {
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.GlobalExpressGuaranteed))
            {
                yield return UspsTemplate.GlobalExpressGuaranteed;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.GlobalExpressGuaranteedNonDocumentRectangular))
            {
                yield return UspsTemplate.GlobalExpressGuaranteedNonDocumentRectangular;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.GlobalExpressGuaranteedNonDocumentNonRectangular))
            {
                yield return UspsTemplate.GlobalExpressGuaranteedNonDocumentNonRectangular;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.UspsGxgEnvelopes))
            {
                yield return UspsTemplate.UspsGxgEnvelopes;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.ExpressMailInternationalFlatRateEnvelope))
            {
                yield return UspsTemplate.ExpressMailInternationalFlatRateEnvelope;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.PriorityMailInternational))
            {
                yield return UspsTemplate.PriorityMailInternational;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.PriorityMailInternationalLargeFlatRateBox))
            {
                yield return UspsTemplate.PriorityMailInternationalLargeFlatRateBox;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.PriorityMailInternationalMediumFlatRateBox))
            {
                yield return UspsTemplate.PriorityMailInternationalMediumFlatRateBox;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.PriorityMailInternationalSmallFlatRateBox))
            {
                yield return UspsTemplate.PriorityMailInternationalSmallFlatRateBox;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.FirstClassMailInternationalLargeEnvelope))
            {
                yield return UspsTemplate.FirstClassMailInternationalLargeEnvelope;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.ExpressMailInternational))
            {
                yield return UspsTemplate.ExpressMailInternational;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.PriorityMailInternationalFlatRateEnvelope))
            {
                yield return UspsTemplate.PriorityMailInternationalFlatRateEnvelope;
            }
            if (ShippingMethod.GetBooleanParam(items, UspsTemplate.FirstClassMailInternationalPackage))
            {
                yield return UspsTemplate.FirstClassMailInternationalPackage;
            }
        }

        private List<ShippingOption> GetShippingOption()
        {
            string request = _isDomenic
                ? UrlDomenic + GetXmlDomenicPackage()
                : UrlInternational + GetXmlInternationalPackage();

            string xml = new WebClient().DownloadString(request);
            if (xml.Contains("<Error>"))
            {
                int idx1 = xml.IndexOf("<Description>") + 13;
                int idx2 = xml.IndexOf("</Description>");
                string errorText = xml.Substring(idx1, idx2 - idx1);

                Debug.LogError(new Exception("USPS Error returned: " + errorText), false);
            }

            return ParseResponseMessage(xml);
        }

        private string GetXmlDomenicPackage()
        {
            var lb = (int)_weight;
            int oz = ((int)(_weight * 16)) % 16;

            var sb = new StringBuilder();
            sb.AppendFormat("<RateV4Request USERID=\"{0}\" PASSWORD=\"{1}\">", UserId, Password);
            sb.Append("<Package ID=\"0\">");

            sb.AppendFormat("<Service>{0}</Service>", "ALL");
            sb.AppendFormat("<ZipOrigination>{0}</ZipOrigination>", PostalCodeFrom);
            sb.AppendFormat("<ZipDestination>{0}</ZipDestination>", PostalCodeTo);
            sb.AppendFormat("<Pounds>{0}</Pounds>", lb);
            sb.AppendFormat("<Ounces>{0}</Ounces>", oz);
            sb.AppendFormat("<Container />");
            sb.AppendFormat("<Size>{0}</Size>", Size);
            sb.Append("<Machinable>FALSE</Machinable>");

            sb.Append("</Package>");
            sb.Append("</RateV4Request>");

            return sb.ToString();
        }

        private string GetXmlInternationalPackage()
        {
            var lb = (int)_weight;
            int oz = ((int)(_weight * 16)) % 16;

           // TODO: Ширина, длинна, высота и обхват посылки. Измеряется в дюймах и округляется до ближайшего целого.
            int width = 0;
            int length = 0;
            int height = 0;
            int girth = 0;

            var sb = new StringBuilder();
            sb.AppendFormat("<IntlRateV2Request USERID=\"{0}\" PASSWORD=\"{1}\">", UserId, Password);
            sb.Append("<Package ID=\"0\">");

            sb.AppendFormat("<Pounds>{0}</Pounds>", lb);
            sb.AppendFormat("<Ounces>{0}</Ounces>", oz);
            sb.AppendFormat("<Machinable>{0}</Machinable>", "FALSE");
            sb.AppendFormat("<MailType>{0}</MailType>", "ALL");
            sb.AppendFormat("<ValueOfContents>{0}</ValueOfContents>", TotalPrice);
            sb.AppendFormat("<Country>{0}</Country>", CountryTo);
            sb.AppendFormat("<Container>{0}</Container>", "RECTANGULAR");
            sb.AppendFormat("<Size>{0}</Size>", Size);
            sb.AppendFormat("<Width>{0}</Width>", width);
            sb.AppendFormat("<Length>{0}</Length>", length);
            sb.AppendFormat("<Height>{0}</Height>", height);
            sb.AppendFormat("<Girth>{0}</Girth>", girth);
            sb.AppendFormat("<CommercialFlag>{0}</CommercialFlag>", "Y");

            sb.Append("</Package>");
            sb.Append("</IntlRateV2Request>");

            return sb.ToString();
        }

        private List<ShippingOption> ParseResponseMessage(string response)
        {
            string serviceTag = "Service";
            string serviceNameTag = "SvcDescription";
            string rateTag = "Postage";

            if (_isDomenic)
            {
                serviceTag = "Postage";
                serviceNameTag = "MailService";
                rateTag = "Rate";
            }

            var shippingOptions = new List<ShippingOption>();
            using (var sr = new StringReader(response))
            using (var tr = new XmlTextReader(sr))
            {
                do
                {
                    tr.Read();

                    if ((tr.Name == serviceTag) && (tr.NodeType == XmlNodeType.Element))
                    {
                        string serviceCode = string.Empty;
                        string postalRate = string.Empty;

                        do
                        {
                            tr.Read();

                            if ((tr.Name == serviceNameTag) && (tr.NodeType == XmlNodeType.Element))
                            {
                                serviceCode = tr.ReadString().Replace("**", "");
                                int idx1 = serviceCode.IndexOf("&lt;sup&gt;");
                                int idx2 = serviceCode.IndexOf("&lt;/sup&gt;") + 12;

                                if (idx1 >= 0)
                                {
                                    serviceCode = serviceCode.Remove(idx1, idx2 - idx1);
                                }

                                tr.ReadEndElement();
                                if ((tr.Name == serviceNameTag) && (tr.NodeType == XmlNodeType.EndElement))
                                    break;
                            }

                            if ((tr.Name == rateTag) && (tr.NodeType == XmlNodeType.Element))
                            {
                                postalRate = tr.ReadString();
                                tr.ReadEndElement();
                                if ((tr.Name == rateTag) && (tr.NodeType == XmlNodeType.EndElement))
                                    break;
                            }

                        }
                        while (!((tr.Name == serviceTag) && (tr.NodeType == XmlNodeType.EndElement)));

                        if ((EnabledService.Contains(serviceCode)) && (shippingOptions.Find(s => s.Name == serviceCode) == null))
                        {
                            var shippingRate = (Rate > 0) ? postalRate.TryParseFloat() * Rate + Extracharge
                                                          : postalRate.TryParseFloat() + Extracharge;
                            //if (Rate > 0)
                            //{
                            //    shippingRate *= Rate;
                            //}

                            var shippingOption = new ShippingOption
                                                     {
                                                         Rate = shippingRate,
                                                         Name = serviceCode
                                                     };
                            shippingOptions.Add(shippingOption);
                        }
                    }
                }
                while (!tr.EOF);
            }
            return shippingOptions;
        }
        #endregion
    }
}