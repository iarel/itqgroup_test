using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Infrastructure.Logging;

public class MongoRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMongoCollection<RequestLog> _collection;

    public MongoRequestLoggingMiddleware(RequestDelegate next, IMongoClient mongoClient)
    {
        _next = next;
        var database = mongoClient.GetDatabase("webservice_db");
        _collection = database.GetCollection<RequestLog>("request_logs");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        var requestBody = await ReadStreamAsync(context.Request.Body);
        context.Request.Body.Position = 0;

        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        var requestLog = new RequestLog
        {
            Path = context.Request.Path,
            Method = context.Request.Method,
            QueryString = context.Request.QueryString.ToString(),
            RequestBody = requestBody,
            Timestamp = DateTime.UtcNow
        };

        await _next(context);

        context.Response.Body.Position = 0;
        var responseBody = await ReadStreamAsync(context.Response.Body);
        context.Response.Body.Position = 0;

        await responseBodyStream.CopyToAsync(originalBodyStream);

        requestLog.ResponseBody = responseBody;
        requestLog.StatusCode = context.Response.StatusCode;

        await _collection.InsertOneAsync(requestLog);
    }

    private static async Task<string> ReadStreamAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}
