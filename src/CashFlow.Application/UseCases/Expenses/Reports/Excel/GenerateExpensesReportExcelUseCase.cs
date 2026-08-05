using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using ClosedXML.Excel;

namespace CashFlow.Application.UseCases.Expenses.Reports.Excel;

public class GenerateExpensesReportExcelUseCase : IGenerateExpensesReportExcelUseCase
{
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

        var file = new MemoryStream();

        workbook.SaveAs(file);

        return file.ToArray();
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