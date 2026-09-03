using FinanceManagement.Application.Common.Pagination;
using FluentValidation;

namespace FinanceManagement.Application.Features.Wallets.Queries.GetAllWallets;

public class GetAllWalletsQueryValidator : PagedQueryValidator<GetAllWalletsQuery>
{
    private static readonly string[] _allowedSortFields = ["name", "createdat", "currency"];

    public GetAllWalletsQueryValidator() : base(_allowedSortFields)
    {
        
    }
}