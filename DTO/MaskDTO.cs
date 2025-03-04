using Backend.Interface;

namespace Backend.DTO;

public class MaskBaseDTO
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
}
public class MaskInfoDTO : MaskBaseDTO, ISearchable
{
    public string Name { get; set; } = "";
    public string Color { get; set; } = "";
    public int QuantityPerPack { get; set; } = 0;
}