using FluentValidation;
using GymApplication.Shared.BusinessObject.CheckLogs.Response;
using GymApplication.Shared.Common;
using GymApplication.Shared.Emuns;
using MediatR;

namespace GymApplication.Shared.BusinessObject.CheckLogs.Request;

public sealed class CreateCheckLogRequestValidation : AbstractValidator<CreateCheckLogsRequest>
{
    public CreateCheckLogRequestValidation()
    {
        RuleFor(u => u.UserId)
            .NotNull().NotEmpty();

        RuleFor(u => u.UserSubscriptionId)
            .NotNull().NotEmpty();
        
        RuleFor(u => u.CheckStatus)
            .NotNull().NotEmpty()
            .Must(BeValidCheckStatus)
            .WithMessage("CheckStatus must be either 'CheckIn', 'CheckOut', or their numeric equivalents (1 for 'CheckIn', 2 for 'CheckOut').");
        
    }
    
    private bool BeValidCheckStatus(string? checkStatus)
    {
        if (string.IsNullOrEmpty(checkStatus)) return false;

        // Check if it's a valid string enum value
        if (Enum.TryParse(typeof(LogsStatus), checkStatus, out var _)) return true;

        // Check if it's a valid numeric representation
        if (int.TryParse(checkStatus, out var statusNumber))
        {
            return Enum.IsDefined(typeof(LogsStatus), statusNumber);
        }

        return false;
    }
    
}

public sealed class CreateCheckLogsRequest : IRequest<Result<CheckLogsResponse>>
{
    public Guid UserId { get; set; }
    public Guid? CheckInId { get; set; }
    public Guid UserSubscriptionId { get; set; }
    public string? CheckStatus { get; set; }
}
