using IEM.Application.Models.Commons;
using IEM.Application.Models.Pagination;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace IEM.Application.Utils
{
    public static class ResponseUtils
    {
        public static async ValueTask<IApiResponseModel<T>> AcceptedResultAsync<T>(ValueTask<T> data)
        {
            return await AcceptedResultAsync(data.AsTask());
        }

        public static async ValueTask<IApiResponseModel<T>> AcceptedResultAsync<T>(Task<T> data)
        {
            return await CreateResultAsync(data, StatusCodes.Status202Accepted);
        }

        public static IApiResponseModel<T> AcceptedResult<T>(T data)
        {
            return CreateResult(data, StatusCodes.Status202Accepted);
        }

        public static async ValueTask<IApiResponseModel<T>> CreatedResultAsync<T>(ValueTask<T> data)
        {
            return await CreatedResultAsync(data.AsTask());
        }

        public static async ValueTask<IApiResponseModel<T>> CreatedResultAsync<T>(Task<T> data)
        {
            return await CreateResultAsync(data, StatusCodes.Status201Created);
        }

        public static IApiResponseModel<T> CreatedResult<T>(T data)
        {
            return CreateResult(data, StatusCodes.Status201Created);
        }

        public static async ValueTask<IApiResponseModel<T>> OkResultAsync<T>(ValueTask<T> data)
        {
            return await CreateResultAsync(data.AsTask(), StatusCodes.Status200OK);
        }

        public static async ValueTask<IApiResponseModel<PaginationModel<T>>> OkResultAsync<T>(ValueTask<PaginationModel<T>> data)
        {
            return await CreateResultAsync(data.AsTask(), StatusCodes.Status200OK);
        }

        public static async ValueTask<IApiResponseModel<T>> OkResultAsync<T>(Task<T> data)
        {
            return await CreateResultAsync(data, StatusCodes.Status200OK);
        }

        public static IApiResponseModel<T> OkResult<T>(T data)
        {
            return CreateResult(data, StatusCodes.Status200OK);
        }

        public static async ValueTask<IApiResponseModel<T>> CreateResultAsync<T>(ValueTask<T> data, int statusCode)
        {
            return await CreateResultAsync(data.AsTask(), statusCode);
        }

        public static async ValueTask<IApiResponseModel<T>> CreateResultAsync<T>(Task<T> data, int statusCode)
        {
            return new ApiResponseModel<T>(await data, statusCode);
        }

        public static IApiResponseModel<T> CreateResult<T>(T data, int statusCode)
        {
            return new ApiResponseModel<T>(data, statusCode);
        }

        public static IApiErrorResponseModel CreateErrorResult(int errorCode, string errorMessage, string errorDetail = null)
        {
            return new ApiErrorResponseModel(errorCode, errorMessage, errorDetail);
        }

        public static IApiErrorResponseModel CreateErrorResult(int errorCode, Exception exception)
        {
            return new ApiErrorResponseModel(errorCode, exception.Message);
        }

        public static async Task CreateJsonResponseAsync(this JwtBearerChallengeContext context, int statusCode, int errorCode,
            string errorMessage, string errorDetail = null)
        {
            var boby = CreateErrorResult(errorCode, errorMessage, errorDetail);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(boby.ToString());
            context.HandleResponse();
        }

        public static async Task CreateJsonResponseAsync(this HttpContext context, int statusCode, int errorCode,
            string errorMessage, string errorDetail = null)
        {
            var boby = CreateErrorResult(errorCode, errorMessage, errorDetail);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(boby.ToString());
        }
    }
}
