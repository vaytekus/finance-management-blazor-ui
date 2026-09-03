namespace FinanceManagement.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string msg) : base(msg) { }
}
