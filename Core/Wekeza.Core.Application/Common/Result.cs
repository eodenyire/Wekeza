namespace Wekeza.Core.Application.Common;

/// <summary>
/// Represents the result of an operation that can either succeed or fail
/// </summary>
/// <typeparam name="T">The type of the success value</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; private set; }
    public string Error { get; private set; } = string.Empty;
    public List<string> Errors { get; private set; } = new();

    private Result(bool isSuccess, T? value, string error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors ?? new List<string>();
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, string.Empty);
    }

    /// <summary>
    /// Creates a failed result with a single error
    /// </summary>
    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, default, error);
    }

    /// <summary>
    /// Creates a failed result with multiple errors
    /// </summary>
    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>(false, default, string.Join("; ", errors), errors);
    }

    /// <summary>
    /// Creates a failed result with multiple errors
    /// </summary>
    public static Result<T> Failure(params string[] errors)
    {
        return new Result<T>(false, default, string.Join("; ", errors), errors.ToList());
    }

    /// <summary>
    /// Implicit conversion from T to Result<T>
    /// </summary>
    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    /// <summary>
    /// Implicit conversion from string to failed Result<T>
    /// </summary>
    public static implicit operator Result<T>(string error)
    {
        return Failure(error);
    }
}

/// <summary>
/// Represents the result of an operation without a return value
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; private set; } = string.Empty;
    public List<string> Errors { get; private set; } = new();

    private Result(bool isSuccess, string error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? new List<string>();
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    /// <summary>
    /// Creates a failed result with a single error
    /// </summary>
    public static Result Failure(string error)
    {
        return new Result(false, error);
    }

    /// <summary>
    /// Creates a failed result with multiple errors
    /// </summary>
    public static Result Failure(List<string> errors)
    {
        return new Result(false, string.Join("; ", errors), errors);
    }

    /// <summary>
    /// Creates a failed result with multiple errors
    /// </summary>
    public static Result Failure(params string[] errors)
    {
        return new Result(false, string.Join("; ", errors), errors.ToList());
    }

    /// <summary>
    /// Implicit conversion from string to failed Result
    /// </summary>
    public static implicit operator Result(string error)
    {
        return Failure(error);
    }
}