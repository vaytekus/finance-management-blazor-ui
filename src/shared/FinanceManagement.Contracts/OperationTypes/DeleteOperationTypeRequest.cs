namespace FinanceManagement.Contracts.OperationTypes;

public record DeleteOperationTypeRequest(Guid? ReplaceWithId, string? ReplaceWithName);
