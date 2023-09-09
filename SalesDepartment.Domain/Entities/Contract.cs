using SalesDepartment.Domain.Commons;

namespace SalesDepartment.Domain.Entities
{
    public class Contract:BaseAuditableEntity
    {
        public string ContractNumber { get; set; }
        public DateTime ContractStartDate { get; set; }

        public double TotalAmountOfContract { get; set; }
        public DateTime ContractEndDate { get; set; }

        public int HomeId { get; set; }
        public virtual Home Home { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int FounderId { get; set; }
        public virtual Founder Founder { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}