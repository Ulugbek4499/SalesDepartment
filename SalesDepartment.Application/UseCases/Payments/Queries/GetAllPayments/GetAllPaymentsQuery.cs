using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Payments.Response;
using SalesDepartment.Domain.Entities;

namespace SalesDepartment.Application.UseCases.Payments.Queries.GetAllPayments
{
    public record GetAllPaymentsQuery : IRequest<IEnumerable<PaymentResponse>>;

    public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, IEnumerable<PaymentResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllPaymentsQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public Task<IEnumerable<PaymentResponse>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Payment> Payments = _context.Payments;

            return Task.FromResult(_mapper.Map<IEnumerable<PaymentResponse>>(Payments));
        }
    }
}
