using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.Expenses.Register;

public class RegisterExpenseUseCase
{
    public ResponseRegisteredExpenseJson Execute(RequestRegisteredExpenseJson request)
    {
        Validate(request);
        
        return new ResponseRegisteredExpenseJson();
    }

    private void Validate(RequestRegisteredExpenseJson request)
    {
        var isTitleEmpty = string.IsNullOrWhiteSpace(request.Title);
        if (isTitleEmpty)
        {
            throw new ArgumentException("Title is required.");
        }
     
        var valueIsZeroOrNegative = request.Amount <= 0;
        if (valueIsZeroOrNegative)
        {
            throw new ArgumentException("Value must be greater than zero.");
        }

        var isDateOnFuture = DateTime.Compare(request.Date, DateTime.UtcNow) > 0;
        if (isDateOnFuture)
        {
            throw new ArgumentException("Date cannot be in the future.");
        }

        var isPaymentTypeValid = Enum.IsDefined(typeof(PaymentType), request.PaymentType);
        if (!isPaymentTypeValid)
        {
            throw new ArgumentException("Invalid payment type.");
        }
    }
}