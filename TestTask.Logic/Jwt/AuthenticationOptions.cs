using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace TestTask.Logic.Jwt;

public class AuthenticationOptions
{
    public const string     Issuer = "KompasServer";
    public const string     Audience = "KompasUser";
    public const int        AccessTokenExpirationMinutes = 15;
    public const int        RefreshTokenExpirationMinutes = 1440;
    const string            SecretKey = "Kompas-SecretKey_for_TestTask!";

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
    }
}