using IEM.Application.Models.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IEM.Application.Models.Commons
{
    public class ApiResponseModel<T> : IApiResponseModel<T>
    {
        public int StatusCode { get; set; }
        public T Data { get; set; }
        public IEnumerable<ApiErrorResponseModel> Errors { get; set; }

        public ApiResponseModel(T data, int statusCode = StatusCodes.Status200OK)
        {
            Data = data;
            StatusCode = statusCode;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            ObjectResult objectResult;
            if (Data is PaginationModel)
            {
                var paginationData = Data as PaginationModel;
                objectResult = new ObjectResult(new
                {
                    Data = paginationData.Data,
                    TotalItems = paginationData.TotalItems,
                    TotalPages = paginationData.TotalPages,
                    PageIndex = paginationData.PageIndex,
                    PageSize = paginationData.PageSize
                });

                if (this.Errors != null)
                {
                    objectResult.Value = new ObjectResult(new
                    {
                        Data = paginationData.Data,
                        TotalItems = paginationData.TotalItems,
                        TotalPages = paginationData.TotalPages,
                        PageIndex = paginationData.PageIndex,
                        PageSize = paginationData.PageSize,
                        Errors = this.Errors
                    });
                }
            }
            else
            {
                objectResult = new ObjectResult(new { Data = this.Data });
                if (this.Errors != null)
                {
                    objectResult.Value = new { Data = this.Data, Errors = this.Errors };
                }
            }
            objectResult.StatusCode = StatusCode;
            await objectResult.ExecuteResultAsync(context);
        }
    }
}
