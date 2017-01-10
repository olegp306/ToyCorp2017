//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Diagnostics;

namespace AdvantShop.Helpers
{
    public class NumberToString
    {
        public static string ConvertToString(float number)
        {
            return Num2Text(number);
        }

        // ���������� ����� �� ������������� ��������
        // ����� ����� �������� � ��������� � ���� ����
        private static string Num2Text(float dNum)
        {
            string strRes = "";
            string sR = ""; //�����
            string sK; //�������

            try
            {
                switch ((int)dNum % 10)
                {
                    case 0:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        sR = Num2Text12(dNum) + " ������ ";
                        break;
                    case 1:
                        sR = Num2Text12(dNum) + " ����� ";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        sR = Num2Text12(dNum) + " ����� ";
                        break;
                }

                if ((int)dNum % 100 == 11)
                {
                    sR = Num2Text12(dNum) + " ������ ";
                }

                var iK = (int)(Math.Round(dNum * 100, 0) % 100);

                if (iK < 10)
                {
                    sK = "0" + iK;
                }
                else
                {
                    sK = iK.ToString();
                }

                switch (iK % 10)
                {
                    case 0:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        sK = sK + " ������.";
                        break;
                    case 1:
                        sK = sK + " �������.";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        sK = sK + " �������.";
                        break;
                }

                if (iK == 11)
                {
                    sK = iK + " ������.";
                }

                if (dNum < 0)
                {
                    strRes = "����� " + sR + sK.Trim();
                }
                if (dNum >= 0)
                {
                    strRes = (sR + sK).Substring(0, 1).ToUpper() + (sR + sK).Trim().Substring(1);
                }
            }
            catch (Exception ex)
            {
                //Debug.LogError(ex, dNum);
                Debug.LogError(ex);
                strRes = "";
            }

            return strRes;
        }

        // ����� ����� �������� �������� ���� (1-� ������)
        private static string Num2Text1M(float dNum)
        {
            string returnValue;
            var s = string.Empty;
            dNum = (float)Math.Abs(Math.Floor(dNum));
            if (dNum < 0)
            {
                returnValue = dNum + "(������ 0)";
                return returnValue;
            }
            if (dNum > 9)
            {
                returnValue = dNum + "(������ 9)";
                return returnValue;
            }
            if (dNum == 0)
            {
                s = "����";
            }
            else if (dNum == 1)
            {
                s = "����";
            }
            else if (dNum == 2)
            {
                s = "���";
            }
            else if (dNum == 3)
            {
                s = "���";
            }
            else if (dNum == 4)
            {
                s = "������";
            }
            else if (dNum == 5)
            {
                s = "����";
            }
            else if (dNum == 6)
            {
                s = "�����";
            }
            else if (dNum == 7)
            {
                s = "����";
            }
            else if (dNum == 8)
            {
                s = "������";
            }
            else if (dNum == 9)
            {
                s = "������";
            }
            returnValue = s;
            return returnValue;
        }

        // ����� ����� �������� �������� ����(1-� ������)
        private static string Num2Text1W(double dNum)
        {
            string returnValue;
            var s = string.Empty;
            dNum = Math.Abs(Math.Floor(dNum));
            if (dNum < 0)
            {
                returnValue = dNum + "(������ 0)";
                return returnValue;
            }
            if (dNum > 9)
            {
                returnValue = dNum + "(������ 9)";
                return returnValue;
            }
            if (dNum == 0)
            {
                s = "����";
            }
            else if (dNum == 1)
            {
                s = "����";
            }
            else if (dNum == 2)
            {
                s = "���";
            }
            else if (dNum == 3)
            {
                s = "���";
            }
            else if (dNum == 4)
            {
                s = "������";
            }
            else if (dNum == 5)
            {
                s = "����";
            }
            else if (dNum == 6)
            {
                s = "�����";
            }
            else if (dNum == 7)
            {
                s = "����";
            }
            else if (dNum == 8)
            {
                s = "������";
            }
            else if (dNum == 9)
            {
                s = "������";
            }
            returnValue = s;
            return returnValue;
        }

