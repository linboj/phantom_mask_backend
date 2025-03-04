namespace Backend.Parameters;

public class SearchByNameParameter
{
    public SearchType Type { get; set; } = SearchType.Pharmacy;
    public string Keyword { get; set; } = "";
    public int Limit { get; set; } = 10;
    public int Offset { get; set; } = 0;
}