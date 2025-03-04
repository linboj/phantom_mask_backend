namespace Backend.Parameters;

public enum Order : byte
{
    DESC,
    ASC,
}

public enum DayOfWeekAbbr : ushort
{
    Sun = 0,
    Mon = 1,
    Tue = 2,
    Wed = 3,
    Thur = 4,
    Fri = 5,
    Sat = 6,
}

public enum MaskSortBy : short
{
    Name,
    Price,
}

public enum UserTranscationSortBy : short
{
    Quantity,
    Amount,
}

public enum SearchType : short
{
    Pharmacy,
    Mask,
}