        // ����� ����� �������� �������� ����(2-� ������)
        private static string Num2Text2M(float dNum)
        {
            string returnValue;
            var s = string.Empty;
            dNum = (float)Math.Abs(Math.Floor(dNum));
            if (dNum.ToString().Length <= 1)
            {
                returnValue = Num2Text1M(dNum);
                return returnValue;
            }
            if (dNum.ToString().Length > 2)
            {
                returnValue = dNum + "������ 99";
                return returnValue;
            }
            if (dNum == 10)
            {
                s = "������";
            }
            else if (dNum == 11)
            {
                s = "�����������";
            }
            else if (dNum == 12)
            {
                s = "����������";
            }
            else if (dNum == 13)
            {
                s = "����������";
            }
            else if (dNum == 14)
            {
                s = "������������";
            }
            else if (dNum == 15)
            {
                s = "����������";
            }
            else if (dNum == 16)
            {
                s = "�����������";
            }
            else if (dNum == 17)
            {
                s = "����������";
            }
            else if (dNum == 18)
            {
                s = "������������";
            }
            else if (dNum == 19)
            {
                s = "������������";
            }
            else if ((((((((((dNum == 20) || (dNum == 21)) || (dNum == 22)) || (dNum == 23)) || (dNum == 24)) ||
                         (dNum == 25)) || (dNum == 26)) || (dNum == 27)) || (dNum == 28)) || (dNum == 29))
            {
                s = "��������";
            }
            else if ((((((((((dNum == 30) || (dNum == 31)) || (dNum == 32)) || (dNum == 33)) || (dNum == 34)) ||
                         (dNum == 35)) || (dNum == 36)) || (dNum == 37)) || (dNum == 38)) || (dNum == 39))
            {
                s = "��������";
            }
            else if ((((((((((dNum == 40) || (dNum == 41)) || (dNum == 42)) || (dNum == 43)) || (dNum == 44)) ||
                         (dNum == 45)) || (dNum == 46)) || (dNum == 47)) || (dNum == 48)) || (dNum == 49))
            {
                s = "�����";
            }
            else if ((((((((((dNum == 50) || (dNum == 51)) || (dNum == 52)) || (dNum == 53)) || (dNum == 54)) ||
                         (dNum == 55)) || (dNum == 56)) || (dNum == 57)) || (dNum == 58)) || (dNum == 59))
            {
                s = "���������";
            }
            else if ((((((((((dNum == 60) || (dNum == 61)) || (dNum == 62)) || (dNum == 63)) ||
                          (dNum == 64)) || (dNum == 65)) || (dNum == 66)) || (dNum == 67)) ||
                      (dNum == 68)) || (dNum == 69))
            {
                s = "����������";
            }
            else if ((((((((((dNum == 70) || (dNum == 71)) || (dNum == 72)) || (dNum == 73)) ||
                          (dNum == 74)) || (dNum == 75)) || (dNum == 76)) || (dNum == 77)) ||
                      (dNum == 78)) || (dNum == 79))
            {
                s = "���������";
            }
            else if ((((((((((dNum == 80) || (dNum == 81)) || (dNum == 82)) || (dNum == 83)) ||
                          (dNum == 84)) || (dNum == 85)) || (dNum == 86)) || (dNum == 87)) ||
                      (dNum == 88)) || (dNum == 89))
            {
                s = "�����������";
            }
            else if ((((((((((dNum == 90) || (dNum == 91)) || (dNum == 92)) || (dNum == 93)) ||
                          (dNum == 94)) || (dNum == 95)) || (dNum == 96)) || (dNum == 97)) ||
                      (dNum == 98)) || (dNum == 99))
            {
                s = "���������";
            }
            int iLeft = int.Parse(dNum.ToString().Substring(0, 1));
            int iRight = int.Parse(dNum.ToString().Substring(dNum.ToString().Length - 1, 1));
            if (iLeft >= 2 && iLeft <= 9 && iRight >= 1 && iRight <= 9)
            {
                s = s + " " + Num2Text1M(int.Parse(dNum.ToString().Substring(dNum.ToString().Length - 1, 1)));
            }
            returnValue = s;
            return returnValue;
        }

