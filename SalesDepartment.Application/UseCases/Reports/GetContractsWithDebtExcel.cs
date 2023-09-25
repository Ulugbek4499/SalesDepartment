using System.Data;
using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.Common.Models;
using SalesDepartment.Application.UseCases.Contracts.Response;

namespace SalesDepartment.Application.UseCases.Reports
{
    public class GetContractsWithDebtExcel : IRequest<ExcelReportResponse>
    {
        public string FileName { get; set; }
    }

    public class GetContractsWithDebtExcelHandler : IRequestHandler<GetContractsWithDebtExcel, ExcelReportResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetContractsWithDebtExcelHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ExcelReportResponse> Handle(GetContractsWithDebtExcel request, CancellationToken cancellationToken)
        {
            using (XLWorkbook workbook = new())
            {
                var orderData = await GetContractsAsync(cancellationToken);
                var excelSheet = workbook.AddWorksheet(orderData, "Contracts");

                excelSheet.RowHeight = 20;
                excelSheet.Column(1).Width = 18;
                excelSheet.Column(2).Width = 18;
                excelSheet.Column(3).Width = 18;
                excelSheet.Column(4).Width = 18;
                excelSheet.Column(5).Width = 18;
                excelSheet.Column(6).Width = 18;
                excelSheet.Column(7).Width = 18;
                excelSheet.Column(8).Width = 18;
                excelSheet.Column(9).Width = 18;
                excelSheet.Column(10).Width = 18;
                excelSheet.Column(11).Width = 18;
                excelSheet.Column(12).Width = 18;
                excelSheet.Column(13).Width = 18;
                excelSheet.Column(14).Width = 18;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);

                    return new ExcelReportResponse(memoryStream.ToArray(), "Contract/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{request.FileName}.xlsx");
                }
            }
        }

        private async Task<DataTable> GetContractsAsync(CancellationToken cancellationToken = default)
        {
            var contracts = await _context.Contracts.ToArrayAsync();

            var contractResponses = new List<ContractResponse>();

            foreach (var contract in contracts)
            {
                Dictionary<DateTime, decimal> actualPaymentSchedule = contract.Payments
                    .ToDictionary(payment => payment.PaymentDate, payment => payment.Amount);

                Dictionary<DateTime, decimal> paymentSchedule = new Dictionary<DateTime, decimal>();

                DateTime paymentDate = contract.PaymentStartDate;

                for (int i = 1; i < contract.NumberOfMonths; i++)
                {
                    decimal paymentAmount = contract.InAdvancePaymentOfContract + (i * contract.AmountOfMonthlyPayment);

                    paymentSchedule.Add(paymentDate, paymentAmount);

                    paymentDate = paymentDate.AddMonths(1);
                }

                DateTime calculationDate = DateTime.Now;
                var sumOfPayments = contract.Payments.Sum(x => x.Amount);
                var scheduledPayment = paymentSchedule.FirstOrDefault(x => x.Key.Month == calculationDate.Month && x.Key.Year == calculationDate.Year);
                decimal deptAmount = 0;

                if (scheduledPayment.Key != default(DateTime))
                {
                    decimal scheduledPaymentAmount = scheduledPayment.Value;

                    deptAmount = scheduledPaymentAmount - sumOfPayments;
                }

                ContractResponse contractResponse = new ContractResponse()
                {
                    Id = contract.Id,
                    ContractNumber = contract.ContractNumber,
                    ContractStartDate = contract.ContractStartDate,
                    PaymentStartDate = contract.PaymentStartDate,
                    TotalAmountOfContract = contract.TotalAmountOfContract,
                    InAdvancePaymentOfContract = contract.InAdvancePaymentOfContract,
                    NumberOfMonths = contract.NumberOfMonths,
                    AmountOfMonthlyPayment = contract.AmountOfMonthlyPayment,
                    HomeId = contract.HomeId,
                    Home = contract.Home,
                    CustomerId = contract.CustomerId,
                    Customer = contract.Customer,
                    FounderId = contract.FounderId,
                    Founder = contract.Founder,
                    CreatedDate = contract.CreatedDate,
                    ModifyDate = contract.ModifyDate,
                    Payments = contract.Payments,
                    DeptAmout = deptAmount,
                    ScheduledInfo = paymentSchedule,
                    ActualInfo = actualPaymentSchedule
                };

                contractResponses.Add(contractResponse);
            }

            DataTable excelDataTable = new()
            {
                TableName = "Empdata"
            };

            excelDataTable.Columns.Add("Contract Number", typeof(string));
            excelDataTable.Columns.Add("Customer FullName", typeof(string));
            excelDataTable.Columns.Add("Customer Phone Number One", typeof(string));
            excelDataTable.Columns.Add("Customer Phone Number Two", typeof(string));
            excelDataTable.Columns.Add("Total Amount Of Contract", typeof(decimal));
            excelDataTable.Columns.Add("In Advance Payment Of Contract", typeof(decimal));
            excelDataTable.Columns.Add("Amount Of Monthly Payment", typeof(decimal));
            excelDataTable.Columns.Add("Scheduled Payment", typeof(decimal));
            excelDataTable.Columns.Add("Actual Payment", typeof(decimal));
            excelDataTable.Columns.Add("Dept Amout", typeof(decimal));
            excelDataTable.Columns.Add("Contract Start Date", typeof(DateTime));
            excelDataTable.Columns.Add("Payment Start Date", typeof(DateTime));
            excelDataTable.Columns.Add("Number Of Months", typeof(int));
            excelDataTable.Columns.Add("Home Info", typeof(string));

            var ContractsList = _mapper.Map<List<ContractResponse>>(contractResponses);

            if (ContractsList.Count > 0)
            {
                ContractsList.ForEach(item =>
                {
                    excelDataTable.Rows.Add(
                        item.ContractNumber, 
                        item.Customer.FirstName + " " + item.Customer.LastName + " " +item.Customer.MiddleName,
                        item.Customer.PhoneNumberOne,
                        item.Customer.PhoneNumberTwo,
                        item.TotalAmountOfContract, 
                        item.InAdvancePaymentOfContract,
                        item.AmountOfMonthlyPayment,
                        item.ScheduledInfo
                                .Where(entry => entry.Key.Month <= DateTime.Now.Month && entry.Key.Year <= DateTime.Now.Year)
                                .Sum(entry => entry.Value),
                        item.ActualInfo.Values.Sum(),
                        item.DeptAmout,
                        item.ContractStartDate, 
                        item.PaymentStartDate,
                        item.NumberOfMonths, 
                        item.Home.Block + " / " + item.Home.Entrance.ToString() + " / " + item.Home.Floor.ToString() + " / " + item.Home.ApartmentNumber.ToString() + " / " + item.Home.Area.ToString()
                        );
                });
            }

            return excelDataTable;
        }
    }
}
