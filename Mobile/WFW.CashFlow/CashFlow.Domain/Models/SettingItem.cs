using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Models
{
    [Table("Settings")]
    public class SettingItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime LastChanged { get; set; }

    }
}
