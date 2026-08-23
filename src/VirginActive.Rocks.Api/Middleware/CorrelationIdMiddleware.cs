using Serilog.Context;

namespace VirginActive.Rocks.Api.Middleware
{
    public sealed class CorrelationIdMiddleware(RequestDelegate next)
    {
        public const string HeaderName = "X-Correlation-Id";

        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetCorrelationId(context);

            context.TraceIdentifier = correlationId;
            context.Response.Headers[HeaderName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }

        private static string GetCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(HeaderName, out var suppliedCorrelationId) &&
                !string.IsNullOrWhiteSpace(suppliedCorrelationId.ToString()))
            {
                return suppliedCorrelationId.ToString();
            }

            return Guid.NewGuid().ToString("N");
        }
    }
}
