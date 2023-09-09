using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.UseCases.Customers.Response;
using SalesDepartment.Domain.Entities;

namespace SalesDepartment.Application.UseCases.Customers.Queries.GetAllCustomers
{
    public record GetAllCustomersQuery : IRequest<IEnumerable<CustomerResponse>>;

    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public GetAllCustomersQueryHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public Task<IEnumerable<CustomerResponse>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Customer> Customers = _context.Customers;

            return Task.FromResult(_mapper.Map<IEnumerable<CustomerResponse>>(Customers));
        }
    }
}
