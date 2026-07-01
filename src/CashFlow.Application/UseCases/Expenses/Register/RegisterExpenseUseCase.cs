using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.Expenses.Register;

public class RegisterExpenseUseCase
{
    public ResponseRegisteredExpenseJson Execute(RequestRegisteredExpenseJson request)
    {
        // TODO: Validations
        
        return new ResponseRegisteredExpenseJson();
    }
}