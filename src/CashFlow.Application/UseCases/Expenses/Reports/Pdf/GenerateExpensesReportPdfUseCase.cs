using System.Reflection;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Colors;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Fonts;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
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

        CreateHeaderWithProfilePhotoAndName(page);

        var totalExpenses = expenses.Sum(expense => expense.Amount);
        CreateTotalSpentSection(page, month, totalExpenses);

        foreach (var expense in expenses)
        {
            var table = CreateExpenseTable(page);

            var row = table.AddRow();
            row.Height = 25;

            // TITLE CELL
            row.Cells[0].AddParagraph(expense.Title);
            row.Cells[0].Format.Font = new Font
            {
                Name = FontHelper.RALEWAY_BLACK,
                Size = 14,
                Color = ColorsHelper.BLACK
            };
            row.Cells[0].Shading.Color = ColorsHelper.RED_LIGHT;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            row.Cells[0].MergeRight = 2; // Merge the first cell with the next two cells
            row.Cells[0].Format.LeftIndent = 20; // Add left indent to the merged cell

            // AMOUNT CELL
            row.Cells[3].AddParagraph(ResourceReportGenerationMessages.AMOUNT);
            row.Cells[3].Format.Font = new Font
            {
                Name = FontHelper.RALEWAY_BLACK,
                Size = 14,
                Color = ColorsHelper.WHITE
            };
            row.Cells[3].Shading.Color = ColorsHelper.RED_DARK;
            row.Cells[3].VerticalAlignment = VerticalAlignment.Center;

            // BOTTOM SPACING
            var bottomSpacingRow = table.AddRow();
            bottomSpacingRow.Height = 30;
            bottomSpacingRow.Borders.Visible = false;
        }

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

    private void CreateHeaderWithProfilePhotoAndName(Section page)
    {
        var table = page.AddTable();
        // Add 2 columns
        table.AddColumn();
        table.AddColumn("300"); // Set the width of the second column to 300 pixels

        var row = table.AddRow();

        var assembly = Assembly.GetExecutingAssembly();
        var directoryName = Path.GetDirectoryName(assembly.Location);
        var filePath = Path.Combine(directoryName!, "Assets", "profile.png");

        row.Cells[0].AddImage(filePath).Width = 62;

        row.Cells[1].AddParagraph("Hey, Pablo Alan");
        row.Cells[1].Format.Font = new Font
        {
            Name = FontHelper.RALEWAY_BLACK,
            Size = 16,
        };
        row.Cells[1].VerticalAlignment = VerticalAlignment.Center;
    }

    private void CreateTotalSpentSection(Section page, DateOnly month, decimal totalExpenses)
    {
        var paragraph = page.AddParagraph();
        paragraph.Format.SpaceBefore = "40";
        paragraph.Format.SpaceAfter = "40";

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

        paragraph.AddFormattedText($"{CURRENCY_SYMBOL} {totalExpenses:F2}", new Font
        {
            Name = FontHelper.WORKSANS_BLACK,
            Size = 50,
        });
    }

    private Table CreateExpenseTable(Section page)
    {
        var table = page.AddTable();

        // ADD COLUMNS
        table.AddColumn("195").Format.Alignment = ParagraphAlignment.Left;
        table.AddColumn("80").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("120").Format.Alignment = ParagraphAlignment.Center;
        table.AddColumn("120").Format.Alignment = ParagraphAlignment.Right;

        return table;
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