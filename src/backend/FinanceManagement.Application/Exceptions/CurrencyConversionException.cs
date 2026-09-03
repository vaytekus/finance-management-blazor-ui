namespace FinanceManagement.Application.Exceptions;

public class CurrencyConversionException : Exception
{
    public  CurrencyConversionException(string error) : base(error) { }
    public  CurrencyConversionException(string error, Exception inner) : base(error, inner) { }
}
