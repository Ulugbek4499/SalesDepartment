using System.Data;
using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesDepartment.Application.Common.Interfaces;
using SalesDepartment.Application.Common.Models;
using SalesDepartment.Application.UseCases.Contracts.Response;

namespace SalesDepartment.Application.UseCases.Contracts.Reports
{
    public class GetContractsExcel : IRequest<ExcelReportResponse>
    {
        public string FileName { get; set; }
    }

    public class GetContractsExcelHandler : IRequestHandler<GetContractsExcel, ExcelReportResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetContractsExcelHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ExcelReportResponse> Handle(GetContractsExcel request, CancellationToken cancellationToken)
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

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);

                    return new ExcelReportResponse(memoryStream.ToArray(), "Contract/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{request.FileName}.xlsx");
                }
            }
        }

        private async Task<DataTable> GetContractsAsync(CancellationToken cancellationToken = default)
        {
            var AllContracts = await _context.Contracts.ToListAsync(cancellationToken);

            DataTable excelDataTable = new()
            {
                TableName = "Empdata"
            };

            excelDataTable.Columns.Add("ContractNumber", typeof(string));
            excelDataTable.Columns.Add("ContractStartDate", typeof(DateTime));
            excelDataTable.Columns.Add("TotalAmountOfContract", typeof(double));
            excelDataTable.Columns.Add("ContractEndDate", typeof(DateTime));
            excelDataTable.Columns.Add("NumberOfMonths", typeof(int));
            excelDataTable.Columns.Add("PaymentDay", typeof(int));
            excelDataTable.Columns.Add("Home Number", typeof(string));
            excelDataTable.Columns.Add("Customer FullName", typeof(string));
            excelDataTable.Columns.Add("Founder Name", typeof(int));

            var ContractsList = _mapper.Map<List<ContractResponse>>(AllContracts);

            if (ContractsList.Count > 0)
            {
                ContractsList.ForEach(item =>
                {
                    excelDataTable.Rows.Add(item.ContractNumber, item.ContractStartDate, item.TotalAmountOfContract, item.ContractEndDate,
                        item.NumberOfMonths, item.PaymentDay, item.Home.Block + item.Home.Entrance.ToString() + item.Home.Floor.ToString() + item.Home.ApartmentNumber.ToString() + item.Home.Area.ToString(), item.Customer.FirstName + item.Customer.LastName, item.Founder.FirstName + item.Founder.LastName);
                });
            }

            return excelDataTable;
        }
    }
}
