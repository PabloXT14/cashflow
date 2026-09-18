using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Tokens;

public interface IAccessTokenGenerator
{
    string Generate(User user);
}