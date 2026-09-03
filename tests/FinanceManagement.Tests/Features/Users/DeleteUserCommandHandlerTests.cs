using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Features.Users.Commands.DeleteUser;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace FinanceManagement.Tests.Features.Users;

public class DeleteUserCommandHandlerTests
{
    private readonly IUserRepository _repository = Substitute.For<IUserRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly DeleteUserCommandHandler _sut;

    public DeleteUserCommandHandlerTests()
    {
        _currentUser.Id.Returns(_currentUserId);
        _sut = new DeleteUserCommandHandler(_repository, _currentUser, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenDeletingSelf_ThrowsValidation()
    {
        var command = new DeleteUserCommand(_currentUserId);

        var act = async () => await _sut.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*cannot delete your own account*");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenLastAdmin_ThrowsValidation()
    {
        var target = NewUser(UserRole.Admin);
        _repository.GetByIdAsync(target.Id, Arg.Any<CancellationToken>()).Returns(target);
        _repository.AnyOtherAdminAsync(target.Id, Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await _sut.Handle(new DeleteUserCommand(target.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*last admin*");
        _repository.DidNotReceive().Remove(Arg.Any<User>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRegularUser_RemovesAndSaves()
    {
        var target = NewUser(UserRole.User);
        _repository.GetByIdAsync(target.Id, Arg.Any<CancellationToken>()).Returns(target);

        await _sut.Handle(new DeleteUserCommand(target.Id), CancellationToken.None);

        await _repository.Received(1).DeleteOperationsAsync(target.Id, Arg.Any<CancellationToken>());
        _repository.Received(1).Remove(target);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static User NewUser(UserRole roleId) => new()
    {
        Id = Guid.NewGuid(),
        UserName = "u",
        Email = "u@x.com",
        PasswordHash = "hash",
        RoleId = roleId,
        CreatedAt = DateTime.UtcNow
    };
}