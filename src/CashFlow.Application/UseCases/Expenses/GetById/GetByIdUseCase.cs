using AutoMapper;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Expenses;

namespace CashFlow.Application.UseCases.Expenses.GetById;

public class GetByIdUseCase : IGetByIdUseCase
{
    private readonly IExpensesRepository _expenseRepository;
    private readonly IMapper _mapper;

    public GetByIdUseCase(IExpensesRepository expenseRepository, IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _mapper = mapper;
    }

    public async Task<ResponseExpenseJson> Execute(long id)
    {
        var expense = await _expenseRepository.GetById(id);

        return _mapper.Map<ResponseExpenseJson>(expense);
    }
}