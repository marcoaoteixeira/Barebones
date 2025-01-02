using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Nameless.WebApp;

/// <summary>
/// A guard clause (https://en.wikipedia.org/wiki/Guard_(computer_science))
/// is a software pattern that simplifies complex functions by "failing fast",
/// checking for invalid inputs up front and immediately failing if any are found.
/// </summary>
public class Prevent {
    private const string ArgNullMessage = "Argument cannot be null";
    private const string ArgEmptyMessage = "Argument cannot be empty";
    private const string ArgDefaultMessage = "Argument cannot be default";
    private const string ArgWhiteSpacesMessage = "Argument cannot be white spaces";
    private const string ArgNoMatchingPatternMessage = "Argument does not match pattern: {0}";
    private const string ArgLowerOrEqualMessage = "Argument cannot be lower or equal to {0}";
    private const string ArgGreaterOrEqualMessage = "Argument cannot be greater or equal to {0}";
    private const string ArgLowerThanMessage = "Argument cannot be lower than {0}";
    private const string ArgGreaterThanMessage = "Argument cannot be greater than {0}";
    private const string ArgOutOfRangeMessage = "Argument must be between min ({0}) and max ({1}) values";
    private const string ArgZeroMessage = "Argument cannot be zero";
    private const string ArgUndefinedEnumMessage = "Argument '{0}' provides value '{1}' that is not defined in enum '{2}'";

    /// <summary>
    /// Gets the unique instance of <see cref="Prevent" />.
    /// </summary>
    public static Prevent Argument { get; } = new();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static Prevent() { }

