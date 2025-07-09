namespace Service.Contracts.Services;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static ServiceResult<T> Ok(T data) => new ServiceResult<T> { Success = true, Data = data };
    public static ServiceResult<T> Fail(string error) => new ServiceResult<T> { Success = false, ErrorMessage = error };
}
