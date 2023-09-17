using Microsoft.AspNetCore.Mvc;
using SalesDepartment.Application.UseCases.Contracts.Commands.CreateContract;
using SalesDepartment.Application.UseCases.Contracts.Commands.DeleteContract;
using SalesDepartment.Application.UseCases.Contracts.Commands.UpdateContract;
using SalesDepartment.Application.UseCases.Contracts.Queries.GetAllContracts;
using SalesDepartment.Application.UseCases.Contracts.Queries.GetContractById;
using SalesDepartment.Application.UseCases.Contracts.Reports;
using SalesDepartment.Application.UseCases.Contracts.Response;
using SalesDepartment.Application.UseCases.Customers.Queries.GetAllCustomers;
using SalesDepartment.Application.UseCases.Customers.Response;
using SalesDepartment.Application.UseCases.Founders.Queries.GetAllFounders;
using SalesDepartment.Application.UseCases.Founders.Response;
using SalesDepartment.Application.UseCases.Homes.Queries.GetAllHomes;
using SalesDepartment.Application.UseCases.Homes.Response;
using Xceed.Words.NET;

namespace SalesDepartment.MVC.Controllers
{
    public class ContractController : ApiBaseController
    {
        [HttpGet("[action]")]
        public async ValueTask<IActionResult> CreateContract()
        {
            FounderResponse[] founders = await Mediator.Send(new GetAllFoundersQuery());
            ViewData["Founders"] = founders;

            CustomerResponse[] customers = await Mediator.Send(new GetAllCustomersQuery());
            ViewData["Customers"] = customers;

            HomeResponse[] homes = await Mediator.Send(new GetAllHomesQuery());
            ViewData["Homes"] = homes;

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
        public async ValueTask<FileResult> GetAllContractsExcel(string fileName = "ContractExcel")
        {
            var result = await Mediator.Send(new GetContractsExcel { FileName = fileName });

            return File(result.FileContents, result.Option, result.FileName);
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

        [HttpGet("{id}")]
        public async ValueTask<IActionResult> ContractInDocx(int id)
        {

            ContractResponse contract = await Mediator.Send(new GetContractByIdQuery(id));
            var templatePath = @"D:\PDP\SalesDepartment\SalesDepartment.MVC\wwwroot\docs\ДОГОВОР.docx";
            
            string TotalAmountInWords = ConvertToWords(decimal.Parse(contract.TotalAmountOfContract.ToString()));
            double SumOfOneMeterSquare = contract.TotalAmountOfContract / contract.Home.Area;
            string SumOfOneMeterSquareInWords= ConvertToWords(decimal.Parse(SumOfOneMeterSquare.ToString()));

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
            doc.ReplaceText("«Общая_сумма_контракта» ", contract.TotalAmountOfContract.ToString());
            doc.ReplaceText("«Сумма_контракта_прописью»", TotalAmountInWords);
            doc.ReplaceText("«Сумма_1_М2» ", SumOfOneMeterSquare.ToString());

            doc.ReplaceText("«Сумма_1_М2_прописью»", SumOfOneMeterSquareInWords);
            doc.ReplaceText("«Тел_Ном_»", contract.Customer.PhoneNumberOne);

            var newFilename = $"Договор_{contract.Customer.LastName+ "_"+contract.Customer.FirstName}.docx";

            doc.SaveAs(newFilename);

            var fileBytes = System.IO.File.ReadAllBytes(newFilename);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", newFilename);
        }

        static string[] units = { "ноль", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
        static string[] teens = { "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" };
        static string[] tens = { "", "", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };
        static string[] hundreds = { "", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" };
        static string[] thousands = { "", "тысяча", "тысячи", "тысяч" };
        static string[] millions = { "", "миллион", "миллиона", "миллионов" };
        static string[] billions = { "", "миллиард", "миллиарда", "миллиардов" };
        static string[] trillions = { "", "триллион", "триллиона", "триллионов" };

        static string ConvertToWords(decimal number)
        {
            if (number == 0)
                return units[0];

            int wholePart = (int)Math.Floor(number);
            int fractionPart = (int)((number - wholePart) * 100);

            string words = ConvertWholeToWords(wholePart);
            if (fractionPart > 0)
            {
                words += " и " + ConvertToWords(fractionPart);
            }

            return words;
        }

        static string ConvertWholeToWords(long number)
        {
            if (number < 10)
                return units[number];

            if (number < 20)
                return teens[number - 10];

            if (number < 100)
                return tens[number / 10] + (number % 10 > 0 ? " " + units[number % 10] : "");

            if (number < 1000)
                return hundreds[number / 100] + (number % 100 > 0 ? " " + ConvertWholeToWords(number % 100) : "");

            if (number < 1000000)
            {
                return ConvertWholeToWords(number / 1000) + " " +
                    thousands[(number / 1000) % 10] + " " +
                    ConvertWholeToWords(number % 1000);
            }

            if (number < 1000000000)
            {
                return ConvertWholeToWords(number / 1000000) + " " +
                    millions[(number / 1000000) % 10] + " " +
                    ConvertWholeToWords(number % 1000000);
            }

            return ConvertWholeToWords(number / 1000000000) + " " +
                billions[(number / 1000000000) % 10] + " " +
                ConvertWholeToWords(number % 1000000000);
        }
    }
}
