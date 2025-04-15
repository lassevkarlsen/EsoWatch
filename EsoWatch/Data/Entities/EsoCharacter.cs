using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace EsoWatch.Data.Entities;

using System.ComponentModel.DataAnnotations;

[Index(nameof(UserId), IsUnique = false)]
public class EsoCharacter
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    public DateTime? DungeonRewardsAvailableAt { get; set; }

    [Required]
    public required Guid UserId { get; set; }
}