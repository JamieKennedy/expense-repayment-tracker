using JetBrains.Annotations;

namespace Common.DataTransferObjects
{
    [UsedImplicitly]
    public record ErrorDto(int StatusCode, string Message);
}