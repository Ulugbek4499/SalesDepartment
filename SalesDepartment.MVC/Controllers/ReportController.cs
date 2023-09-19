using Microsoft.AspNetCore.Mvc;
using SalesDepartment.Application.UseCases.Contracts.Queries.GetAllContracts;
using SalesDepartment.Application.UseCases.Contracts.Queries.GetContractById;
using SalesDepartment.Application.UseCases.Contracts.Reports;
using SalesDepartment.Application.UseCases.Contracts.Response;
using SalesDepartment.Application.UseCases.Reports;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace SalesDepartment.MVC.Controllers
{
    public class ReportController:ApiBaseController
    {

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> GetStatistics()
        {
            var statistic = await Mediator.Send(new GetStatisticsQuery());

            return View(statistic);
        }

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> GetContractsBetweenDates(DateTime fromDate, DateTime toDate, string fileName = "ContractExcel")
        {
            var result = await Mediator.Send(new ContractsByDayExcel { CreatedFromDate = fromDate, ToDate = toDate, FileName = fileName}) ;

            return File(result.FileContents, result.Option, result.FileName);
        }

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> GraphicInDocx(int id)
        {

            ContractResponse contract = await Mediator.Send(new GetContractByIdQuery(id));
            var templatePath = @"D:\PDP\SalesDepartment\SalesDepartment.MVC\wwwroot\docs\Grafik.docx";

            decimal SumOfOneMeterSquare = contract.TotalAmountOfContract / contract.Home.Area;

            var doc = DocX.Load(templatePath);

            doc.ReplaceText("«ДОГОВОР_»", contract.ContractNumber);
            doc.ReplaceText("«Дата_контракта»", contract.ContractStartDate.ToString());
            doc.ReplaceText("«ФИO_Инвестор»", contract.Customer.LastName + " " + contract.Customer.FirstName + " " + contract.Customer.MiddleName);
            doc.ReplaceText("«Паспорт_Серия_Инвестор»", contract.Customer.Passport);
            doc.ReplaceText("«Выдан_Паспорт__Инвестор»", contract.Customer.PassportIssuedBy);

            doc.ReplaceText("«Mесто_жительства»", contract.Customer.Address);
            doc.ReplaceText("««Здания»»", contract.Home.Block);
            doc.ReplaceText("«Подъезд_»", contract.Home.Entrance.ToString());
            doc.ReplaceText("«Квартира_»", contract.Home.ApartmentNumber.ToString());
            doc.ReplaceText("«Количество_комнат»", contract.Home.NumberOfRooms.ToString());

            doc.ReplaceText("«Этаже»", contract.Home.Floor.ToString());
            doc.ReplaceText("«Проектной_площадью»", contract.Home.Area.ToString());
            doc.ReplaceText("«Общая_сумма_контракта»", contract.TotalAmountOfContract.ToString());
            doc.ReplaceText("«advance_amount»", contract.InAdvancePaymentOfContract.ToString());
            doc.ReplaceText("«monthly_payment»", contract.AmountOfMonthlyPayment.ToString());
            doc.ReplaceText("«number_of_month»", contract.NumberOfMonths.ToString());
            doc.ReplaceText("«Сумма_1_М2»", SumOfOneMeterSquare.ToString());

            doc.ReplaceText("«Тел_Ном_»", contract.Customer.PhoneNumberOne);

            var paymentTable = doc.AddTable(contract.NumberOfMonths + 1, 4); // +1 for the header row
            paymentTable.Design = TableDesign.TableGrid;

            paymentTable.Rows[0].Cells[0].Paragraphs.First().Append("Number");
            paymentTable.Rows[0].Cells[1].Paragraphs.First().Append("Payment Days");
            paymentTable.Rows[0].Cells[2].Paragraphs.First().Append("Monthly Payment");
            paymentTable.Rows[0].Cells[3].Paragraphs.First().Append("Rest Of The Dept");

            decimal monthlyPayment = (contract.TotalAmountOfContract - contract.InAdvancePaymentOfContract) / contract.NumberOfMonths;
            decimal remainingDebt = contract.TotalAmountOfContract - contract.InAdvancePaymentOfContract;

            DateTime paymentDate = contract.PaymentStartDate;

            for (int month = 1; month <= contract.NumberOfMonths; month++)
            {
                // Fill in the table with the calculated values
                paymentTable.Rows[month].Cells[0].Paragraphs.First().Append(month.ToString());
                paymentTable.Rows[month].Cells[1].Paragraphs.First().Append(paymentDate.ToString("dd.MM.yyyy"));
                paymentTable.Rows[month].Cells[2].Paragraphs.First().Append(monthlyPayment.ToString());
                paymentTable.Rows[month].Cells[3].Paragraphs.First().Append(remainingDebt.ToString());

                // Update payment date and remaining debt for the next iteration
                paymentDate = paymentDate.AddMonths(1);
                remainingDebt -= monthlyPayment;
            }

            doc.InsertTable(paymentTable);

            var newFilename = $"Graphic_{contract.ContractNumber}.docx";

            doc.SaveAs(newFilename);

            var fileBytes = System.IO.File.ReadAllBytes(newFilename);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", newFilename);
        }
    }
}
