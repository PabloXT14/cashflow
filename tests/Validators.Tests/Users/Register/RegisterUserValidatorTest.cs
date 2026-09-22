using CashFlow.Application.UseCases.User.Register;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using Shouldly;

namespace Validators.Tests.Users.Register;

public class RegisterUserValidatorTest
{
    [Fact]
    public void Success()
    {
        // Arrange
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("         ")]
    [InlineData(null)]
    public void Error_Name_Empty(string name)
    {
        // Arrange
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Name = name;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.NAME_EMPTY))
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("         ")]
    [InlineData(null)]
    public void Error_Email_Empty(string email)
    {
        // Arrange
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Email = email;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.EMAIL_EMPTY))
        );
    }
}
