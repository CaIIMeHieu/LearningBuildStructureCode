using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Abstractions.Shared;

public enum ErrorTypeEnum
{
    None,
    NotFound,
    Validation,
    Conflict
}

public class Error : IEquatable<Error>
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

    public Error(string code, string message, ErrorTypeEnum errorType = ErrorTypeEnum.None)
    {
        Code = code;
        Message = message;
        ErrorType = errorType;
    }

    public string Code { get; }

    public string Message { get; }

    public ErrorTypeEnum ErrorType { get; }

    public static implicit operator string(Error error) => error.Code;

    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Error? a, Error? b) => !(a == b);
    public static Error NotFound(string code, string message) => new Error(code, message, ErrorTypeEnum.NotFound);
    public static Error Validation(string code, string message) => new Error(code, message, ErrorTypeEnum.Validation);
    public static Error Conflict(string code, string message) => new Error(code, message, ErrorTypeEnum.Conflict);
    public virtual bool Equals(Error? other)
    {
        if (other is null)
        {
            return false;
        }

        return Code == other.Code && Message == other.Message;
    }

    public override bool Equals(object? obj) => obj is Error error && Equals(error);

    public override int GetHashCode() => HashCode.Combine(Code, Message);

    public override string ToString() => Code;
}