    private Prevent() { }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if parameter <paramref name="paramValue"/> is <c>null</c>.
    /// </summary>
    /// <typeparam name="TValue">Type of the parameter value.</typeparam>
    /// <param name="paramValue">The parameter value.</param>
    /// <param name="paramName">The parameter name (optional).</param>
    /// <param name="message">The exception message (optional).</param>
    /// <param name="exceptionCreator">The exception creator (optional).</param>
    /// <returns>
    /// The current <paramref name="paramValue"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static TValue Null<TValue>([NotNull] TValue? paramValue,
                                      [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                      string? message = null,
                                      Func<Exception>? exceptionCreator = null) where TValue : class {
        if (paramValue is not null) {
            return paramValue;
        }

        throw exceptionCreator?.Invoke() ?? new ArgumentNullException(paramName: paramName,
                                                                      message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgNullMessage
                                                                          : message);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if parameter <paramref name="paramValue"/> is <c>null</c>.
    /// </summary>
    /// <typeparam name="TValue">Type of the parameter value.</typeparam>
    /// <param name="paramValue">The parameter value.</param>
    /// <param name="paramName">The parameter name (optional).</param>
    /// <param name="message">The exception message (optional).</param>
    /// <param name="exceptionCreator">The exception creator (optional).</param>
    /// <returns>
    /// The current <paramref name="paramValue"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="paramValue"/> is <c>null</c>.
    /// </exception>
    [DebuggerStepThrough]
    public static TValue Null<TValue>([NotNull] TValue? paramValue,
                                      [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                      string? message = null,
                                      Func<Exception>? exceptionCreator = null) where TValue : struct {
        if (paramValue is not null) {
            return paramValue.Value;
        }

        throw exceptionCreator?.Invoke() ?? new ArgumentNullException(paramName: paramName,
                                                                      message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgNullMessage
                                                                          : message);
    }

    [DebuggerStepThrough]
    public static string NullOrEmpty([NotNull] string? paramValue,
                                     [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                     string? message = null,
                                     Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue.Length == 0) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgEmptyMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static string NullOrWhiteSpace([NotNull] string? paramValue,
                                          [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                          string? message = null,
                                          Func<Exception>? exceptionCreator = null) {
        NullOrEmpty(paramValue, paramName, message, exceptionCreator);

        if (paramValue.Trim().Length == 0) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgWhiteSpacesMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static Guid NullOrEmpty([NotNull] Guid? paramValue,
                                   [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                   string? message = null,
                                   Func<Exception>? exceptionCreator = null) {
        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue == Guid.Empty) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgEmptyMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue.Value;
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    [DebuggerStepThrough]
    public static T NullOrEmpty<T>([NotNull] T? paramValue,
                                   [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                   string? message = null,
                                   Func<Exception>? exceptionCreator = null) where T : class, IEnumerable {

        Null(paramValue, paramName, message, exceptionCreator);

        if (paramValue is Array { Length: 0 } or ICollection { Count: 0 }) {
            ThrowError(paramName, message, exceptionCreator);
        }

        // Unfortunately, it needs to enumerate here =/
        var enumerator = paramValue.GetEnumerator();
        var canMoveNext = enumerator.MoveNext();
        if (enumerator is IDisposable disposable) {
            disposable.Dispose();
        }

        if (!canMoveNext) {
            ThrowError(paramName, message, exceptionCreator);
        }

        return paramValue;

        static void ThrowError(string? paramName, string? message, Func<Exception>? exceptionCreator) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgEmptyMessage
                                                                          : message,
                                                                      paramName: paramName);
        }
    }

    [DebuggerStepThrough]
    public static T Default<T>([NotNull] T? paramValue,
                               [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                               string? message = null,
                               Func<Exception>? exceptionCreator = null) {
        if (EqualityComparer<T?>.Default.Equals(paramValue, default) || paramValue is null) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgDefaultMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static string NoMatchingPattern(string paramValue,
                                           string pattern,
                                           [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                           string? message = null,
                                           Func<Exception>? exceptionCreator = null) {
        var match = Regex.Match(paramValue, pattern);
        if (!match.Success || match.Value != paramValue) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? string.Format(ArgNoMatchingPatternMessage, pattern)
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<char> Empty(ReadOnlySpan<char> paramValue,
                                           [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                           string? message = null,
                                           Func<Exception>? exceptionCreator = null) {
        if (paramValue.Length == 0 || paramValue == string.Empty) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgEmptyMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static ReadOnlySpan<char> WhiteSpace(ReadOnlySpan<char> paramValue,
                                                [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                                string? message = null,
                                                Func<Exception>? exceptionCreator = null) {
        if (paramValue.IsWhiteSpace()) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgWhiteSpacesMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static TimeSpan LowerOrEqual(TimeSpan paramValue,
                                        TimeSpan to,
                                        [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                        string? message = null,
                                        Func<Exception>? exceptionCreator = null) {
        if (paramValue <= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ArgLowerOrEqualMessage, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static TimeSpan GreaterOrEqual(TimeSpan paramValue,
                                          TimeSpan to,
                                          [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                          string? message = null,
                                          Func<Exception>? exceptionCreator = null) {
        if (paramValue >= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ArgGreaterOrEqualMessage, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int LowerOrEqual(int paramValue,
                                   int to,
                                   [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                   string? message = null,
                                   Func<Exception>? exceptionCreator = null) {
        if (paramValue <= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ArgLowerOrEqualMessage, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int GreaterOrEqual(int paramValue,
                                     int to,
                                     [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                     string? message = null,
                                     Func<Exception>? exceptionCreator = null) {
        if (paramValue >= to) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ArgGreaterOrEqualMessage, to)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int LowerThan(int paramValue,
                                int minValue,
                                [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                string? message = null,
                                Func<Exception>? exceptionCreator = null) {
        if (paramValue < minValue) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ArgLowerThanMessage, minValue)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int GreaterThan(int paramValue,
                                  int maxValue,
                                  [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                  string? message = null,
                                  Func<Exception>? exceptionCreator = null) {
        if (paramValue > maxValue) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ArgGreaterThanMessage, maxValue)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int OutOfRange(int paramValue,
                                 int min,
                                 int max,
                                 [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                 string? message = null,
                                 Func<Exception>? exceptionCreator = null) {
        if (paramValue < min || paramValue > max) {
            throw exceptionCreator?.Invoke() ?? new ArgumentOutOfRangeException(message: string.IsNullOrWhiteSpace(message)
                                                                                    ? string.Format(ArgOutOfRangeMessage, min, max)
                                                                                    : message,
                                                                                paramName: paramName,
                                                                                actualValue: paramValue);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static int Zero(int paramValue,
                           [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                           string? message = null,
                           Func<Exception>? exceptionCreator = null) {
        if (paramValue == 0) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgZeroMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static double Zero(double paramValue,
                              [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                              string? message = null,
                              Func<Exception>? exceptionCreator = null) {
        if (paramValue == 0D) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgZeroMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static decimal Zero(decimal paramValue,
                               [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                               string? message = null,
                               Func<Exception>? exceptionCreator = null) {
        if (paramValue == 0M) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgZeroMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static TimeSpan Zero(TimeSpan paramValue,
                                [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                string? message = null,
                                Func<Exception>? exceptionCreator = null) {
        if (paramValue == TimeSpan.Zero) {
            throw exceptionCreator?.Invoke() ?? new ArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                          ? ArgZeroMessage
                                                                          : message,
                                                                      paramName: paramName);
        }

        return paramValue;
    }

    [DebuggerStepThrough]
    public static TEnum UndefinedEnum<TEnum>(int paramValue,
                                             [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                             Func<Exception>? exceptionCreator = null)
        where TEnum : struct, Enum {
        if (!Enum.IsDefined(typeof(TEnum), paramValue)) {
            throw exceptionCreator?.Invoke() ?? new InvalidEnumArgumentException(paramName, paramValue, typeof(TEnum));
        }

        return (TEnum)Enum.ToObject(typeof(TEnum), paramValue);
    }

    [DebuggerStepThrough]
    public static TEnum UndefinedEnum<TEnum>(string paramValue,
                                             [CallerArgumentExpression(nameof(paramValue))] string? paramName = null,
                                             string? message = null,
                                             Func<Exception>? exceptionCreator = null)
        where TEnum : struct, Enum {
        if (!Enum.IsDefined(typeof(TEnum), paramValue)) {
            throw exceptionCreator?.Invoke() ?? new InvalidEnumArgumentException(message: string.IsNullOrWhiteSpace(message)
                                                                                     ? string.Format(ArgUndefinedEnumMessage, paramName, paramValue, typeof(TEnum))
                                                                                     : message);
        }

        return (TEnum)Enum.ToObject(typeof(TEnum), paramValue);
    }
}