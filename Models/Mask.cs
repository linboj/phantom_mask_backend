using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>
/// Represents a mask with its type, and price at which pharmacy it was sold.
/// </summary>
public class Mask
{
    public Guid Id { get; set; }

    /// <summary>
    /// Price of the mask.
    /// </summary>
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
    public decimal Price { get; set; }

    /// <summary>
    /// Identifier of the mask type.
    /// </summary>
    [Required]
    public required Guid MaskTypeId { get; set; }
    [ForeignKey("MaskTypeId")]
    public MaskType MaskType { get; set; } = null!;

    /// <summary>
    /// Identifier of the pharmacy where the mask is sold.
    /// </summary>
    [Required]
    public Guid PharmacyId { get; set; }
    [ForeignKey("PharmacyId")]
    public Pharmacy Pharmacy { get; set; } = null!;
}