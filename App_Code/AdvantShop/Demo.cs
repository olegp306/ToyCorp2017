//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Text;
using AdvantShop.Core;
using Resources;

namespace AdvantShop
{
    public class Demo
    {
        public static bool IsDemoEnabled
        {
            get { return ModeConfigService.IsModeEnabled(ModeConfigService.Modes.DemoMode); }
        }

        public static string GetRandomName()
        {
            var rnd = new Random();
            return GetRandomName(rnd);
        }

        private static string GetRandomName(Random rnd)
        {

            string[] strMassFirstName = {
                                            Resource.Client_OrderConfirmation_RandomName,
                                            Resource.Client_OrderConfirmation_RandomName1,
                                            Resource.Client_OrderConfirmation_RandomName2,
                                            Resource.Client_OrderConfirmation_RandomName3,
                                            Resource.Client_OrderConfirmation_RandomName4,
                                            Resource.Client_OrderConfirmation_RandomName5,
                                            Resource.Client_OrderConfirmation_RandomName6,
                                            Resource.Client_OrderConfirmation_RandomName7,
                                            Resource.Client_OrderConfirmation_RandomName8
                                        };

            var intTemp = rnd.Next(strMassFirstName.Length);

            return strMassFirstName[intTemp];
        }

        public static string GetRandomLastName()
        {
            var rnd = new Random();
            return GetRandomLastName(rnd);
        }

       private static string GetRandomLastName(Random rnd)
        {

            string[] strMassLastName = {
                                           Resource.Client_OrderConfirmation_RandomLastName,
                                           Resource.Client_OrderConfirmation_RandomLastName1,
                                           Resource.Client_OrderConfirmation_RandomLastName2,
                                           Resource.Client_OrderConfirmation_RandomLastName3,
                                           Resource.Client_OrderConfirmation_RandomLastName4,
                                           Resource.Client_OrderConfirmation_RandomLastName5,
                                           Resource.Client_OrderConfirmation_RandomLastName6,
                                           Resource.Client_OrderConfirmation_RandomLastName7,
                                           Resource.Client_OrderConfirmation_RandomLastName8
                                       };

            var intTemp = rnd.Next(strMassLastName.Length);

            return strMassLastName[intTemp];
        }

        public static string GetRandomCity()
        {
            var rnd = new Random();
            return GetRandomCity(rnd);
        }

        private static string GetRandomCity(Random rnd)
        {

            string[] strMassRandomCity = {
                                           Resource.Client_OrderConfirmation_RandomCity,
                                           Resource.Client_OrderConfirmation_RandomCity1,
                                           Resource.Client_OrderConfirmation_RandomCity2,
                                           Resource.Client_OrderConfirmation_RandomCity3,
                                           Resource.Client_OrderConfirmation_RandomCity4,
                                           Resource.Client_OrderConfirmation_RandomCity5,
                                           Resource.Client_OrderConfirmation_RandomCity6,
                                           Resource.Client_OrderConfirmation_RandomCity7,
                                           Resource.Client_OrderConfirmation_RandomCity8
                                       }; // 8

            var intTemp = rnd.Next(strMassRandomCity.Length);

            return strMassRandomCity[intTemp];
        }

        public static string GetRandomAdress()
        {
            var rnd = new Random();
            return GetRandomAdress(rnd);
        }

        private static string GetRandomAdress(Random rnd)
        {
            string[] strMassRandomAdress = {
                                           Resource.Client_OrderConfirmation_RandomAdress,
                                           Resource.Client_OrderConfirmation_RandomAdress1,
                                           Resource.Client_OrderConfirmation_RandomAdress2,
                                           Resource.Client_OrderConfirmation_RandomAdress3,
                                           Resource.Client_OrderConfirmation_RandomAdress4,
                                           Resource.Client_OrderConfirmation_RandomAdress5,
                                           Resource.Client_OrderConfirmation_RandomAdress6,
                                           Resource.Client_OrderConfirmation_RandomAdress7,
                                           Resource.Client_OrderConfirmation_RandomAdress8
                                       }; // 8

            var intTemp = rnd.Next(strMassRandomAdress.Length);

            return string.Format(Resource.Client_OrderConfirmation_RandomAdressFormat, strMassRandomAdress[intTemp], intTemp + 1);
        }

        public static string GetRandomPhone()
        {
            var rnd = new Random();
            return GetRandomPhone(rnd);
        }

        private static string GetRandomPhone(Random rnd)
        {

            int intTemp;

            var sb = new StringBuilder();

            for (byte i = 0; i <= 7; i++)
            {
                intTemp = rnd.Next(10);
                sb.Append(intTemp.ToString());
            }

            return string.Format("+0 92{0}", sb.ToString());
        }

        public static string GetRandomEmail()
        {
            var rnd = new Random();
            return GetRandomEmail(rnd);
        }

        private static string GetRandomEmail(Random rnd)
        {
            string[] strMassName = { "my", "main", "we", "love", "you", "life", "big", "hello", "haha" }; // 8
            string[] strMassDomain = { "net", "com", "ru", "tv", "ws", "us", "ee", "de", "info" }; // 8

            var strFirstPart = new StringBuilder();

            for (byte i = 0; i <= 1; i++)
            {
                int intTemp = rnd.Next(strMassName.Length);
                strFirstPart.Append(strMassName[intTemp]);
            }

            return string.Format("{0}@{1}.{2}",
                strFirstPart.ToString(),
                strMassName[rnd.Next(strMassName.Length)],
                strMassDomain[rnd.Next(strMassDomain.Length)]);
        }
    }
}