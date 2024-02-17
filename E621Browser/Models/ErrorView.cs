namespace E621Browser.Models;

// Model for displaying error information on the error view page
public class ErrorView
{
    public string? RequestId { get; set; } // The request ID associated with the error

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Indicates whether the RequestId should be shown
}