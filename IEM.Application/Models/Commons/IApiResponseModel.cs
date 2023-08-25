using Microsoft.AspNetCore.Mvc;

namespace IEM.Application.Models.Commons
{
    public interface IApiResponseModel : IActionResult
    {
        int StatusCode { get; set; }
        IEnumerable<ApiErrorResponseModel> Errors { get; set; }
    }

    public interface IApiResponseModel<T> : IApiResponseModel
    {
        T Data { get; set; }
    }
}
