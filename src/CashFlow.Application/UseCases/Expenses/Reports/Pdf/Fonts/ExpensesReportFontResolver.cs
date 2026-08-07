using System.Reflection;
using PdfSharp.Fonts;

namespace CashFlow.Application.UseCases.Expenses.Reports.Pdf.Fonts;

public class ExpensesReportFontResolver : IFontResolver
{
    public byte[]? GetFont(string faceName)
    {
        var fontStream = ReadFontFile(faceName);
        fontStream ??= ReadFontFile(FontHelper.DEFAULT_FONT); // just execute this line if the fontStream is null

        var length = (int)fontStream!.Length;

        var fontData = new byte[length];

        // offset = 0 because we want to read the entire font file from the beginning from the array of bytes
        fontStream.Read(buffer: fontData, offset: 0, count: length);

        return fontData;
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
    {
        return new FontResolverInfo(familyName);
    }

    private Stream? ReadFontFile(string faceName)
    {
        var assembly = Assembly.GetExecutingAssembly(); // Get the reference to the Assembly DLL of the current project (CashFlow.Application.dll) where the font files are embedded as resources

        return assembly.GetManifestResourceStream($"CashFlow.Application.UseCases.Expenses.Reports.Pdf.Fonts.{faceName}.ttf");
    }
}