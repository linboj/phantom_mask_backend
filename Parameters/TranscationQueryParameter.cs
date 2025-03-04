namespace Backend.Parameters;

public class InDateRangeQueryParameter
{
    public DateOnly StartDate { get; set; } = DateOnly.MinValue;
    public DateOnly EndDate { get; set; } = DateOnly.MaxValue;
}
public class TopUsersQueryParameter: InDateRangeQueryParameter
{
    public int Limit { get; set; } = 10;
}