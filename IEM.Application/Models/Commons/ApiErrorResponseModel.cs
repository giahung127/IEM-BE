using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace IEM.Application.Models.Commons
{
    public interface IApiErrorResponseModel
    {
        int ErrorCode { get; set; }
        string ErrorMessage { get; set; }
        string? ErrorDetail { get; set; }
        string? ErrorUrl { get; set; }

        string ToString();

        ModelStateDictionary? ModelError { get; set; }
    }

    public class ApiErrorResponseModel : IApiErrorResponseModel
    {
        public ApiErrorResponseModel(int errorCode, string errorMessage, string? errorDetail = null)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ErrorDetail = errorDetail!;
        }

        public ApiErrorResponseModel(int errorCode, string errorMessage, ModelStateDictionary modelError)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ModelError = modelError;
        }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string? ErrorDetail { get; set; }

        public string? ErrorUrl { get; set; }

        public DateTimeOffset Timestamp
        { get { return DateTimeOffset.UtcNow; } }

        public ModelStateDictionary? ModelError { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                ReadCommentHandling = JsonCommentHandling.Skip
            });
        }
    }
}
