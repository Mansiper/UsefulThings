using System.Text.Json.Serialization;

namespace DataResultExample;

public class DataResult
{
	public RequestException? Exception { get; set; }
	[JsonIgnore]
	public virtual bool HasData => Exception is null;

	[JsonConstructor]
	public DataResult() { }
	public DataResult(RequestException exception) => Exception = exception;
	public DataResult(RequestExceptionType exceptionType, string? message = null, string? additional = null) =>
		Exception = new RequestException(exceptionType, message, additional);

	[JsonIgnore]
	public static DataResult Empty => new();
}

public class DataResult<T> : DataResult
{
	public T? Data { get; set; }
	public bool IsNull { get; set; }

	[JsonConstructor]
	public DataResult() {}
	public DataResult(T data)
	{
		Data = data;
		IsNull = Data is null;
	}
	public DataResult(RequestException exception) => Exception = exception;
	public DataResult(RequestExceptionType exceptionType, string? message = null, string? additional = null) =>
		Exception = new RequestException(exceptionType, message, additional);
	public DataResult(T? data, RequestExceptionType exceptionType, string? message = null, string? additional = null)
	{
		if (data is null)
			Exception = new RequestException(exceptionType, message, additional);
		else Data = data;
	}

	// public DataResult(List<(string, string)> validationMessages) =>
		// Exception = new RequestException(validationMessages);

	public DataResult<T1> NewResult<T1>(Func<T, T1> convertor) where T1 : new() =>
		HasData ? new(convertor(Data!)) : new(Exception!);

	[JsonIgnore]
	public override bool HasData => (Data is not null || IsNull) && Exception is null;
}