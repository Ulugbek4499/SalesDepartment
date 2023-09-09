using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesDepartment.Application.UseCases.Contracts.Response
{
    public class ContractResponse
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; }
        public DateTime ContractStartDate { get; set; }
        public double TotalAmountOfContract { get; set; }
        public DateTime ContractEndDate { get; set; }
        public int HomeId { get; set; }
        public int CustomerId { get; set; }
        public int FounderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
