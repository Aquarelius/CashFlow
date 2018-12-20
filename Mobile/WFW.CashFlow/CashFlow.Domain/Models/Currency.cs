﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Domain.Models
{
    [Table("Currencies")]
   public class Currency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Code { get; set; }

        public bool IsBaseCurrency { get; set; }

        public double Rate { get; set; }

        public DateTime LastUpdate { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}