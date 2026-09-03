using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Features.Wallets.Commands.CreateWallet;
using FinanceManagement.Application.Interfaces;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;
using ContractsCurrency = FinanceManagement.Contracts.Enums.Currency;
using FluentAssertions;
using NSubstitute;

namespace FinanceManagement.Tests.Features.Wallets;

public class CreateWalletCommandHandlerTests
{
    private readonly IWalletRepository _repository = Substitute.For<IWalletRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly CreateWalletCommandHandler _sut;

    public CreateWalletCommandHandlerTests()
    {
        _currentUser.Id.Returns(_currentUserId);
        _sut = new CreateWalletCommandHandler(_repository, _currentUser, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenNameAlreadyExists_ThrowsConflict()
    {
        var command = new CreateWalletCommand("Main", Currency.UAH);
        _repository.ExistsByNameForUserAsync("Main", _currentUserId, null, Arg.Any<CancellationToken>())
            .Returns(true);

        var act = async () => await _sut.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*Main*already exists*");
        _repository.DidNotReceive().Add(Arg.Any<Wallet>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNameUnique_AddsAndSaves()
    {
        var command = new CreateWalletCommand("Main", Currency.USD);
        _repository.ExistsByNameForUserAsync("Main", _currentUserId, null, Arg.Any<CancellationToken>())
            .Returns(false);

        var response = await _sut.Handle(command, CancellationToken.None);

        response.Name.Should().Be("Main");
        response.Currency.Should().Be(ContractsCurrency.USD);
        _repository.Received(1).Add(Arg.Is<Wallet>(w =>
            w.Name == "Main" && w.Currency == Currency.USD && w.UserId == _currentUserId));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}