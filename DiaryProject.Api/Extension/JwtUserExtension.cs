using System.Text;
using DiaryProject.Api.Service;
using DiaryProject.Shared.Dtos;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace DiaryProject.Api.Extension;

public static class JwtUserExtension
{
    private static readonly IJwtEncoder Encoder;
    private static readonly IJwtDecoder Decoder;
    
    private const string SecretKey = "corvoleftemilyabestdunwall";
    
    static JwtUserExtension()
    {
        IJsonSerializer serializer = new JsonNetSerializer();
        IDateTimeProvider provider = new UtcDateTimeProvider();
        IJwtValidator validator = new JwtValidator(serializer, provider);
        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
        Decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
        Encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
    }
    
    public static string GetToken(UserDto? user)
    {
        var key = Encoding.UTF8.GetBytes(SecretKey);
        return Encoder.Encode(user, key);
    }

    public static async Task<int> GetUserId(HttpContext httpContext, IUserService userService)
    {
        try
        {
            var authHeader = from t in httpContext.Request.Headers
                where t.Key == "auth"
                select t.Value.FirstOrDefault();
            var enumerable = authHeader as string[] ?? authHeader.ToArray();
            var token = enumerable.FirstOrDefault();
            if (string.IsNullOrEmpty(enumerable.FirstOrDefault())) throw new Exception("Authorization failed");
            var key = Encoding.UTF8.GetBytes(SecretKey);
            var authInfo = Decoder.DecodeToObject<UserDto>(token, key);
            if (authInfo == null || !(await userService.VerifyUserAsync(authInfo.UserName, authInfo.Password)).Status)
                throw new Exception("Authorization failed");
            return authInfo.Id;
        }
        catch (Exception)
        {
            return -1;
        }
    }
}