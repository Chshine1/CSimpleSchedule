namespace DiaryProject.Shared.Contact;

public class ApiResponse
{
    public ApiResponse(string message, bool status = false)
    {
        Message = message;
        Status = status;
    }

    public ApiResponse(bool status, object result)
    {
        Status = status;
        Result = result;
    }
    
    public string? Message { get; set; }
    
    public bool Status { get; set; }
    
    public object? Result { get; set; }
}

public class ApiResponse<T>
{
    public bool Connected { get; init; } = true;
    
    public string? Message { get; init; }
    
    public bool Status { get; init; }
    
    public T? Result { get; init; }
}