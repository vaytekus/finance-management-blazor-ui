using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Application.Services;

public class UserProvisioningService : IUserProvisioningService
{
    private const string _defaultWalletName = "Cash";

    private readonly IWalletRepository _walletRepository;
    private readonly IOperationTypeRepository _operationTypeRepository;

    public UserProvisioningService(
        IWalletRepository walletRepository,
        IOperationTypeRepository operationTypeRepository)
    {
        _walletRepository = walletRepository;
        _operationTypeRepository = operationTypeRepository;
    }

    public void AddDefaultsFor(User user)
    {
        var wallet = new Wallet
        {
            Name = _defaultWalletName, Currency = Currency.UAH, UserId = user.Id, CreatedAt = DateTime.UtcNow,
        };

        _walletRepository.Add(wallet);

        foreach (var template in DefaultOperationTypes.All)
        {
            _operationTypeRepository.Add(template.ToEntity(user.Id));
        }
    }
}
