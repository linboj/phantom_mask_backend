namespace Backend.Seeds;

internal class PharmacyJson
{
    public string Name { get; set; }
    public decimal CashBalance { get; set; }
    public string OpeningHours { get; set; }

    public List<MaskJson> Masks { get; set; }
}

internal class MaskJson
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

internal class UserJson
{
    public string Name { get; set; }
    public decimal CashBalance { get; set; }
    public List<PurchaseHistory> PurchaseHistories { get; set; }
}

internal class PurchaseHistory
{
    public string PharmacyName { get; set; }
    public string MaskName { get; set; }
    public decimal TransactionAmount { get; set; }
    public DateTime TransactionDate { get; set; }
}