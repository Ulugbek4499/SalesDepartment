using SalesDepartment.Application.UseCases.Customers.Response;
using SalesDepartment.Application.UseCases.Founders.Response;
using SalesDepartment.Application.UseCases.Homes.Response;
using SalesDepartment.Domain.Entities;

namespace SalesDepartment.Application.UseCases.Contracts.Response
{
    public class ContractResponse
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; }
        public DateTime ContractStartDate { get; set; }
        public double TotalAmountOfContract { get; set; }
        public DateTime ContractEndDate { get; set; }
        public int NumberOfMonths { get; set; }
        public int PaymentDay { get; set; }
        public int HomeId { get; set; }
        public Home Home { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int FounderId { get; set; }
        public Founder Founder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public ICollection<Payment> Payments { get; set; }
    }
}
