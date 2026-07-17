using CashFlow.Application.UseCases.Expenses.Register;
using CashFlow.Communication.Enums;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using Shouldly;

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
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("         ")]
    [InlineData(null)]
    public void Error_Title_Empty(string title)
    {
        // Arrange
        var validator = new RegisterExpenseValidator();
        var request = RequestRegisterExpenseJsonBuilder.Build();

        request.Title = title;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.TITLE_REQUIRED))
        );
    }

    [Fact]
    public void Error_Date_Future()
    {
        // Arrange
        var validator = new RegisterExpenseValidator();
        var request = RequestRegisterExpenseJsonBuilder.Build();
        request.Date = DateTime.UtcNow.AddDays(1); // Set the date to tomorrow, which is invalid

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.EXPENSES_CANNOT_FOR_THE_FUTURE))
        );
    }

    [Fact]
    public void Error_Payment_Invalid()
    {
        // Arrange
        var validator = new RegisterExpenseValidator();
        var request = RequestRegisterExpenseJsonBuilder.Build();
        request.PaymentType = (PaymentType)999; // Set an invalid payment type

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.PAYMENT_TYPE_INVALID))
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-7)]
    public void Error_Amount_Invalid(decimal amount)
    {
        // Arrange
        var validator = new RegisterExpenseValidator();
        var request = RequestRegisterExpenseJsonBuilder.Build();
        request.Amount = amount; // Set an invalid amount

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.AMOUNT_MUST_BE_GREATER_THAN_ZERO))
        );
    }
}
