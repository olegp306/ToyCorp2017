//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using UPSRateServiceWebReference;

namespace AdvantShop.Shipping
{
    /// UPSP packaging type
    public enum UpsPackagingType
    {
        CustomerSuppliedPackage,
        Letter,
        Tube,
        PAK,
        ExpressBox,
        _10KgBox,
        _25KgBox
    }
    public enum UpsPickupType
    {
        DailyPickup,
        CustomerCounter,
        OneTimePickup,
        OnCallAir,
        SuggestedRetailRates,
        LetterCenter,
        AirServiceCenter
    }
    public enum UpsCustomerClassification
    {
        Retail,
        Wholesale,
        Occasional
    }

    public class Ups : IShippingMethod
    {
        private const float MaxPackageWeight = 68.03F;

        public string AccessKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public UpsCustomerClassification CustomerType { get; set; }

        public UpsPickupType PickupType { get; set; }
        public UpsPackagingType PackagingType { get; set; }
        public float Extracharge { get; set; }
        public float Rate { get; set; }

        public string CountryCodeFrom { get; set; }
        public string PostalCodeFrom { get; set; }

        public string CountryCodeTo { get; set; }
        public string PostalCodeTo { get; set; }
        public string StateTo { get; set; }
        public string CityTo { get; set; }
        public string AddressTo { get; set; }

        public string EnabledService { get; private set; }

        public ShoppingCart ShoppingCart { get; set; }

