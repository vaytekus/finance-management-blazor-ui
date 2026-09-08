using FinanceManagement.Application.Common;
using FinanceManagement.Application.Interfaces.Repositories;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Entities;
using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Application.Services;

public class UserProvisioningService(
    IWalletRepository walletRepository,
    IOperationTypeRepository operationTypeRepository) : IUserProvisioningService
{
    private const string _defaultWalletName = "Cash";

    public void AddDefaultsFor(User user)
    {
        var wallet = new Wallet
        {
            Name = _defaultWalletName, Currency = Currency.UAH, UserId = user.Id, CreatedAt = DateTime.UtcNow,
        };

        walletRepository.Add(wallet);

        foreach (var template in DefaultOperationTypes.All)
        {
            operationTypeRepository.Add(template.ToEntity(user.Id));
        }
    }
}
