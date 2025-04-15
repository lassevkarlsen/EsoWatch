using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace EsoWatch.Data.Entities;

[Index(nameof(UserId), IsUnique = false)]
public class GenericTimer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(50)]
    public required string Name { get; set; }

    public DateTime? ElapsesAt { get; set; }

    public TimeSpan? Remaining { get; set; }

    public required TimeSpan Duration { get; set; }
    public bool NotificationSent { get; set; }

    [Required]
    public required Guid UserId { get; set; }
}