        public Ups(Dictionary<string, string> parameters)
        {
            try
            {
                AccessKey = parameters.ElementOrDefault(UpsTemplate.AccessKey);
                UserName = parameters.ElementOrDefault(UpsTemplate.UserName);
                Password = parameters.ElementOrDefault(UpsTemplate.Password);
                PostalCodeFrom = parameters.ElementOrDefault(UpsTemplate.PostalCode);
                CountryCodeFrom = parameters.ElementOrDefault(UpsTemplate.CountryCode);
                CustomerType = (UpsCustomerClassification)parameters.ElementOrDefault(UpsTemplate.CustomerClassification).TryParseInt();
                PickupType = (UpsPickupType)parameters.ElementOrDefault(UpsTemplate.PickupType).TryParseInt();
                PackagingType = (UpsPackagingType)parameters.ElementOrDefault(UpsTemplate.PackagingType).TryParseInt();
                Extracharge = parameters.ElementOrDefault(UpsTemplate.Extracharge).TryParseFloat();
                Rate = parameters.ElementOrDefault(UpsTemplate.Rate).TryParseFloat();

                EnabledService = GetEnabledService(parameters);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// Don't use this for Ups
        /// </summary>
        /// <returns></returns>
        public float GetRate()
        {
            throw new Exception("GetRate method isnot avalible for Ups");
        }

        public List<ShippingOption> GetShippingOptions()
        {
            var shippingOptions = new List<ShippingOption>();

            try
            {
                var service = new RateService();
                RateRequest request = CreateRateRequest(service);
                RateResponse rateResponse = service.ProcessRate(request);
                shippingOptions = ParseAnswer(rateResponse);
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                Debug.LogError(ex, false);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                Debug.LogError(ex, false);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return shippingOptions;
        }

        private static string GetEnabledService(Dictionary<string, string> items)
        {
            var res = new StringBuilder();
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsNextDayAir))
            {
                res.Append(UpsTemplate.UpsNextDayAir + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.Ups2NdDayAir))
            {
                res.Append(UpsTemplate.Ups2NdDayAir + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsGround))
            {
                res.Append(UpsTemplate.UpsGround + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsWorldwideExpress))
            {
                res.Append(UpsTemplate.UpsWorldwideExpress + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsWorldwideExpedited))
            {
                res.Append(UpsTemplate.UpsWorldwideExpedited + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsStandard))
            {
                res.Append(UpsTemplate.UpsStandard + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.Ups3DaySelect))
            {
                res.Append(UpsTemplate.Ups3DaySelect + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsNextDayAirSaver))
            {
                res.Append(UpsTemplate.UpsNextDayAirSaver + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsNextDayAirEarlyAm))
            {
                res.Append(UpsTemplate.UpsNextDayAirEarlyAm + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsWorldwideExpressPlus))
            {
                res.Append(UpsTemplate.UpsWorldwideExpressPlus + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.Ups2NdDayAirAm))
            {
                res.Append(UpsTemplate.Ups2NdDayAirAm + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsSaver))
            {
                res.Append(UpsTemplate.UpsSaver + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsTodayStandard))
            {
                res.Append(UpsTemplate.UpsTodayStandard + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsTodayDedicatedCourrier))
            {
                res.Append(UpsTemplate.UpsTodayDedicatedCourrier + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsTodayExpress))
            {
                res.Append(UpsTemplate.UpsTodayExpress + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, UpsTemplate.UpsTodayExpressSaver))
            {
                res.Append(UpsTemplate.UpsTodayExpressSaver + ";");
            }

            return res.ToString();
        }


        public static string GetServiceId(string service)
        {
            string serviceId = string.Empty;
            switch (service)
            {
                case "UPS Next Day Air":
                    serviceId = "01";
                    break;
                case "UPS 2nd Day Air":
                    serviceId = "02";
                    break;
                case "UPS Ground":
                    serviceId = "03";
                    break;
                case "UPS Worldwide Express":
                    serviceId = "07";
                    break;
                case "UPS Worldwide Expedited":
                    serviceId = "08";
                    break;
                case "UPS Standard":
                    serviceId = "11";
                    break;
                case "UPS 3 Day Select":
                    serviceId = "12";
                    break;
                case "UPS Next Day Air Saver":
                    serviceId = "13";
                    break;
                case "UPS Next Day Air Early A.M.":
                    serviceId = "14";
                    break;
                case "UPS Worldwide Express Plus":
                    serviceId = "54";
                    break;
                case "UPS 2nd Day Air A.M.":
                    serviceId = "59";
                    break;
                case "UPS Saver":
                    serviceId = "65";
                    break;
                case "UPS Today Standard": //82-86, for Polish Domestic Shipments
                    serviceId = "82";
                    break;
                case "UPS Today Dedicated Courier":
                    serviceId = "83";
                    break;
                case "UPS Today Express":
                    serviceId = "85";
                    break;
                case "UPS Today Express Saver":
                    serviceId = "86";
                    break;
                default:
                    break;
            }
            return serviceId;
        }

        #region PrivateMethods
        private RateRequest CreateRateRequest(RateService rate)
        {
            RateRequest rateRequest = new RateRequest();
            UPSSecurity upss = new UPSSecurity();
            UPSSecurityServiceAccessToken upssSvcAccessToken = new UPSSecurityServiceAccessToken();
            upssSvcAccessToken.AccessLicenseNumber = AccessKey;
            upss.ServiceAccessToken = upssSvcAccessToken;

            UPSSecurityUsernameToken upssUsrNameToken = new UPSSecurityUsernameToken();
            upssUsrNameToken.Username = UserName;
            upssUsrNameToken.Password = Password;
            upss.UsernameToken = upssUsrNameToken;
            rate.UPSSecurityValue = upss;

            RequestType request = new RequestType();
            String[] requestOption = { "Shop" };
            request.RequestOption = requestOption;
            rateRequest.Request = request;

            ShipmentType shipment = new ShipmentType();

            ShipperType shipper = new ShipperType();
            //shipper.ShipperNumber = "ISUS01";
            AddressType shipperAddress = new AddressType();
            String[] addressLine = { "Shipper\'s address line" };
            shipperAddress.AddressLine = addressLine;
            shipperAddress.City = "Shipper\'s city";
            shipperAddress.PostalCode = PostalCodeFrom;
            //shipperAddress.StateProvinceCode = UpsItem.CountryCode;
            shipperAddress.CountryCode = CountryCodeFrom;
            shipperAddress.AddressLine = addressLine;
            shipper.Address = shipperAddress;
            shipment.Shipper = shipper;

            ShipFromType shipFrom = new ShipFromType();
            AddressType shipFromAddress = new AddressType();
            shipFromAddress.AddressLine = addressLine;
            shipFromAddress.City = "ShipFrom city";
            shipFromAddress.PostalCode = PostalCodeFrom;
            //shipFromAddress.StateProvinceCode = "GA";
            shipFromAddress.CountryCode = CountryCodeFrom;
            shipFrom.Address = shipFromAddress;
            shipment.ShipFrom = shipFrom;

            ShipToType shipTo = new ShipToType();
            ShipToAddressType shipToAddress = new ShipToAddressType();
            String[] addressLine1 = { AddressTo };
            shipToAddress.AddressLine = addressLine1;
            shipToAddress.City = CityTo;
            shipToAddress.PostalCode = PostalCodeTo;
            shipToAddress.StateProvinceCode = StateTo;
            shipToAddress.CountryCode = CountryCodeTo;
            shipTo.Address = shipToAddress;
            shipment.ShipTo = shipTo;

            //CodeDescriptionType service = new CodeDescriptionType();
            //service.Code = "02";
            //shipment.Service = service;
            float weight = MeasureUnits.ConvertWeight(ShoppingCart.TotalShippingWeight, MeasureUnits.WeightUnit.Kilogramm, MeasureUnits.WeightUnit.Pound);

            var data = new List<PackageType>();
            if (!IsPackageTooHeavy(weight))
            {

                PackageType package = new PackageType();
                PackageWeightType packageWeight = new PackageWeightType();
                packageWeight.Weight = weight.ToString("F3").Replace(',', '.');

                CodeDescriptionType uom = new CodeDescriptionType();
                uom.Code = "LBS";
                uom.Description = "Pounds";
                packageWeight.UnitOfMeasurement = uom;
                package.PackageWeight = packageWeight;

                CodeDescriptionType packType = new CodeDescriptionType();
                packType.Code = "02";
                package.PackagingType = packType;
                data.Add(package);
            }
            else
            {
                int totalPackages = 1;
                int totalPackagesWeights = 1;
                if (IsPackageTooHeavy(weight))
                {
                    totalPackagesWeights = SQLDataHelper.GetInt(Math.Ceiling(weight / MaxPackageWeight));
                }

                totalPackages = totalPackagesWeights;
                if (totalPackages == 0)
                    totalPackages = 1;

                float weight2 = weight / totalPackages;

                if (weight2 < 1)
                    weight2 = 1;
                for (int i = 0; i < totalPackages; i++)
                {
                    PackageType package = new PackageType();
                    PackageWeightType packageWeight = new PackageWeightType();
                    packageWeight.Weight = weight2.ToString("F3");

                    CodeDescriptionType uom = new CodeDescriptionType();
                    uom.Code = "LBS";
                    uom.Description = "Pounds";
                    packageWeight.UnitOfMeasurement = uom;
                    package.PackageWeight = packageWeight;

                    CodeDescriptionType packType = new CodeDescriptionType();
                    packType.Code = GetPackagingTypeCode(PackagingType);
                    package.PackagingType = packType;
                    data.Add(package);
                }
            }

            PackageType[] pkgArray = data.ToArray();
            shipment.Package = pkgArray;
            rateRequest.Shipment = shipment;

            CodeDescriptionType pckup = new CodeDescriptionType() { Code = GetPickupTypeCode(PickupType) };
            rateRequest.PickupType = pckup;

            CodeDescriptionType ccustomer = new CodeDescriptionType() { Code = GetCustomerClassificationCode(CustomerType) };
            rateRequest.CustomerClassification = ccustomer;

            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();

            return rateRequest;
        }

        private static bool IsPackageTooHeavy(float weight)
        {
            if (weight > MaxPackageWeight)
                return true;
            return false;
        }

        private static string GetCustomerClassificationCode(UpsCustomerClassification customerClassification)
        {
            switch (customerClassification)
            {
                case UpsCustomerClassification.Wholesale:
                    return "01";
                case UpsCustomerClassification.Occasional:
                    return "03";
                case UpsCustomerClassification.Retail:
                    return "04";
                default:
                    return string.Empty;
            }
        }
        private static string GetPackagingTypeCode(UpsPackagingType packagingType)
        {
            switch (packagingType)
            {
                case UpsPackagingType.Letter:
                    return "01";
                case UpsPackagingType.CustomerSuppliedPackage:
                    return "02";
                case UpsPackagingType.Tube:
                    return "03";
                case UpsPackagingType.PAK:
                    return "04";
                case UpsPackagingType.ExpressBox:
                    return "21";
                case UpsPackagingType._10KgBox:
                    return "25";
                case UpsPackagingType._25KgBox:
                    return "24";
                default:
                    return string.Empty;
            }
        }
        private static string GetPickupTypeCode(UpsPickupType pickupType)
        {
            switch (pickupType)
            {
                case UpsPickupType.DailyPickup:
                    return "01";
                case UpsPickupType.CustomerCounter:
                    return "03";
                case UpsPickupType.OneTimePickup:
                    return "06";
                case UpsPickupType.OnCallAir:
                    return "07";
                case UpsPickupType.SuggestedRetailRates:
                    return "11";
                case UpsPickupType.LetterCenter:
                    return "19";
                case UpsPickupType.AirServiceCenter:
                    return "20";
                default:
                    return string.Empty;
            }
        }
        private static string GetServiceName(string serviceID)
        {
            switch (serviceID)
            {
                case "01":
                    return "UPS Next Day Air";
                case "02":
                    return "UPS 2nd Day Air";
                case "03":
                    return "UPS Ground";
                case "07":
                    return "UPS Worldwide Express";
                case "08":
                    return "UPS Worldwide Expedited";
                case "11":
                    return "UPS Standard";
                case "12":
                    return "UPS 3 Day Select";
                case "13":
                    return "UPS Next Day Air Saver";
                case "14":
                    return "UPS Next Day Air Early A.M.";
                case "54":
                    return "UPS Worldwide Express Plus";
                case "59":
                    return "UPS 2nd Day Air A.M.";
                case "65":
                    return "UPS Saver";
                case "82": //82-86, for Polish Domestic Shipments
                    return "UPS Today Standard";
                case "83":
                    return "UPS Today Dedicated Courier";
                case "85":
                    return "UPS Today Express";
                case "86":
                    return "UPS Today Express Saver";
                default:
                    return string.Empty;
            }
        }
        private List<ShippingOption> ParseAnswer(RateResponse rateResponse)
        {
            var res = new List<ShippingOption>();
            var enabledServices = EnabledService;
            foreach (var rateDetail in rateResponse.RatedShipment)
            {
                var shippingOption = new ShippingOption();
                if (!String.IsNullOrEmpty(enabledServices) && !enabledServices.Contains(GetServiceName(rateDetail.Service.Code)))
                {
                    continue;
                }

                string serviceName = GetServiceName(rateDetail.Service.Code);
                shippingOption.Name = serviceName;

                var shippingRate = (Rate > 0) ? rateDetail.TotalCharges.MonetaryValue.TryParseFloat() * Rate + Extracharge
                                              : rateDetail.TotalCharges.MonetaryValue.TryParseFloat() + Extracharge;
                shippingOption.Rate = shippingRate;
                res.Add(shippingOption);
            }
            return res;
        }
        #endregion
    }
}