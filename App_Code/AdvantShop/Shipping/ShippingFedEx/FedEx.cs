//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using FedExRateServiceWebReference;
//using RateWebServiceClient.RateServiceWebReference;

namespace AdvantShop.Shipping
{
    public class FedEx : IShippingMethod
    {
        private const float MaxPackageWeight = 68.03F;
        //***
        public string AccountNumber { get; set; }
        public string MeterNumber { get; set; }
        public float Rate { get; set; }
        public float Extracharge { get; set; }
        public string Key { get; set; }
        public string Password { get; set; }

        //**
        public string CountryCodeFrom { get; set; }
        public string PostalCodeFrom { get; set; }
        public string StateFrom { get; set; }
        public string CityFrom { get; set; }
        public string AddressFrom { get; set; }

        //***
        public string CountryCodeTo { get; set; }
        public string PostalCodeTo { get; set; }
        public string StateTo { get; set; }
        public string CityTo { get; set; }
        public string AddressTo { get; set; }

        public float TotalPrice { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

        public string CurrencyIso3 { get; set; }

        //***
        public string EnabledService { get; private set; }

        //Constructor
        public FedEx(Dictionary<string, string> parameters)
        {
            try
            {
                CountryCodeFrom = parameters.ElementOrDefault(FedExTemplate.CountryCode);
                PostalCodeFrom = parameters.ElementOrDefault(FedExTemplate.PostalCode);
                StateFrom = parameters.ElementOrDefault(FedExTemplate.State);
                CityFrom = parameters.ElementOrDefault(FedExTemplate.City);
                AddressFrom = parameters.ElementOrDefault(FedExTemplate.Address);
                AccountNumber = parameters.ElementOrDefault(FedExTemplate.AccountNumber);
                MeterNumber = parameters.ElementOrDefault(FedExTemplate.MeterNumber);
                Rate = parameters.ElementOrDefault(FedExTemplate.Rate).TryParseFloat();
                Extracharge = parameters.ElementOrDefault(FedExTemplate.Extracharge).TryParseFloat();
                Key = parameters.ElementOrDefault(FedExTemplate.Key);
                Password = parameters.ElementOrDefault(FedExTemplate.Password);

                EnabledService = GetEnabledService(parameters);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        /// <summary>
        /// Don't use this for FedEx
        /// </summary>
        /// <returns></returns>
        public float GetRate()
        {
            throw new Exception("GetRate method isnot avalible for FedEx");
        }

        public List<ShippingOption> GetShippingOptions()
        {
            var shippingOptions = new List<ShippingOption>();
            RateRequest request = CreateRateRequest();
            //
            var service = new RateService(); // Initialize the service
            try
            {
                // Call the web service passing in a RateRequest and returning a RateReply
                RateReply reply = service.getRates(request);

                if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING) // check if the call was successful
                {
                    shippingOptions = ParseAnswer(reply);
                }
                else
                {
                    Debug.LogError(new Exception(reply.Notifications[0].Message), false);
                }
            }
            catch (SoapException e)
            {
                Debug.LogError(e);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return shippingOptions;
        }

        public string GetServiceName(string serviceType)
        {
            switch (serviceType)
            {
                case "EUROPE_FIRST_INTERNATIONAL_PRIORITY":
                    return "FedEx Europe First International Priority";
                case "FEDEX_1_DAY_FREIGHT":
                    return "FedEx 1Day Freight";
                case "FEDEX_2_DAY":
                    return "FedEx 2Day";
                case "FEDEX_2_DAY_FREIGHT":
                    return "FedEx 2Day Freight";
                case "FEDEX_3_DAY_FREIGHT":
                    return "FedEx 3Day Freight";
                case "FEDEX_EXPRESS_SAVER":
                    return "FedEx Express Saver";
                case "FEDEX_GROUND":
                    return "FedEx Ground";
                case "FIRST_OVERNIGHT":
                    return "FexEx First Overnight";
                case "GROUND_HOME_DELIVERY":
                    return "FedEx Ground Home Delivery";
                case "INTERNATIONAL_DISTRIBUTION_FREIGHT":
                    return "FedEx International Distribution Freight";
                case "INTERNATIONAL_ECONOMY":
                    return "FedEx International Economy";
                case "INTERNATIONAL_ECONOMY_DISTRIBUTION":
                    return "FedEx International Economy Distribution";
                case "INTERNATIONAL_ECONOMY_FREIGHT":
                    return "FedEx International Economy Freight";
                case "INTERNATIONAL_FIRST":
                    return "FedEx International First";
                case "INTERNATIONAL_PRIORITY":
                    return "FedEx International Priority";
                case "INTERNATIONAL_PRIORITY_FREIGHT":
                    return "FedEx International Priority Freight";
                case "PRIORITY_OVERNIGHT":
                    return "FedEx Priority Overnight";
                case "SMART_POST":
                    return "FedEx Smart Post";
                case "STANDARD_OVERNIGHT":
                    return "FedEx Standard Overnight";
                case "FEDEX_FREIGHT":
                    return "FedEx Freight";
                case "FEDEX_NATIONAL_FREIGHT":
                    return "FedEx National Freight";
                default:
                    return "UNKNOWN";
            }
        }

        #region PrivateMethods
        private static string GetEnabledService(Dictionary<string, string> items)
        {
            var res = new StringBuilder();


            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.EuropeFirstInternationalPriority))
            {
                res.Append(FedExTemplate.EuropeFirstInternationalPriority + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.Fedex1DayFreight))
            {
                res.Append(FedExTemplate.Fedex1DayFreight + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.Fedex2Day))
            {
                res.Append(FedExTemplate.Fedex2Day + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.Fedex2DayFreight))
            {
                res.Append(FedExTemplate.Fedex2DayFreight + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.Fedex3DayFreight))
            {
                res.Append(FedExTemplate.Fedex3DayFreight + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.FedexExpressSaver))
            {
                res.Append(FedExTemplate.FedexExpressSaver + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.FedexGround))
            {
                res.Append(FedExTemplate.FedexGround + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.FirstOvernight))
            {
                res.Append(FedExTemplate.FirstOvernight + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.GroundHomeDelivery))
            {
                res.Append(FedExTemplate.GroundHomeDelivery + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.InternationalDistributionFreight))
            {
                res.Append(FedExTemplate.InternationalDistributionFreight + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.InternationalEconomy))
            {
                res.Append(FedExTemplate.InternationalEconomy + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.InternationalEconomyDistribution))
            {
                res.Append(FedExTemplate.InternationalEconomyDistribution + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.InternationalEconomyFreight))
            {
                res.Append(FedExTemplate.InternationalEconomyFreight + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.InternationalFirst))
            {
                res.Append(FedExTemplate.InternationalFirst + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.InternationalPriority))
            {
                res.Append(FedExTemplate.InternationalPriority + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.InternationalPriorityFreight))
            {
                res.Append(FedExTemplate.InternationalPriorityFreight + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.PriorityOvernight))
            {
                res.Append(FedExTemplate.PriorityOvernight + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.SmartPost))
            {
                res.Append(FedExTemplate.SmartPost + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.StandardOvernight))
            {
                res.Append(FedExTemplate.StandardOvernight + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.FedexFreight))
            {
                res.Append(FedExTemplate.FedexFreight + ";");
            }
            if (ShippingMethod.GetBooleanParam(items, FedExTemplate.FedexNationalFreight))
            {
                res.Append(FedExTemplate.FedexNationalFreight + ";");
            }

            return res.ToString();
        }

        private RateRequest CreateRateRequest()
        {
            // Build the RateRequest
            var request = new RateRequest();
            //
            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = Key; // Replace "XXX" with the Key
            request.WebAuthenticationDetail.UserCredential.Password = Password; // Replace "XXX" with the Password
            //
            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = AccountNumber; // Replace "XXX" with client's account number
            request.ClientDetail.MeterNumber = MeterNumber; // Replace "XXX" with client's meter number
            //
            request.TransactionDetail = new TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "***Rate for AdvantShop***"; // This is a reference field for the customer.  Any value can be used and will be provided in the response.
            //
            request.Version = new VersionId(); // WSDL version information, value is automatically set from wsdl
            //
            request.ReturnTransitAndCommit = true;
            request.ReturnTransitAndCommitSpecified = true;
            request.CarrierCodes = new CarrierCodeType[2];
            request.CarrierCodes[0] = CarrierCodeType.FDXE;
            request.CarrierCodes[1] = CarrierCodeType.FDXG;
            //
            SetShipmentDetails(request);
            //
            SetOrigin(request);
            //
            SetDestination(request);
            //
            SetPayment(request);
            //
            SetIndividualPackageLineItems(request);
            //
            return request;
        }

        private void SetShipmentDetails(RateRequest request)
        {
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP; //Drop off types are BUSINESS_SERVICE_CENTER, DROP_BOX, REGULAR_PICKUP, REQUEST_COURIER, STATION
            //request.RequestedShipment.ServiceType = ServiceType.INTERNATIONAL_PRIORITY; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...
            //request.RequestedShipment.ServiceTypeSpecified = true;
            //request.RequestedShipment.PackagingType = PackagingType.YOUR_PACKAGING; // Packaging type FEDEX_BOK, FEDEX_PAK, FEDEX_TUBE, YOUR_PACKAGING, ...
            //request.RequestedShipment.PackagingTypeSpecified = true;

            request.RequestedShipment.TotalInsuredValue = new Money();

            request.RequestedShipment.TotalInsuredValue.Amount = (decimal)TotalPrice; // Не использовать ShoppingCart.TotalPrice - не доступен из потока
            request.RequestedShipment.TotalInsuredValue.Currency = CurrencyIso3;
            request.RequestedShipment.ShipTimestamp = DateTime.Now; // Shipping date and time
            request.RequestedShipment.ShipTimestampSpecified = true;
            request.RequestedShipment.RateRequestTypes = new RateRequestType[2];
            request.RequestedShipment.RateRequestTypes[0] = RateRequestType.ACCOUNT;
            request.RequestedShipment.RateRequestTypes[1] = RateRequestType.LIST;
            request.RequestedShipment.PackageDetail = RequestedPackageDetailType.INDIVIDUAL_PACKAGES;
            request.RequestedShipment.PackageDetailSpecified = true;
        }

        private void SetOrigin(RateRequest request)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Address = new Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new string[1] { AddressFrom };
            request.RequestedShipment.Shipper.Address.City = CityFrom;
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = StateFrom;
            request.RequestedShipment.Shipper.Address.PostalCode = PostalCodeFrom;
            request.RequestedShipment.Shipper.Address.CountryCode = CountryCodeFrom;
        }

