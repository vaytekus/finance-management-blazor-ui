using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using MediatR;

namespace FinanceManagement.Application.Features.Operations.Commands.DeleteOperation;

public class DeleteOperationCommandHandler : IRequestHandler<DeleteOperationCommand>
{
    private readonly IOperationRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOperationCommandHandler(
        IOperationRepository repository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteOperationCommand request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdForUserAsync(request.Id, _currentUser.Id, ct)
            .OrThrowAsync(request.Id);

        _repository.SoftDelete(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
