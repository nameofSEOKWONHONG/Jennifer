using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jennifer.Domain.Account;

public class JenniferEntityBase
{
    [Column(Order = 97)]
    [Required]
    public Guid CreatedBy { get; set; }
    [Column(Order = 98)]
    [Required]
    public DateTime CreatedOn { get; set; }
    [Column(Order = 99)]
    public Guid? ModifiedBy { get; set; }
    [Column(Order = 100)]
    public DateTime? ModifiedOn { get; set; }
}