        private void SetDestination(RateRequest request)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Address = new Address();
            request.RequestedShipment.Recipient.Address.StreetLines = new string[1] { AddressTo };
            request.RequestedShipment.Recipient.Address.City = CityTo;
            request.RequestedShipment.Recipient.Address.StateOrProvinceCode = StateTo;
            request.RequestedShipment.Recipient.Address.PostalCode = PostalCodeTo;
            request.RequestedShipment.Recipient.Address.CountryCode = CountryCodeTo;
        }

        private void SetPayment(RateRequest request)
        {
            request.RequestedShipment.ShippingChargesPayment = new FedExRateServiceWebReference.Payment(); //new RateWebServiceClient.RateServiceWebReference.Payment(); 
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER; // Payment options are RECIPIENT, SENDER, THIRD_PARTY
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.AccountNumber = AccountNumber; // Replace "XXX" with client's account number
        }

        private void SetIndividualPackageLineItems(RateRequest request)
        {
            // ------------------------------------------
            // Passing individual pieces rate request
            // ------------------------------------------
            var weight = ShoppingCart.TotalShippingWeight;
            if (!IsPackageTooHeavy(weight))
            {
                request.RequestedShipment.PackageCount = "1";

                request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];
                request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem();
                request.RequestedShipment.RequestedPackageLineItems[0].SequenceNumber = "1"; // package sequence number            
                request.RequestedShipment.RequestedPackageLineItems[0].Weight = new Weight(); // package weight
                request.RequestedShipment.RequestedPackageLineItems[0].Weight.Units = WeightUnits.KG;
                request.RequestedShipment.RequestedPackageLineItems[0].Weight.Value = (decimal)weight;
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions = new Dimensions(); // package dimensions

                //it's better to don't pass dims now
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Length = "0";
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Width = "0";
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Height = "0";
                request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Units = LinearUnits.IN;
                request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue = new Money(); // insured value

                request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Amount = (decimal)TotalPrice;
                request.RequestedShipment.RequestedPackageLineItems[0].InsuredValue.Currency = CurrencyIso3;

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

                float orderSubTotal2 = ShoppingCart.TotalPrice / totalPackages;

                request.RequestedShipment.PackageCount = totalPackages.ToString();
                request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[totalPackages];

                for (int i = 0; i < totalPackages; i++)
                {
                    request.RequestedShipment.RequestedPackageLineItems[i] = new RequestedPackageLineItem();
                    request.RequestedShipment.RequestedPackageLineItems[i].SequenceNumber = (i + 1).ToString(); // package sequence number            
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight = new Weight(); // package weight
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.Units = WeightUnits.KG;
                    request.RequestedShipment.RequestedPackageLineItems[i].Weight.Value = (decimal)weight2;
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions = new Dimensions(); // package dimensions

                    //it's better to don't pass dims now
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Length = "0";
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Width = "0";
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Height = "0";
                    request.RequestedShipment.RequestedPackageLineItems[i].Dimensions.Units = LinearUnits.CM;
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue = new Money(); // insured value
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Amount = (decimal)orderSubTotal2;
                    request.RequestedShipment.RequestedPackageLineItems[i].InsuredValue.Currency = CurrencyIso3;
                }
            }
        }

        private static bool IsPackageTooHeavy(float weight)
        {
            if (weight > MaxPackageWeight)
                return true;
            return false;
        }

        private List<ShippingOption> ParseAnswer(RateReply reply)
        {
            var res = new List<ShippingOption>();
            var enabledServices = EnabledService;
            foreach (var rateDetail in reply.RateReplyDetails)
            {
                var shippingOption = new ShippingOption();
                if (!String.IsNullOrEmpty(enabledServices) && !enabledServices.Contains(rateDetail.ServiceType.ToString()))
                {
                    continue;
                }
                string serviceName = GetServiceName(rateDetail.ServiceType.ToString());
                shippingOption.Name = serviceName;
                foreach (RatedShipmentDetail shipmentDetail in rateDetail.RatedShipmentDetails)
                {
                    shippingOption.Rate = Rate > 0 ? (float)shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount * Rate + Extracharge
                                                   : (float)shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount + Extracharge;

                    // Vladimir: Старый код вытаскивал только некоторые Rate. Не знаю зачем. Пусть будут все.
                    //if (shipmentDetail.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT_SHIPMENT)
                    //{
                    //    shippingOption.Rate = shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount + Extracharge;
                    //    if (Rate > 0)
                    //    {
                    //        shippingOption.Rate *= Rate;
                    //    }
                    //    break;
                    //}

                    //if (shipmentDetail.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST_SHIPMENT) // Get List Rates (not discount rates)
                    //{
                    //    shippingOption.Rate = Rate > 0 ? shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount * Rate + Extracharge 
                    //                                   : shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount + Extracharge;
                    //    break;
                    //}

                    //var shippingRate = shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount + Extracharge;
                    //if (Rate > 0)
                    //{
                    //    shippingRate *= Rate;
                    //}
                    //shippingOption.Rate = shippingRate;
                }
                res.Add(shippingOption);
            }
            return res;
        }
        #endregion

    }
}