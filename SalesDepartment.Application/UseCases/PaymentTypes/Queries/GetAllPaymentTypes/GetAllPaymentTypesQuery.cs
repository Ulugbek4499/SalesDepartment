using AutoMapper;
using MediatR;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.PaymentTypes.Response;
using SalesDepartment.Domain.Entities;

namespace SalesDepartment.Application.UseCases.PaymentTypes.Queries.GetAllPaymentTypes
{
    public record GetAllPaymentTypesQuery : IRequest<IEnumerable<PaymentTypeResponse>>;

    public class GetAllPaymentTypesQueryHandler : IRequestHandler<GetAllPaymentTypesQuery, IEnumerable<PaymentTypeResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllPaymentTypesQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public Task<IEnumerable<PaymentTypeResponse>> Handle(GetAllPaymentTypesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<PaymentType> PaymentTypes = _context.PaymentTypes;

            return Task.FromResult(_mapper.Map<IEnumerable<PaymentTypeResponse>>(PaymentTypes));
        }
    }
}
