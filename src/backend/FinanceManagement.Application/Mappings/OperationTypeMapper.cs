using FinanceManagement.Contracts.OperationTypes;
using FinanceManagement.Domain.Entities;

namespace FinanceManagement.Application.Mappings;

public static class OperationTypeMapper
{
    public static OperationTypeResponse ToResponse(this OperationType entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Kind = (Contracts.Enums.OperationKind)entity.Kind
    };
}
