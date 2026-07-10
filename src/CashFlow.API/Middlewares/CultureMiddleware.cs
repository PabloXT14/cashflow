using System.Globalization;

namespace CashFlow.API.Middlewares;

public class CultureMiddleware
{
    private readonly RequestDelegate _next;

    public CultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var culture = context.Request.Headers.AcceptLanguage.FirstOrDefault();

        var cultureInfo = new CultureInfo("en"); // Default culture/language

        if (!string.IsNullOrWhiteSpace(culture))
        {
            cultureInfo = new CultureInfo(culture);
        }

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo; // Set the current UI culture

        await _next(context); // Continue processing the request
    }
}