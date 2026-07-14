using CashFlow.Application.UseCases.Expenses.Register;
using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;

namespace Validators.Tests.Expenses.Register;

public class RegisterExpenseValidatorTests
{
    [Fact]
    public void Success()
    {
        // Arrange (config the instances that we need to execute our test)
        var validator = new RegisterExpenseValidator();

        var request = RequestRegisterExpenseJsonBuilder.Build();

        // Act (execute the method that we want to test)
        var result = validator.Validate(request);

        // Assert (verify that the result is what we expected)
        Assert.True(result.IsValid);
    }
}
