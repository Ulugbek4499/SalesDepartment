using Microsoft.AspNetCore.Mvc;
using SalesDepartment.Application.UseCases.Contracts.Queries.GetAllContracts;
using SalesDepartment.Application.UseCases.Contracts.Queries.GetContractById;
using SalesDepartment.Application.UseCases.Contracts.Response;
using SalesDepartment.Application.UseCases.Payments.Commands.CreatePayment;
using SalesDepartment.Application.UseCases.Payments.Commands.DeletePayment;
using SalesDepartment.Application.UseCases.Payments.Commands.UpdatePayment;
using SalesDepartment.Application.UseCases.Payments.Queries.GetAllPayments;
using SalesDepartment.Application.UseCases.Payments.Queries.GetPaymentById;
using SalesDepartment.Application.UseCases.Payments.Reports;
using SalesDepartment.Application.UseCases.Payments.Response;
using SalesDepartment.Application.UseCases.PaymentTypes.Queries.GetAllPaymentTypes;
using SalesDepartment.Application.UseCases.PaymentTypes.Response;
using Xceed.Words.NET;

namespace SalesDepartment.MVC.Controllers;

public class PaymentController : ApiBaseController
{
    [HttpGet("[action]")]
    public async ValueTask<IActionResult> CreatePayment()
    {
        PaymentTypeResponse[] paymentTypes = await Mediator.Send(new GetAllPaymentTypesQuery());
        ViewData["PaymentTypes"] = paymentTypes;

        ContractResponse[] contracts = await Mediator.Send(new GetAllContractsQuery());
        ViewData["Contracts"] = contracts;

        return View();
    }

    [HttpPost("[action]")]
    public async ValueTask<IActionResult> CreatePayment([FromForm] CreatePaymentCommand Payment)
    {
        await Mediator.Send(Payment);

        return RedirectToAction("GetAllPayments");
    }

    [HttpGet("[action]")]
    public async ValueTask<IActionResult> CreatePaymentFromExcel()
    {
        return View();
    }

    [HttpGet("[action]")]
    public async ValueTask<IActionResult> GetAllPayments()
    {
        var Payments = await Mediator.Send(new GetAllPaymentsQuery());

        return View(Payments);
    }

    [HttpGet("[action]")]
    public async ValueTask<FileResult> GetAllPaymentsExcel(string fileName = "PaymentsExcel")
    {
        var result = await Mediator.Send(new GetPaymentsExcel { FileName = fileName });

        return File(result.FileContents, result.Option, result.FileName);
    }

    [HttpGet("[action]")]
    public async ValueTask<IActionResult> UpdatePayment(int Id)
    {
        var Payment = await Mediator.Send(new GetPaymentByIdQuery(Id));

        return View(Payment);
    }

    [HttpPost("[action]")]
    public async ValueTask<IActionResult> UpdatePayment([FromForm] UpdatePaymentCommand Payment)
    {
        await Mediator.Send(Payment);
        return RedirectToAction("GetAllPayments");
    }

    public async ValueTask<IActionResult> DeletePayment(int Id)
    {
        await Mediator.Send(new DeletePaymentCommand(Id));

        return RedirectToAction("GetAllPayments");
    }

    [HttpGet("[action]")]
    public async ValueTask<IActionResult> ViewPayment(int id)
    {
        var Payment = await Mediator.Send(new GetPaymentByIdQuery(id));

        return View("ViewPayment", Payment);
    }


    [HttpGet("[action]")]
    public async ValueTask<IActionResult> PaymentInDocx(int id)
    {

        PaymentResponse payment = await Mediator.Send(new GetPaymentByIdQuery(id));
        var templatePath = @"D:\PDP\SalesDepartment\SalesDepartment.MVC\wwwroot\docs\Kvitansiya.docx";

        var doc = DocX.Load(templatePath);

        doc.ReplaceText("«Договар_»", payment.Contract.ContractNumber);
        doc.ReplaceText("«Договар_дата»", payment.Contract.ContractStartDate.ToString());
        doc.ReplaceText("«Принято_от_ФИО»", payment.Contract.Customer.LastName + " " + payment.Contract.Customer.FirstName + " " + payment.Contract.Customer.MiddleName);
        doc.ReplaceText("«Сумма»", payment.Amount.ToString());
        doc.ReplaceText("«Summa_soz_bilan»", payment.AmountsInWords);

        doc.ReplaceText("«Номер_документа_»", payment.PaymentNumber);
        doc.ReplaceText("«Дата_составления»", payment.PaymentDate.ToString());

        var newFilename = $"Квитансия_{payment.Contract.Customer.LastName + "_" + payment.Contract.Customer.FirstName + "_" + payment.PaymentNumber}.docx";

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