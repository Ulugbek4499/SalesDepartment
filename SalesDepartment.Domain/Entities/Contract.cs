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
        public Home Home { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int FounderId { get; set; }
        public Founder Founder { get; set; }
    }
}