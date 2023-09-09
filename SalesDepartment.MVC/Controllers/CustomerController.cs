using Microsoft.AspNetCore.Mvc;
using SalesDepartment.Application.UseCases.Customers.Commands.CreateCustomer;
using SalesDepartment.Application.UseCases.Customers.Commands.DeleteCustomer;
using SalesDepartment.Application.UseCases.Customers.Commands.UpdateCustomer;
using SalesDepartment.Application.UseCases.Customers.Queries.GetAllCustomers;
using SalesDepartment.Application.UseCases.Customers.Queries.GetCustomerById;

namespace SalesDepartment.MVC.Controllers
{
    public class CustomerController : ApiBaseController
    {
        [HttpGet("[action]")]
        public async ValueTask<IActionResult> CreateCustomer()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async ValueTask<IActionResult> CreateCustomer([FromForm] CreateCustomerCommand Customer)
        {
            await Mediator.Send(Customer);

            return RedirectToAction("GetAllCustomers");
        }

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> CreateCustomerFromExcel()
        {
            return View();
        }

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> GetAllCustomers()
        {
            var Customers = await Mediator.Send(new GetAllCustomersQuery());

            return View(Customers);
        }

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> UpdateCustomer(int Id)
        {
            var Customer = await Mediator.Send(new GetCustomerByIdQuery(Id));

            return View(Customer);
        }

        [HttpPost("[action]")]
        public async ValueTask<IActionResult> UpdateCustomer([FromForm] UpdateCustomerCommand Customer)
        {
            await Mediator.Send(Customer);
            return RedirectToAction("GetAllCustomers");
        }

        public async ValueTask<IActionResult> DeleteCustomer(int Id)
        {
            await Mediator.Send(new DeleteCustomerCommand(Id));

            return RedirectToAction("GetAllCustomers");
        }

        [HttpGet("[action]")]
        public async ValueTask<IActionResult> ViewCustomer(int id)
        {
            var Customer = await Mediator.Send(new GetCustomerByIdQuery(id));

            return View("ViewCustomer", Customer);
        }

        /*       [HttpPost("[action]")]
               public async ValueTask<IActionResult> CreateCustomerFromExcel(IFormFile excelfile)
               {
                   var result = await Mediator.Send(new AddCustomersFromExcel(excelfile));

                   return RedirectToAction("GetAllCustomers");
               }*/
        /*
                [HttpGet("[action]")]
                public async ValueTask<IActionResult> CreateCustomerFromCSV()
                {
                    return View();
                }*/

        /*        [HttpPost("[action]")]
                public async ValueTask<IActionResult> CreateCustomerFromCSV(IFormFile csvfile)
                {
                    var result = await Mediator.Send(new AddCustomersFromCsv(csvfile));

                    return RedirectToAction("GetAllCustomers");
                }*/

        /*
                [HttpGet("[action]")]
                public async ValueTask<FileResult> GetAllCustomersExcel(string fileName = "CustomersExcel")
                {
                    var result = await Mediator.Send(new GetCustomersExcel { FileName = fileName });

                    return File(result.FileContents, result.Option, result.FileName);
                }*/

        /*       [HttpGet("[action]")]
               public async ValueTask<IActionResult> GetAllCustomersCsv(string fileName = "CustomersCsv")
               {
                   var result = await Mediator.Send(new GetCustomersCsv { FileName = fileName });

                   return File(result.FileContents, result.Option, result.FileName);
               }*/

        /*       [HttpGet("[action]")]
               public async Task<IActionResult> GetAllCustomersPDF(string fileName = "CustomersPDF")
               {
                   var result = await Mediator.Send(new GetCustomerPDF(FileName: fileName));

                   return File(result.FileContents, result.Options, result.FileName);
               }*/
    }
}
