using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jennifer.Models;

public class JenniferEntityBase
{
    [Column(Order = 97)]
    [Required]
    public string CreatedBy { get; set; }
    [Column(Order = 98)]
    [Required]
    public DateTime CreatedOn { get; set; }
    [Column(Order = 99)]
    public string ModifiedBy { get; set; }
    [Column(Order = 100)]
    public DateTime? ModifiedOn { get; set; }
}