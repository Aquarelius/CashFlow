using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Models
{
    [Table("Tokens")]
    public class TokenModel
    {
        [Key]
        [MaxLength(255)]
        public string Key { get; set; }

        [MaxLength]
        public string Data { get; set; }

        public bool Fixed { get; set; }
    }
}