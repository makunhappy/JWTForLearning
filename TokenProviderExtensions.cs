using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace test
{
    public static class TokenProviderExtensions
    {
        public static IApplicationBuilder UseAuthentication(this IApplicationBuilder app, TokenProviderOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));
        }
    }
}