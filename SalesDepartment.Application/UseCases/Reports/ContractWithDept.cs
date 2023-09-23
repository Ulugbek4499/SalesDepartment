using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Resource;
using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Contracts.Response;
using SalesDepartment.Application.UseCases.Payments.Response;
using SalesDepartment.Domain.Entities;

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
            var contract = _dbContext.Contracts.Find(request.Id);

            Dictionary<DateTime, decimal> actualPaymentSchedule = contract.Payments
                         .ToDictionary(payment => payment.PaymentDate, payment => payment.Amount);


            Dictionary<DateTime, decimal> paymentSchedule = new Dictionary<DateTime, decimal>();

            DateTime paymentDate = contract.PaymentStartDate;

            for (int i = 0; i < contract.NumberOfMonths; i++)
            {
                decimal paymentAmount = contract.InAdvancePaymentOfContract + (i * contract.AmountOfMonthlyPayment);

                paymentSchedule.Add(paymentDate, paymentAmount);

                paymentDate = paymentDate.AddMonths(1);
            }

            DateTime calculationDate = request.DateTime;
            var sumOfPayments= contract.Payments.Sum(x => x.Amount);
            var scheduledPayment = paymentSchedule.FirstOrDefault(x => x.Key.Month == calculationDate.Month);
            decimal deptAmount=0;

            if (scheduledPayment.Key != default(DateTime))
            {
                decimal scheduledPaymentAmount = scheduledPayment.Value;

                deptAmount = scheduledPaymentAmount - sumOfPayments;
            }


            ContractResponse contractResponse = new ContractResponse()
            {
                Id=request.Id,
                ContractNumber= contract.ContractNumber,
                ContractStartDate= contract.ContractStartDate,
                PaymentStartDate=contract.PaymentStartDate,
                TotalAmountOfContract=contract.TotalAmountOfContract,
                InAdvancePaymentOfContract=contract.InAdvancePaymentOfContract,
                NumberOfMonths=contract.NumberOfMonths,
                AmountOfMonthlyPayment=contract.AmountOfMonthlyPayment,
                HomeId=contract.HomeId,
                Home = contract.Home,
                CustomerId = contract.CustomerId,
                Customer = contract.Customer,
                FounderId = contract.FounderId,
                Founder = contract.Founder,
                CreatedDate = contract.CreatedDate,
                ModifyDate = contract.ModifyDate,
                Payments = contract.Payments,
                DeptAmout = deptAmount,
                ScheduledInfo = paymentSchedule,
                ActualInfo = actualPaymentSchedule
            };

            return contractResponse;
        }
    }
}
