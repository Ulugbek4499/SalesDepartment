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

        public int ContractId { get; set; }
        public virtual Contract Contract { get; set; }

        public int PaymentTypeId { get; set; }
        public virtual PaymentType PaymentType { get; set; }
    }
}
