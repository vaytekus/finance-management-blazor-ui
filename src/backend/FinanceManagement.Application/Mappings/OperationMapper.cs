using FinanceManagement.Contracts.Operations;
using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Mappings;

public static class OperationMapper
{
    public static OperationResponse ToResponse(this Operation entity) => new()
    {
        Id = entity.Id,
        TypeId = entity.TypeId,
        TypeName = entity.Type!.Name,
        Kind = (Contracts.Enums.OperationKind)entity.Type!.Kind,
        WalletId = entity.WalletId,
        WalletName = entity.Wallet!.Name,
        Amount = entity.Amount,
        Currency = (Contracts.Enums.Currency)entity.Wallet!.Currency,
        Date = entity.Date,
        Note = entity.Note
    };
}
