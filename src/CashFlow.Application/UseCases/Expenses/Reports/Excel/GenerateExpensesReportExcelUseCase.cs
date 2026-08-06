using CashFlow.Domain.Enums;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using ClosedXML.Excel;

namespace CashFlow.Application.UseCases.Expenses.Reports.Excel;

public class GenerateExpensesReportExcelUseCase : IGenerateExpensesReportExcelUseCase
{
    private const string CURRENCY_SYMBOL = "R$";
    private readonly IExpensesReadOnlyRepository _expensesRepository;

    public GenerateExpensesReportExcelUseCase(IExpensesReadOnlyRepository expensesRepository)
    {
        _expensesRepository = expensesRepository;
    }

    public async Task<byte[]> Execute(DateOnly month)
    {
        var expenses = await _expensesRepository.FilterByMonth(month);

        if (expenses.Count == 0)
        {
            return [];
        }

        var workbook = new XLWorkbook();

        workbook.Author = "Pablo Alan";
        workbook.Style.Font.FontSize = 12;
        workbook.Style.Font.FontName = "Roboto";

        var period = month.ToString("Y"); // "MMMM yyyy" format

        var worksheet = workbook.Worksheets.Add(period);

        InsertHeader(worksheet);

        var row = 2; // +2 because the first row is the header and the index starts at 0
        foreach (var expense in expenses)
        {
            worksheet.Cell($"A{row}").Value = expense.Title;
            worksheet.Cell($"B{row}").Value = expense.Date.ToString("dd/MM/yyyy");
            worksheet.Cell($"C{row}").Value = ConvertPaymentType(expense.PaymentType);

            worksheet.Cell($"D{row}").Value = expense.Amount;
            worksheet.Cell($"D{row}").Style.NumberFormat.Format = $"-{CURRENCY_SYMBOL} #,##0.00";

            worksheet.Cell($"E{row}").Value = expense.Description;


            worksheet.Cell($"A{row}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell($"B{row}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell($"C{row}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell($"D{row}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            worksheet.Cell($"E{row}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            row++;
        }

        worksheet.Columns().AdjustToContents();

        var file = new MemoryStream();

        workbook.SaveAs(file);

        return file.ToArray();
    }

    private string ConvertPaymentType(PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Cash => ResourceReportGenerationMessages.CASH,
            PaymentType.DebitCard => ResourceReportGenerationMessages.DEBIT_CARD,
            PaymentType.CreditCard => ResourceReportGenerationMessages.CREDIT_CARD,
            PaymentType.ElectronicTransfer => ResourceReportGenerationMessages.ELECTRONIC_TRANSFER,
            _ => string.Empty
        };
    }

    private void InsertHeader(IXLWorksheet worksheet)
    {
        worksheet.Cell("A1").Value = ResourceReportGenerationMessages.TITLE;
        worksheet.Cell("B1").Value = ResourceReportGenerationMessages.DATE;
        worksheet.Cell("C1").Value = ResourceReportGenerationMessages.PAYMENT_TYPE;
        worksheet.Cell("D1").Value = ResourceReportGenerationMessages.AMOUNT;
        worksheet.Cell("E1").Value = ResourceReportGenerationMessages.DESCRIPTION;

        worksheet.Range("A1:E1").Style.Font.SetBold();
        worksheet.Range("A1:E1").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#f5c2b6"));

        worksheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("B1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell("D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        worksheet.Cell("E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
    }
}