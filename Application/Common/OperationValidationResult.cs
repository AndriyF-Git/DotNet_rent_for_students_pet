namespace rent_for_students.Application.Common
{
    public readonly record struct OperationValidationResult
    {
        public bool IsValid { get; }
        public string? ErrorCode { get; }
        public string? ErrorMessage { get; }

        private OperationValidationResult(bool isValid, string? errorCode, string? errorMessage)
        {
            IsValid = isValid;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public static OperationValidationResult Valid()
            => new(isValid: true, errorCode: null, errorMessage: null);

        public static OperationValidationResult Invalid(string message, string? errorCode = null)
            => new(isValid: false, errorCode: errorCode, errorMessage: message);
    }
}
