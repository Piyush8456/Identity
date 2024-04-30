using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace Custom_Identity
{
    public class UnauthorizedRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public UnauthorizedRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check if the user is unauthorized
            if (!context.User.Identity.IsAuthenticated)
            {
                // Redirect to a custom unauthorized page
                context.Response.Redirect("/Roles/AccessDenide");
                return;
            }

            // Continue to the next middleware
            await _next(context);
        }
    }
    public static class UnauthorizedRedirectMiddlewareExtensions
    {
        public static IApplicationBuilder UseUnauthorizedRedirect(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UnauthorizedRedirectMiddleware>();
        }
    }
}