        // ����� ����� �������� �������� ����(2-� ������)
        private static string Num2Text2W(double dNum)
        {
            string returnValue;
            var s = string.Empty;
            dNum = Math.Abs(Math.Floor(dNum));
            if (dNum.ToString().Length <= 1)
            {
                returnValue = Num2Text1W(dNum);
                return returnValue;
            }
            if (dNum.ToString().Length > 2)
            {
                returnValue = dNum + "������ 99";
                return returnValue;
            }
            if (dNum == 10)
            {
                s = "������";
            }
            else if (dNum == 11)
            {
                s = "�����������";
            }
            else if (dNum == 12)
            {
                s = "����������";
            }
            else if (dNum == 13)
            {
                s = "����������";
            }
            else if (dNum == 14)
            {
                s = "������������";
            }
            else if (dNum == 15)
            {
                s = "����������";
            }
            else if (dNum == 16)
            {
                s = "�����������";
            }
            else if (dNum == 17)
            {
                s = "����������";
            }
            else if (dNum == 18)
            {
                s = "������������";
            }
            else if (dNum == 19)
            {
                s = "������������";
            }
            else if ((((((((((dNum == 20) || (dNum == 21)) || (dNum == 22)) || (dNum == 23)) || (dNum == 24)) ||
                         (dNum == 25)) || (dNum == 26)) || (dNum == 27)) || (dNum == 28)) || (dNum == 29))
            {
                s = "��������";
            }
            else if ((((((((((dNum == 30) || (dNum == 31)) || (dNum == 32)) || (dNum == 33)) || (dNum == 34)) ||
                         (dNum == 35)) || (dNum == 36)) || (dNum == 37)) || (dNum == 38)) || (dNum == 39))
            {
                s = "��������";
            }
            else if ((((((((((dNum == 40) || (dNum == 41)) || (dNum == 42)) || (dNum == 43)) || (dNum == 44)) ||
                         (dNum == 45)) || (dNum == 46)) || (dNum == 47)) || (dNum == 48)) || (dNum == 49))
            {
                s = "�����";
            }
            else if ((((((((((dNum == 50) || (dNum == 51)) || (dNum == 52)) || (dNum == 53)) || (dNum == 54)) ||
                         (dNum == 55)) || (dNum == 56)) || (dNum == 57)) || (dNum == 58)) || (dNum == 59))
            {
                s = "���������";
            }
            else if ((((((((((dNum == 60) || (dNum == 61)) || (dNum == 62)) || (dNum == 63)) ||
                          (dNum == 64)) || (dNum == 65)) || (dNum == 66)) || (dNum == 67)) ||
                      (dNum == 68)) || (dNum == 69))
            {
                s = "����������";
            }
            else if ((((((((((dNum == 70) || (dNum == 71)) || (dNum == 72)) || (dNum == 73)) ||
                          (dNum == 74)) || (dNum == 75)) || (dNum == 76)) || (dNum == 77)) ||
                      (dNum == 78)) || (dNum == 79))
            {
                s = "���������";
            }
            else if ((((((((((dNum == 80) || (dNum == 81)) || (dNum == 82)) || (dNum == 83)) ||
                          (dNum == 84)) || (dNum == 85)) || (dNum == 86)) || (dNum == 87)) ||
                      (dNum == 88)) || (dNum == 89))
            {
                s = "�����������";
            }
            else if ((((((((((dNum == 90) || (dNum == 91)) || (dNum == 92)) || (dNum == 93)) ||
                          (dNum == 94)) || (dNum == 95)) || (dNum == 96)) || (dNum == 97)) ||
                      (dNum == 98)) || (dNum == 99))
            {
                s = "���������";
            }
            var iLeft = int.Parse(dNum.ToString().Substring(0, 1));
            var iRight = int.Parse(dNum.ToString().Substring(dNum.ToString().Length - 1, 1));
            if (iLeft >= 2 && iLeft <= 9 && iRight >= 1 && iRight <= 9)
            {
                s = s + " " + Num2Text1W(int.Parse(dNum.ToString().Substring(dNum.ToString().Length - 1, 1)));
            }
            returnValue = s;
            return returnValue;
        }

        // ����� ����� �������� �������� ����(3-� ������)
        private static string Num2Text3M(float dNum)
        {
            string returnValue;
            var s = string.Empty;
            dNum = (float)Math.Abs(Math.Floor(dNum));
            if (dNum.ToString().Length <= 2)
            {
                returnValue = Num2Text2M(dNum);
                return returnValue;
            }
            if (dNum.ToString().Length > 3)
            {
                returnValue = dNum + "(������ 999)";
                return returnValue;
            }
            if (dNum >= 100 && dNum < 200)
            {
                s = "���";
            }
            if (dNum >= 200 && dNum < 300)
            {
                s = "������";
            }
            if (dNum >= 300 && dNum < 400)
            {
                s = "������";
            }
            if (dNum >= 400 && dNum < 500)
            {
                s = "���������";
            }
            if (dNum >= 500 && dNum < 600)
            {
                s = "�������";
            }
            if (dNum >= 600 && dNum < 700)
            {
                s = "��������";
            }
            if (dNum >= 700 && dNum < 800)
            {
                s = "�������";
            }
            if (dNum >= 800 && dNum < 900)
            {
                s = "���������";
            }
            if (dNum >= 900 && dNum < 1000)
            {
                s = "���������";
            }
            if ((int)dNum % 100 != 0)
            {
                s = s + " " + Num2Text2M((int)dNum % 100);
            }
            returnValue = s;
            return returnValue;
        }

