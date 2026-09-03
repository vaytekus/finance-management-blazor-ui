namespace FinanceManagement.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string msg) : base(msg) { }
    
    public NotFoundException(string entityName, object key) : base($"{entityName} with id '{key}' was not found.") { }
}
