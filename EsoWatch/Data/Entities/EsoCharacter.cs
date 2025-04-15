using System.ComponentModel.DataAnnotations.Schema;

namespace EsoWatch.Data.Entities;

using System.ComponentModel.DataAnnotations;

public class EsoCharacter
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    public DateTime? DungeonRewardsAvailableAt { get; set; }
}