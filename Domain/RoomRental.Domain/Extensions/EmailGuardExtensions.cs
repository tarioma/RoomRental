using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Ardalis.GuardClauses;

namespace RoomRental.Domain.Extensions;

/// <summary>
/// Проверка email-адресов для Ardalis.GuardClauses.
/// </summary>
public static partial class EmailGuardExtensions
{
    private static readonly Regex EmailRegex = Regex();

    /// <summary>
    /// Отклоняет пустой или синтаксически некорректный email.
    /// </summary>
    /// <param name="guard">Точка расширения Ardalis.GuardClauses.</param>
    /// <param name="input">Проверяемый адрес.</param>
    /// <param name="parameterName">Имя параметра для текста ошибки.</param>
    /// <param name="message">Своё сообщение об ошибке.</param>
    /// <returns>Тот же адрес, если он корректен.</returns>
    public static string InvalidEmail(
        this IGuardClause guard,
        string input,
        [CallerArgumentExpression(nameof(input))] string? parameterName = null,
        string? message = null)
    {
        Guard.Against.NullOrWhiteSpace(input, parameterName);

        if (!EmailRegex.IsMatch(input))
        {
            throw new ArgumentException(
                message ?? $"Значение '{input}' не является корректным email.",
                parameterName);
        }

        return input;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-EN")]
    private static partial Regex Regex();
}