namespace UserAuthNOrg.Utilities.Extensions
{
    public class ResultModel<T>
    {
        public ResultModel()
        {
        }

        public ResultModel(T data, string message)
        {
            Data = data;
            Message = message;
        }

        public ResultModel(List<Error> errorMessage)
        {
            AddError(errorMessage);
        }

        public List<Error> Errors { get; set; } = new List<Error>();

        public string Message { get; set; }

        public T Data { get; set; }

        public void AddError(IEnumerable<Error> validationResults)
        {
            Errors.AddRange(validationResults);
        }
    }
}
