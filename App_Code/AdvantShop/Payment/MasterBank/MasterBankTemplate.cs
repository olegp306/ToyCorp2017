//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for WebMoneyTemplate
    /// </summary>
    public struct MasterBankTemplate
    {
        //General params
        public const string Order = "ORDER";
        public const string Sign = "SIGN";
        public const string Timestamp = "TIMESTAMP";

        //POST params
        //1.	AMOUNT (����� � ������. ����������� ������ � �����)
        //2.	ORDER (���������� ����� ������. ������ ���� ����������. �������������� ��� ���������� �������. ��������� ������ ����� ������� 6-32 ��������.)
        //3.	MERCH_URL (URL, ������� ������������� �� ������ ������ � �������. ���� �� �����, ������� �� ���� �������� ���������)
        //4.	TERMINAL (��� ���������, ������������� ������)
        //5.	COUNTRY (������. ����������� ����������, ���� �������� ��������� �� � ������)
        //6.	TIMESTAMP (����� ���������� �������� � GMT (-4 ���� �� �����������). ������ YYYYMMDDHHMMSS)
        //7.	SIGN (�������� �������. ��������� �� ���������: md5(TERMINAL. TIMESTAMP.ORDER.AMOUNT.<��������� �����>) ����� ����� ����������� � �������� ������������)
        public const string Amount = "AMOUNT";
        public const string Merch_url = "MERCH_URL";
        public const string Terminal = "TERMINAL";
        public const string Country = "COUNTRY";

        public const string SecretWord = "SecretWord";
        
        //Responce params
        //1.	RESULT (��������� ��������. 0 � �������� 2 � ��������� 3 � ����������� �������� )
        //2.	RC (��� ������ ISO8583)
        //3.	CURRENCY (������)
        //4.	ORDER
        //5.	RRN (����� �������� � �������� �������)
        //6.	INT_REF (���������� ��� ��������)
        //7.	AUTHCODE (��� �����������. ����� �������������)
        //8.	PAN (��������������� ����� �����)
        //9.	TRTYPE (��� ��������. 0 � ����������� (��������� ������ ������������), 21 � ���������� �������, 24 � �������.)
        //10.	TIMESTAMP
        //11.	SIGN (������������ ��� ������������ �������)
        //12.	AMOUNT
        public const string Result = "RESULT";
        public const string Rc = "RC";
        public const string Currency = "CURRENCY";
        public const string Rrn = "RRN";
        public const string Int_ref = "INT_REF";
        public const string Authcode = "AUTHCODE";
        public const string Pan = "PAN";
    }
}