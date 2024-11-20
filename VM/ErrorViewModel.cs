namespace TreeStructure.VM
{
    public class ErrorViewModel
    {
        public string RequestId { get; init; } = string.Empty;

        public bool ShowRequestId => !string.IsNullOrWhiteSpace(RequestId);
    }
}
