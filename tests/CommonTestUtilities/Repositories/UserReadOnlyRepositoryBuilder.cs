using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.User;
using Moq;

namespace CommonTestUtilities.Repositories;

public class UserReadOnlyRepositoryBuilder
{
    private readonly Mock<IUserReadOnlyRepository> _repository;

    public UserReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IUserReadOnlyRepository>();
    }

    public void ExistsActiveUserWithEmail(string email)
    {
        // Just return true if the email is not null or whitespace, otherwise return false, other wise will can also use a fixed email to only return true if the email is the same as the fixed one
        _repository
            .Setup(userReadOnly => userReadOnly.ExistsActiveUserWithEmail(email))
            .ReturnsAsync(true);
    }

    public UserReadOnlyRepositoryBuilder GetUserByEmail(User user)
    {
        _repository
            .Setup(userReadOnly => userReadOnly.GetUserByEmail(user.Email))
            .ReturnsAsync(user);

        return this;
    }

    public IUserReadOnlyRepository Build()
    {
        return _repository.Object;
    }
}