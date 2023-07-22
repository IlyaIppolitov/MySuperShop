using System.Net;
using System.Runtime.Serialization;
using MySuperShop.HttpApiClient;
using MySuperShop.HttpModels.Responses;

namespace MySuperShop.HttpModels.Exceptions;

[Serializable]
public class MySuperShopApiException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public ErrorResponse? Error { get; set; }
    public ValidationProblemDetails? Details { get; }

    public MySuperShopApiException()
    {
    }

    public MySuperShopApiException(HttpStatusCode satusCode, ValidationProblemDetails details)
    {
        StatusCode = satusCode;
        Details = details;
    }

    public MySuperShopApiException(ValidationProblemDetails details)
    {
        Details = details;
    }


    public MySuperShopApiException(ErrorResponse error)
    {
        Error = error;
    }

    protected MySuperShopApiException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Error = new ErrorResponse(info.ToString());
    }

    public MySuperShopApiException(string? message, ErrorResponse error) : base(message)
    {
        Error = error;
    }

    public MySuperShopApiException(string? message) : base(message)
    {
        Error = new ErrorResponse(message);
    }

    public MySuperShopApiException(string? message, Exception? innerException, ErrorResponse error) : base(message, innerException)
    {
        Error = error;
    }
}