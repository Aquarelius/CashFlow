using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        public Transaction()
        {
            UserId = Guid.Empty;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public double Amount { get; set; }

        public double BaseCurrencyAmount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Description { get; set; }

        public int CurrencyId { get; set; }

        public bool IsDeleted { get; set; }

        public Guid DeletedBy { get; set; }

        public DateTime? DeletedAt { get; set; } 

        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }
    }
}
