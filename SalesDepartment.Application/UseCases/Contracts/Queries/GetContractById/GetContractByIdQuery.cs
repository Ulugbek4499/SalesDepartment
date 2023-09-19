using System.Diagnostics.Contracts;
using AutoMapper;
using MediatR;
using SalesDepartment.Application.Common.Exceptions;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Contracts.Response;
using SalesDepartment.Domain.Entities;
using Telegram.Bot.Types;

namespace SalesDepartment.Application.UseCases.Contracts.Queries.GetContractById
{
    public record GetContractByIdQuery(int Id) : IRequest<ContractResponse>;

    public class GetContractByIdQueryHandler : IRequestHandler<GetContractByIdQuery, ContractResponse>
    {
        IApplicationDbContext _dbContext;
        IMapper _mapper;

        public GetContractByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ContractResponse> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
        {
            var contract = _dbContext.Contracts.Find(request.Id);

            var result = _mapper.Map<ContractResponse>(contract);

            // Initialize the ScheduledInfo dictionary
            result.ScheduledInfo = new Dictionary<DateOnly, decimal>();

            // Populate the ActualInfo dictionary
            result.ActualInfo = result.Payments.ToDictionary(
              payment => DateOnly.FromDateTime(payment.PaymentDate), // Convert PaymentDate to DateOnly
              payment => payment.Amount);


            decimal remainingDebt = result.TotalAmountOfContract - result.InAdvancePaymentOfContract;
            decimal monthlyPayment = remainingDebt / result.NumberOfMonths;
            DateOnly paymentDate = DateOnly.FromDateTime(result.PaymentStartDate);

            for (int month = 1; month <= result.NumberOfMonths; month++)
            {
                result.ScheduledInfo[paymentDate] = remainingDebt;
                paymentDate = paymentDate.AddMonths(1);
                remainingDebt -= monthlyPayment;
            }

            return await Task.FromResult(result);
        }
    }
}
