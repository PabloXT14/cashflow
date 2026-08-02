using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.Delete;

public class DeleteExpenseUseCase : IDeleteExpenseUseCase
{
    private readonly IExpensesWriteOnlyRepository _expenseRepository;
    private readonly IUnitOfWork unitOfWork;

    public DeleteExpenseUseCase(IExpensesWriteOnlyRepository expenseRepository, IUnitOfWork unitOfWork)
    {
        _expenseRepository = expenseRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task Execute(long id)
    {
        var result = await _expenseRepository.Delete(id);

        if (result == false)
        {
            throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
        }

        await unitOfWork.Commit();
    }
}