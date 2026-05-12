namespace BdsAdmin.API.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = "Success";
    public object? Errors { get; set; }

    public static ApiResponse<T> Ok(T? data, string message = "Success") => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static ApiResponse<T> Fail(string message, object? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };
}
