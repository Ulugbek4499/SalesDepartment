using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesDepartment.Domain.Commons;

namespace SalesDepartment.Domain.Entities
{
    public class Payment:BaseAuditableEntity
    {
        public string PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }

        public double Amount { get; set; }
        public string AmountsInWords { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int ContractId { get; set; }
        public Contract Contract { get; set; }

        public int PaymentTypeId { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}

/*
        public string PassportNumber { get; set; }
        public string ContractNumber { get; set; }
        public string FullName { get; set; }
*/
