using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Founders.Response;
using SalesDepartment.Domain.Entities;

namespace SalesDepartment.Application.UseCases.Founders.Queries.GetAllFounders
{
    public record GetAllFoundersQuery : IRequest<IEnumerable<FounderResponse>>;

    public class GetAllFoundersQueryHandler : IRequestHandler<GetAllFoundersQuery, IEnumerable<FounderResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllFoundersQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public Task<IEnumerable<FounderResponse>> Handle(GetAllFoundersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Founder> Founders = _context.Founders;

            return Task.FromResult(_mapper.Map<IEnumerable<FounderResponse>>(Founders));
        }
    }
}
