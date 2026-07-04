using CashFlow.Communication.Requests;
using FluentValidation;

namespace CashFlow.Application.UseCases.Expenses.Register;

public class RegisterExpenseValidator : AbstractValidator<RequestRegisterExpenseJson>
{
    public RegisterExpenseValidator()
    {
        RuleFor(expense => expense.Title)
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(expense => expense.Amount)
            .GreaterThan(0).WithMessage("Value must be greater than zero.");

        RuleFor(expense => expense.Date)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future.");

        RuleFor(expense => expense.PaymentType)
            .IsInEnum().WithMessage("Invalid payment type.");
    }
}