using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Interfaces.Services;

public interface IUserProvisioningService
{
    void AddDefaultsFor(User user);
}
