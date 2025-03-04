namespace Backend.Parameters;

public class GetOpenPharmaciesParameter
{
    public TimeOnly Time { get; set; }
    public DayOfWeekAbbr DayOfWeek { get; set; }
}

public class GetMasksOfPharmacyParameter
{
    public MaskSortBy SortBy { get; set; } = MaskSortBy.Name;
    public Order Order { get; set; } = Order.ASC;
}

public class FilterPharmaciesByMaskConditionParameter
{
    public decimal MinPrice { get; set; } = 0;
    public decimal MaxPrice { get; set; } = decimal.MaxValue;
    public int MaskCount { get; set; } = 0;
    public bool IsMoreThan { get; set; } = true;
}