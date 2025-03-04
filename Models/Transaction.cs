using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

/// <summary>
/// Represents a transaction of mask.
/// </summary>
public class Transaction
{
    public Guid Id { get; set; }

    /// <summary>
    /// The amount of the transaction.
    /// </summary>
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TransactionAmount { get; set; }

    /// <summary>
    /// The date and time of the transaction.
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// The user associated with the transaction.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    /// <summary>
    /// The pharmacy associated with the transaction.
    /// </summary>
    [Required]
    public Guid PharmacyId { get; set; }
    [ForeignKey("PharmacyId")]
    public Pharmacy Pharmacy { get; set; } = null!;

    /// <summary>
    /// The mask associated with the transaction.
    /// </summary>
    [Required]
    public Guid MaskId { get; set; }
    [ForeignKey("MaskId")]
    public Mask Mask { get; set; } = null!;
}