        // ����� ����� �������� �������� ����(3-� ������)
        private static string Num2Text3W(double dNum)
        {
            string returnValue;
            var s = string.Empty;
            dNum = Math.Abs(Math.Floor(dNum));
            if (dNum.ToString().Length <= 2)
            {
                returnValue = Num2Text2W(dNum);
                return returnValue;
            }
            if (dNum.ToString().Length > 3)
            {
                returnValue = dNum + "(������ 999)";
                return returnValue;
            }
            if (dNum >= 100 && dNum < 200)
            {
                s = "���";
            }
            if (dNum >= 200 && dNum < 300)
            {
                s = "������";
            }
            if (dNum >= 300 && dNum < 400)
            {
                s = "������";
            }
            if (dNum >= 400 && dNum < 500)
            {
                s = "���������";
            }
            if (dNum >= 500 && dNum < 600)
            {
                s = "�������";
            }
            if (dNum >= 600 && dNum < 700)
            {
                s = "��������";
            }
            if (dNum >= 700 && dNum < 800)
            {
                s = "�������";
            }
            if (dNum >= 800 && dNum < 900)
            {
                s = "���������";
            }
            if (dNum >= 900 && dNum < 1000)
            {
                s = "���������";
            }
            if ((int)dNum % 100 != 0)
            {
                s = s + " " + Num2Text2W((int)dNum % 100);
            }
            returnValue = s;
            return returnValue;
        }

        // ����� ����� �������� (�� 999 999)
        private static string Num2Text6(float dNum)
        {
            string returnValue;
            var s = "�����";
            dNum = (float)Math.Abs(Math.Floor(dNum));
            if (dNum.ToString().Length <= 3)
            {
                returnValue = Num2Text3M(dNum);
                return returnValue;
            }
            if (dNum.ToString().Length > 6)
            {
                returnValue = dNum + "(������ 999 999)";
                return returnValue;
            }
            switch ((int)(dNum / 1000) % 10)
            {
                case 1:
                    s = "������";
                    break;
                case 2:
                case 3:
                case 4:
                    s = "������";
                    break;
            }
            if ((int)(dNum / 1000) % 100 == 11)
            {
                s = "�����";
            }
            if ((int)(dNum) % 1000 != 0)
            {
                returnValue = Num2Text3W((int)(dNum / 1000)) + " " + s + " " +
                              Num2Text3M((int)(dNum) % 1000).Trim();
            }
            else
            {
                returnValue = Num2Text3W((int)(dNum / 1000)).Trim();
            }
            return returnValue;
        }

        // ����� ����� �������� (�� 999 999 999)
        private static string Num2Text9(float dNum)
        {
            string returnValue;
            var s = "���������";
            dNum = (float)Math.Abs(Math.Floor(dNum));
            if (dNum.ToString().Length <= 6)
            {
                returnValue = Num2Text6(dNum);
                return returnValue;
            }
            if (dNum.ToString().Length > 9)
            {
                returnValue = dNum + "(������ 999 999 999)";
                return returnValue;
            }
            switch ((int)(dNum / 1000000) % 1)
            {
                case 1:
                    s = "�������";
                    break;
                case 2:
                case 3:
                case 4:
                    s = "��������";
                    break;
            }
            if ((int)(dNum / 1000000) % 100 == 11)
            {
                s = "���������";
            }
            returnValue = Num2Text3M((float)Math.Floor(dNum / 1000000)) + " " + s + " " +
                          Num2Text6((float)(Double.Parse(dNum.ToString().Substring(dNum.ToString().Length - 6, 6)))).Trim();
            return returnValue;
        }

        // ����� ����� �������� (�� 999 999 999 999)
        private static string Num2Text12(float dNum)
        {
            string returnValue;
            var s = "����������";
            dNum = (float)Math.Abs(Math.Floor(dNum));
            if (dNum.ToString().Length <= 9)
            {
                returnValue = Num2Text9(dNum);
                return returnValue;
            }
            if (dNum.ToString().Length > 12)
            {
                returnValue = dNum + "(������ 999 999 999 999)";
                return returnValue;
            }
            switch ((int)(dNum / 1000000000) % 10)
            {
                case 1:
                    s = "��������";
                    break;
                case 2:
                case 3:
                case 4:
                    s = "���������";
                    break;
            }
            if ((int)(dNum / 1000000000) % 100 == 11)
            {
                s = "����������";
            }
            returnValue = Num2Text3M((float)Math.Floor(dNum / 1000000000)) + " " + s + " " +
                          Num2Text9(float.Parse(dNum.ToString().Substring(dNum.ToString().Length - 9, 9))).Trim();
            return returnValue;
        }
    }
}