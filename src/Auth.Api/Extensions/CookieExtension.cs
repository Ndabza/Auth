namespace Auth.Api.Extensions;

public static class CookieExtension
{
    public static void SetCookie(this IResponseCookies responseCookies, string name, string token, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = expires,
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        responseCookies.Append(name, token, cookieOptions);
    }

    public static void RemoveCookie(this IResponseCookies responseCookies, string cookieName)
    {
        responseCookies.Delete(cookieName);
    }
}