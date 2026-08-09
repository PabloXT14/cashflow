using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Fonts;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;

namespace CashFlow.Application.UseCases.Expenses.Reports.Pdf;

public class GenerateExpensesReportPdfUseCase : IGenerateExpensesReportPdfUseCase
{
    private const string CURRENCY_SYMBOL = "R$";
    private readonly IExpensesReadOnlyRepository _expensesRepository;

    public GenerateExpensesReportPdfUseCase(IExpensesReadOnlyRepository expensesRepository)
    {
        _expensesRepository = expensesRepository;

        GlobalFontSettings.FontResolver = new ExpensesReportFontResolver();
    }

    public async Task<byte[]> Execute(DateOnly month)
    {
        var expenses = await _expensesRepository.FilterByMonth(month);

        if (expenses.Count == 0)
        {
            return [];
        }

        var document = CreateDocument(month);
        var page = CreatePage(document);

        var paragraph = page.AddParagraph();

        // string.Format replace a character in a string with another string, in this case, it replaces {0} with the month format. But you can add as many parameters as you want, and it will replace {1}, {2}, etc. with the corresponding parameter.
        var title = string.Format(
            ResourceReportGenerationMessages.TOTAL_SPENT_IN,
            month.ToString("Y")
        );

        paragraph.AddFormattedText(title, new Font
        {
            Name = FontHelper.RALEWAY_REGULAR,
            Size = 15,
        });

        paragraph.AddLineBreak();


        var totalExpenses = expenses.Sum(expense => expense.Amount);
        paragraph.AddFormattedText($"{CURRENCY_SYMBOL} {totalExpenses:F2}", new Font
        {
            Name = FontHelper.WORKSANS_BLACK,
            Size = 50,
        });

        return RenderDocument(document);
    }

    private Document CreateDocument(DateOnly month)
    {
        var document = new Document();

        document.Info.Title = $"{ResourceReportGenerationMessages.EXPENSES_FOR} {month:Y}";
        document.Info.Author = "Pablo Alan";

        var styles = document.Styles["Normal"];
        styles!.Font.Name = FontHelper.RALEWAY_REGULAR;

        return document;
    }

    private Section CreatePage(Document document)
    {
        var section = document.AddSection();

        section.PageSetup = document.DefaultPageSetup.Clone();

        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.TopMargin = 80;
        section.PageSetup.BottomMargin = 80;
        section.PageSetup.LeftMargin = 40;
        section.PageSetup.RightMargin = 40;

        return section;
    }

    private byte[] RenderDocument(Document document)
    {
        var renderer = new PdfDocumentRenderer
        {
            Document = document,
        };

        renderer.RenderDocument();

        using var fileStream = new MemoryStream();

        renderer.PdfDocument.Save(fileStream);

        return fileStream.ToArray();
    }
}