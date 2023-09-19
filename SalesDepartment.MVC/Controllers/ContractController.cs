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

namespace SalesDepartment.MVC.Controllers;

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

    [HttpGet("[action]")]
    public async ValueTask<IActionResult> ContractInDocx(int id)
    {

        ContractResponse contract = await Mediator.Send(new GetContractByIdQuery(id));
        var templatePath = @"D:\PDP\SalesDepartment\SalesDepartment.MVC\wwwroot\docs\ДОГОВОР.docx";

        string TotalAmountInWords = DecimalToRussianWords(contract.TotalAmountOfContract);
        decimal SumOfOneMeterSquare = contract.TotalAmountOfContract / contract.Home.Area;
        string SumOfOneMeterSquareInWords = DecimalToRussianWords(SumOfOneMeterSquare);

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
        doc.ReplaceText("«Сумма_контракта_прописью»", TotalAmountInWords);
        doc.ReplaceText("«Сумма_1_М2»", SumOfOneMeterSquare.ToString());

        doc.ReplaceText("«Сумма_1_М2_прописью»", SumOfOneMeterSquareInWords);
        doc.ReplaceText("«Тел_Ном_»", contract.Customer.PhoneNumberOne);

        var newFilename = $"Договор_{contract.Customer.LastName + "_" + contract.Customer.FirstName}.docx";

        doc.SaveAs(newFilename);

        var fileBytes = System.IO.File.ReadAllBytes(newFilename);
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", newFilename);
    }

    
    private static readonly string[] RussianUnits =
     {
        "", "тысяча", "миллион", "миллиард", "триллион", "квадриллион", "квинтиллион", "секстиллион", "септиллион", "октиллион", "нониллион", "дециллион"
    };

    private static readonly string[] RussianTeens =
    {
        "", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать"
    };

    private static readonly string[] RussianTens =
    {
        "", "десять", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто"
    };

    private static readonly string[] RussianHundreds =
    {
        "", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот"
    };

    public static string DecimalToRussianWords(decimal number)
    {
        string integralWords = ConvertIntegralPartToRussianWords(Math.Floor(number));
        string fractionalWords = ConvertFractionalPartToRussianWords(number - Math.Floor(number));

        string result = integralWords;

        if (!string.IsNullOrEmpty(fractionalWords))
        {
            result += " целых " + fractionalWords;
        }

        return char.ToUpper(result[0]) + result.Substring(1);
    }

    private static string ConvertIntegralPartToRussianWords(decimal integralPart)
    {
        if (integralPart == 0)
        {
            return "ноль";
        }

        string words = "";
        int unitIndex = 0;

        while (integralPart > 0)
        {
            decimal threeDigits = integralPart % 1000;
            integralPart /= 1000;

            if (threeDigits > 0)
            {
                string threeDigitsWords = ConvertThreeDigitsToRussianWords(threeDigits);
                words = threeDigitsWords + " " + RussianUnits[unitIndex] + " " + words;
            }

            unitIndex++;
        }

        return words.Trim();
    }

    private static string ConvertThreeDigitsToRussianWords(decimal threeDigits)
    {
        int hundreds = (int)(threeDigits / 100);
        int tens = (int)((threeDigits % 100) / 10);
        int ones = (int)(threeDigits % 10);

        string words = "";

        if (hundreds > 0)
        {
            words += RussianHundreds[hundreds] + " ";
        }

        if (tens > 1)
        {
            words += RussianTens[tens] + " ";
            if (ones > 0)
            {
                words += RussianUnits[ones] + " ";
            }
        }
        else if (tens == 1)
        {
            int teenIndex = tens * 10 + ones;
            words += RussianTeens[teenIndex] + " ";
        }
        else if (ones > 0)
        {
            words += RussianUnits[ones] + " ";
        }

        return words.Trim();
    }

    private static string ConvertFractionalPartToRussianWords(decimal fractionalPart)
    {
        const int maxFractionalDigits = 2; // Assuming you want to handle up to two fractional digits

        // Convert the fractional part to words, assuming it's a reasonable fraction
        string words = "";
        decimal fraction = fractionalPart;

        for (int i = 0; i < maxFractionalDigits; i++)
        {
            fraction *= 10;
            int digit = (int)Math.Floor(fraction);

            if (digit > 0)
            {
                words += ConvertThreeDigitsToRussianWords(digit) + " ";
            }

            fraction -= digit;
        }

        return words.Trim();
    }
}

