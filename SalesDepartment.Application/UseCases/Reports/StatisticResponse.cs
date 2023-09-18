using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesDepartment.Application.UseCases.Reports
{
    public class StatisticResponse
    {
        public int AllContracts { get; set; }
        public int AllContractsInCurrentMonth { get; set; }
        public int AllContractsInCurrentWeek { get; set; }
        public int AllContractsInCurrentDay { get; set; }
        public decimal TotalAmountOfAllContracts { get; set; }
        public decimal TotalAmountOfAllContractsInCurrentMonth { get; set; }
        public decimal TotalAmountOfAllContractsInCurrentWeek { get; set; }
        public decimal TotalAmountOfAllContractsInCurrentDay { get; set; }
        public Dictionary<int, string> AllContractsByFounders { get; set;}
    }
}
