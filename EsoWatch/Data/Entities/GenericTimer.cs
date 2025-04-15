using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EsoWatch.Data.Entities;

public class GenericTimer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(50)]
    public required string Name { get; set; }

    public DateTime? ElapsesAt { get; set; }

    public required TimeSpan Duration { get; set; }
    public bool NotificationSent { get; set; }
}