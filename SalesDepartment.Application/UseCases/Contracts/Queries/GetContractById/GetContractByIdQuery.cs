using AutoMapper;
using MediatR;
using SalesDepartment.Application.Common.Exceptions;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Contracts.Response;
using SalesDepartment.Domain.Entities;

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
            var Contract = FilterIfContractExsists(request.Id);

            var result = _mapper.Map<ContractResponse>(Contract);
            return await Task.FromResult(result);
        }

        private Contract FilterIfContractExsists(int id)
            => _dbContext.Contracts
                .Find(id)
                     ?? throw new NotFoundException(
                            " There is no Contract with this Id. ");
    }
}
