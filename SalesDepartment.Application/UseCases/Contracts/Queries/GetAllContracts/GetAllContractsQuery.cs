using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Contracts.Response;
using SalesDepartment.Domain.Entities;

namespace SalesDepartment.Application.UseCases.Contracts.Queries.GetAllContracts
{
    public record GetAllContractsQuery : IRequest<IEnumerable<ContractResponse>>;

    public class GetAllContractsQueryHandler : IRequestHandler<GetAllContractsQuery, IEnumerable<ContractResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllContractsQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public Task<IEnumerable<ContractResponse>> Handle(GetAllContractsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Contract> Contracts = _context.Contracts;

            return Task.FromResult(_mapper.Map<IEnumerable<ContractResponse>>(Contracts));
        }
    }
}
