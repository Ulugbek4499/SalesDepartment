using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Contracts.Response;

namespace SalesDepartment.Application.UseCases.Reports
{
    public record GetContractsWithDepts(DateTime Time) : IRequest<ContractResponse[]>;

    public class GetAllContractsQueryHandler : IRequestHandler<GetContractsWithDepts, ContractResponse[]>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllContractsQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ContractResponse[]> Handle(GetContractsWithDepts request, CancellationToken cancellationToken)
        {
            var contracts = await _context.Contracts.ToArrayAsync();

            var contractResponses = new List<ContractResponse>();

            foreach (var contract in contracts)
            {
                Dictionary<DateTime, decimal> actualPaymentSchedule = contract.Payments
                    .ToDictionary(payment => payment.PaymentDate, payment => payment.Amount);

                Dictionary<DateTime, decimal> paymentSchedule = new Dictionary<DateTime, decimal>();

                DateTime paymentDate = contract.PaymentStartDate;

                for (int i = 1; i < contract.NumberOfMonths; i++)
                {
                    decimal paymentAmount = contract.InAdvancePaymentOfContract + (i * contract.AmountOfMonthlyPayment);

                    paymentSchedule.Add(paymentDate, paymentAmount);

                    paymentDate = paymentDate.AddMonths(1);
                }

                DateTime calculationDate = request.Time;
                var sumOfPayments = contract.Payments.Sum(x => x.Amount);
                var scheduledPayment = paymentSchedule.FirstOrDefault(x => x.Key.Month == calculationDate.Month && x.Key.Year == calculationDate.Year);
                decimal deptAmount = 0;

                if (scheduledPayment.Key != default(DateTime))
                {
                    decimal scheduledPaymentAmount = scheduledPayment.Value;

                    deptAmount = scheduledPaymentAmount - sumOfPayments;
                }

                ContractResponse contractResponse = new ContractResponse()
                {
                    Id = contract.Id,
                    ContractNumber = contract.ContractNumber,
                    ContractStartDate = contract.ContractStartDate,
                    PaymentStartDate = contract.PaymentStartDate,
                    TotalAmountOfContract = contract.TotalAmountOfContract,
                    InAdvancePaymentOfContract = contract.InAdvancePaymentOfContract,
                    NumberOfMonths = contract.NumberOfMonths,
                    AmountOfMonthlyPayment = contract.AmountOfMonthlyPayment,
                    HomeId = contract.HomeId,
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

                contractResponses.Add(contractResponse);
            }

            return contractResponses.ToArray();
        }
    }
}
