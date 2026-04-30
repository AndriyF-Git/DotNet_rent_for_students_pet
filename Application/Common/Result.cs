namespace rent_for_students.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; protected init; }
        public string? ErrorCode { get; protected init; }
        public string? Message { get; protected init; }

        public static Result Success(string? message = null)
            => new()
            {
                IsSuccess = true,
                Message = message
            };

        public static Result Failure(string errorCode, string message)
            => new()
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                Message = message
            };
    }

    public sealed class Result<T> : Result
    {
        public T? Value { get; private init; }

        public static Result<T> Success(T value, string? message = null)
            => new()
            {
                IsSuccess = true,
                Value = value,
                Message = message
            };

        public static new Result<T> Failure(string errorCode, string message)
            => new()
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                Message = message
            };
    }
}
