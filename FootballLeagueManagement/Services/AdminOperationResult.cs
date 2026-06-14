namespace FootballLeagueManagement.Services;

public enum AdminOperationStatus
{
    Success,
    NotFound,
    BadRequest,
    Conflict
}

public sealed record AdminOperationResult<T>(AdminOperationStatus Status, T? Value = default, string? Message = null)
{
    public static AdminOperationResult<T> Success(T value) => new(AdminOperationStatus.Success, value);

    public static AdminOperationResult<T> NotFound(string? message = null) => new(AdminOperationStatus.NotFound, default, message);

    public static AdminOperationResult<T> BadRequest(string message) => new(AdminOperationStatus.BadRequest, default, message);

    public static AdminOperationResult<T> Conflict(string message) => new(AdminOperationStatus.Conflict, default, message);
}
