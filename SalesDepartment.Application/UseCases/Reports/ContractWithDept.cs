using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Contracts.Response;
using SalesDepartment.Application.UseCases.Payments.Response;

namespace SalesDepartment.Application.UseCases.Reports
{
    public record ContractWithDept(int Id, DateTime DateTime) : IRequest<ContractResponse>;

    public class CustomerWithDeptHandler : IRequestHandler<ContractWithDept, ContractResponse>
    {
        private readonly IApplicationDbContext _dbContext;

        public CustomerWithDeptHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ContractResponse> Handle(ContractWithDept request, CancellationToken cancellationToken)
        {
            DateTime calculationDate = request.DateTime;

            var contract = _dbContext.Contracts.Find(request.Id);
            var sumOfPayments= contract.Payments.Sum(x => x.Amount);


           
            Dictionary<DateTime, decimal> paymentSchedule = new Dictionary<DateTime, decimal>();

            DateTime paymentDate = contract.PaymentStartDate;

            for (int i = 0; i < contract.NumberOfMonths; i++)
            {
                decimal paymentAmount = contract.InAdvancePaymentOfContract + (i * contract.AmountOfMonthlyPayment);

                paymentSchedule.Add(paymentDate, paymentAmount);

                paymentDate = paymentDate.AddMonths(1);
            }

            var scheduledPayment = paymentSchedule.FirstOrDefault(x => x.Key.Month == calculationDate.Month);

            if (scheduledPayment.Key != default(DateTime))
            {
                decimal scheduledPaymentAmount = scheduledPayment.Value;

                decimal dept = scheduledPaymentAmount - sumOfPayments;

            }

            return null; //contractsWithOutstandingPayments;
        }
    }
}
