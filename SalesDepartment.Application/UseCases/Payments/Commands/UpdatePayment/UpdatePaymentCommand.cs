using AutoMapper;
using MediatR;
using SalesDepartment.Application.Common.Exceptions;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Domain.Entities;

namespace SalesDepartment.Application.UseCases.Payments.Commands.UpdatePayment
{
    public class UpdatePaymentCommand : IRequest
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public string AmountsInWords { get; set; }
        public int ContractId { get; set; }
        public int PaymentTypeId { get; set; }
    }

    public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public UpdatePaymentCommandHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
        {
            Payment? Payment = await _context.Payments.FindAsync(request.Id);
            _mapper.Map(Payment, request);

            if (Payment is null)
                throw new NotFoundException(nameof(Payment), request.Id);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}