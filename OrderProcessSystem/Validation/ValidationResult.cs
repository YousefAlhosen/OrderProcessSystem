namespace OrderProcessSystem.Validation
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }

        public ValidationResult(bool isValid, params string[] errors)
        {
            IsValid = isValid;
            Errors = errors?.ToList() ?? new List<string>();
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

    }
}
