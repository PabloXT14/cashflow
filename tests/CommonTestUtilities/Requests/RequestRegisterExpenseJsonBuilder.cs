using Bogus;
using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestRegisterExpenseJsonBuilder
{
    public static RequestExpenseJson Build()
    {
        var faker = new Faker();

        return new RequestExpenseJson
        {
            Title = faker.Commerce.ProductName(),
            Description = faker.Commerce.ProductDescription(),
            Amount = faker.Finance.Amount(min: 1, max: 1000),
            Date = faker.Date.Past(),
            PaymentType = faker.PickRandom<PaymentType>(),
        };

        // Other syntax
        // return new Faker<RequestRegisterExpenseJson>()
        //     .RuleFor(x => x.Title, f => f.Commerce.ProductName())
        //     .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
        //     .RuleFor(x => x.Amount, f => f.Finance.Amount(min: 1, max: 1000))
        //     .RuleFor(x => x.Date, f => f.Date.Past())
        //     .RuleFor(x => x.PaymentType, f => f.PickRandom<PaymentType>());
    }
}