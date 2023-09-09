using Microsoft.AspNetCore.Mvc;
using SalesDepartment.Application.UseCases.Contracts.Commands.CreateContract;
using SalesDepartment.Application.UseCases.Contracts.Commands.DeleteContract;
using SalesDepartment.Application.UseCases.Contracts.Commands.UpdateContract;
using SalesDepartment.Application.UseCases.Contracts.Queries.GetAllContracts;
using SalesDepartment.Application.UseCases.Contracts.Queries.GetContractById;

namespace SalesDepartment.MVC.Controllers
{
    public class ContractController : ApiBaseController
    {
        [HttpGet("[action]")]
        public async ValueTask<IActionResult> CreateContract()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async ValueTask<IActionResult> CreateContract([FromForm] CreateContractCommand Contract)
        {
            await Mediator.Send(Contract);

            return RedirectToAction("GetAllContracts");
        }

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> CreateContractFromExcel()
        {
            return View();
        }

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> GetAllContracts()
        {
            var Contracts = await Mediator.Send(new GetAllContractsQuery());

            return View(Contracts);
        }

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> UpdateContract(int Id)
        {
            var Contract = await Mediator.Send(new GetContractByIdQuery(Id));

            return View(Contract);
        }

        [HttpPost("[action]")]
        public async ValueTask<IActionResult> UpdateContract([FromForm] UpdateContractCommand Contract)
        {
            await Mediator.Send(Contract);
            return RedirectToAction("GetAllContracts");
        }

        public async ValueTask<IActionResult> DeleteContract(int Id)
        {
            await Mediator.Send(new DeleteContractCommand(Id));

            return RedirectToAction("GetAllContracts");
        }

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> ViewContract(int id)
        {
            var Contract = await Mediator.Send(new GetContractByIdQuery(id));

            return View("ViewContract", Contract);
        }

        /*       [HttpPost("[action]")]
               public async ValueTask<IActionResult> CreateContractFromExcel(IFormFile excelfile)
               {
                   var result = await Mediator.Send(new AddContractsFromExcel(excelfile));

                   return RedirectToAction("GetAllContracts");
               }*/
        /*
                [HttpGet("[action]")]
                public async ValueTask<IActionResult> CreateContractFromCSV()
                {
                    return View();
                }*/

        /*        [HttpPost("[action]")]
                public async ValueTask<IActionResult> CreateContractFromCSV(IFormFile csvfile)
                {
                    var result = await Mediator.Send(new AddContractsFromCsv(csvfile));

                    return RedirectToAction("GetAllContracts");
                }*/

        /*
                [HttpGet("[action]")]
                public async ValueTask<FileResult> GetAllContractsExcel(string fileName = "ContractsExcel")
                {
                    var result = await Mediator.Send(new GetContractsExcel { FileName = fileName });

                    return File(result.FileContents, result.Option, result.FileName);
                }*/

        /*       [HttpGet("[action]")]
               public async ValueTask<IActionResult> GetAllContractsCsv(string fileName = "ContractsCsv")
               {
                   var result = await Mediator.Send(new GetContractsCsv { FileName = fileName });

                   return File(result.FileContents, result.Option, result.FileName);
               }*/

        /*       [HttpGet("[action]")]
               public async Task<IActionResult> GetAllContractsPDF(string fileName = "ContractsPDF")
               {
                   var result = await Mediator.Send(new GetContractPDF(FileName: fileName));

                   return File(result.FileContents, result.Options, result.FileName);
               }*/
    }
}
