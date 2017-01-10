namespace AdvantShop.BonusSystem
{
    public class BonusCardResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public BonusCard Data { get; set; }
    }

    public class BonusPurchaseResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public BonusPurchase Data { get; set; }
    }

    public class BonusPurchase
    {
        public string CardNumber { get; set; }
        public float PurchaseAmount { get; set; }
        public float NewBonusAmount { get; set; }
        public string Comment { get; set; }
        public string DocumentId { get; set; }
    }

    public class BonusSmsCodeResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public BonusSmsCode Data { get; set; }
    }

    public class BonusSmsCode
    {
        public string Code { get; set; }
    }

    public class BonusPhoneExistResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public BonusPhoneExist Data { get; set; }
    }

    public class BonusPhoneExist
    {
        public string Exist { get; set; }
    }

    public class BonusConfirmResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }

    public class BonusGradeResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public float Data { get; set; }
    }
}