using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Contracts.Response;

namespace SalesDepartment.Application.UseCases.Reports
{
    public record GetStatisticsQuery : IRequest<StatisticResponse>;

    public class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, StatisticResponse>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetStatisticsQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<StatisticResponse> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.Now;

            var contracts = await _context.Contracts.ToArrayAsync();

            var statistic = new StatisticResponse
            {
                AllContracts = contracts.Length,
                AllContractsInCurrentMonth = contracts.Count(c => c.ContractStartDate.Month == currentDate.Month),
                AllContractsInCurrentWeek = contracts.Count(c => (currentDate - c.ContractStartDate).TotalDays <= 7),
                AllContractsInCurrentDay = contracts.Count(c => c.ContractStartDate.Date == currentDate.Date),
                TotalAmountOfAllContracts = contracts.Sum(c => c.TotalAmountOfContract),
                TotalAmountOfAllContractsInCurrentMonth = contracts
                    .Where(c => c.ContractStartDate.Month == currentDate.Month)
                    .Sum(c => c.TotalAmountOfContract),
                TotalAmountOfAllContractsInCurrentWeek = contracts
                    .Where(c => (currentDate - c.ContractStartDate).TotalDays <= 7)
                    .Sum(c => c.TotalAmountOfContract),
                TotalAmountOfAllContractsInCurrentDay = contracts
                    .Where(c => c.ContractStartDate.Date == currentDate.Date)
                    .Sum(c => c.TotalAmountOfContract),
                AllContractsByFounders = contracts
                    .GroupBy(c => c.FounderId)
                    .ToDictionary(
                        group => group.Key,
                        group => string.Join(", ", group.Select(c => c.Founder.LastName + c.Founder.FirstName)))
            };

            return statistic;
        }
    }
}
