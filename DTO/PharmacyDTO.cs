using Backend.Interface;

namespace Backend.DTO;

public class PharmacyBaseDTO: ISearchable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
}
public class OpenedPharmacyDTO : PharmacyBaseDTO
{
    public int Week { get; set; }
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
}

public class PharmacyFilteredByMaskDTO : PharmacyBaseDTO
{
    public int NumberOfMasks { get; set; } = 0;
}