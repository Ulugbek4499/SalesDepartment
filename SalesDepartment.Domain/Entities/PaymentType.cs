using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesDepartment.Domain.Commons;

namespace SalesDepartment.Domain.Entities
{
    public class PaymentType : BaseAuditableEntity
    {
        public string Name { get; set; }
    }
}
