using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace EsoWatch.Data.Entities;

[Index(nameof(UserId), IsUnique = true)]
public class UserSettings
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public required Guid UserId { get; set; }

    public bool UsePushover { get; set; }

    [MaxLength(50)]
    public string? PushoverUserKey { get; set; }

    [MaxLength(50)]
    public required string Title { get; set; }

    public static UserSettings DefaultForUser(Guid userId) => new()
    {
        UserId = userId,
        Title = $"User #{userId}",
    };
}