using CashFlow.Application.UseCases.User.Register;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using Shouldly;

namespace CashFlow.UseCases.Tests.Users.Register;

public class RegisterUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        // Arrange
        var useCase = CreateUseCase();
        var request = RequestRegisterUserJsonBuilder.Build();

        // Act
        var response = await useCase.Execute(request);

        // Assert
        response.ShouldNotBeNull();
        response.Name.ShouldBe(request.Name);
        response.Token.ShouldNotBeNullOrWhiteSpace();
    }



    private RegisterUserUseCase CreateUseCase()
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var userWriteOnlyRepository = UserWriteOnlyRepositoryBuilder.Build();
        var passwordEncripter = PasswordEncripterBuilder.Build();
        var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();

        return new RegisterUserUseCase(
            mapper,
            passwordEncripter,
            null,
            userWriteOnlyRepository,
            unitOfWork,
            accessTokenGenerator
        );
    }
}