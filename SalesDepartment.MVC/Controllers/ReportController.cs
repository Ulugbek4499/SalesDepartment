using Microsoft.AspNetCore.Mvc;
using SalesDepartment.Application.UseCases.Contracts.Queries.GetAllContracts;
using SalesDepartment.Application.UseCases.Contracts.Reports;
using SalesDepartment.Application.UseCases.Reports;

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
    }
}
