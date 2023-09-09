using AutoMapper;
using MediatR;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Homes.Response;
using SalesDepartment.Domain.Entities;

namespace SalesDepartment.Application.UseCases.Homes.Queries.GetAllHomes
{
    public record GetAllHomesQuery : IRequest<IEnumerable<HomeResponse>>;

    public class GetAllHomesQueryHandler : IRequestHandler<GetAllHomesQuery, IEnumerable<HomeResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllHomesQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public Task<IEnumerable<HomeResponse>> Handle(GetAllHomesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Home> Homes = _context.Homes;

            return Task.FromResult(_mapper.Map<IEnumerable<HomeResponse>>(Homes));
        }
    }
}
