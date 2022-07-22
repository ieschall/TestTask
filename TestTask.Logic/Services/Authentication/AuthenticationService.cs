using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Authentication;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using TestTask.DataAccess;
using TestTask.DataAccess.Exceptions;
using TestTask.DataAccess.Models;
using TestTask.Logic.Jwt;
using TestTask.Logic.Services.Authentication.Responses;

namespace TestTask.Logic.Services.Authentication;

public class AuthenticationService
{
    private static readonly Database Db;
    private static IReadOnlyCollection<User> Users { get; }
    
    static AuthenticationService()
    {
        Db = new Database();
        Users = Db.GetDataAllUsers();
    }
        
    public AuthenticationResult Login(string username, string password)
    {
        AuthenticateUser(username, password);
        
        var tokens = GenerateTokensPair(username);
        return tokens;
    }
    
    public AuthenticationResult Refresh(string refreshToken)
    {
        try
        {
            var userData = Db.GetUserDataByRefreshToken(refreshToken);
            if (userData.ValidUntil < DateTimeOffset.Now)
                throw new AuthenticationException("Token was expired");

            Db.DeleteOldRefreshToken(refreshToken);

            var tokens = GenerateTokensPair(userData.Username);
            return tokens;
        }
        catch (EntityNotFoundException ex)
        {
            throw new AuthenticationException(ex.Message);
        }
    }

    public void Logout(string refreshToken)
    {
        //TODO: блокировать access_token
        Db.DeleteOldRefreshToken(refreshToken);
    }
    
    private AuthenticationResult GenerateTokensPair(string username)
    {
        var accessToken = GenerateAccessToken(username);
        var refreshToken = GenerateRefreshToken();
            
        var refreshTokenExpiration =
            DateTimeOffset.Now + TimeSpan.FromMinutes(AuthenticationOptions.RefreshTokenExpirationMinutes);
        Db.AddRefreshToken(username, refreshToken, refreshTokenExpiration);
        
        return new AuthenticationResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken);
    }

    private static void AuthenticateUser(string username, string password)
    {
        var user = Users.FirstOrDefault(u => u.UserLogin == username);
        if (user is null)
            throw new AuthenticationException("User not found");
        
        var passwordHash = PasswordHash(password, user.Salt);
        if (passwordHash != user.PasswordHash)
            throw new AuthenticationException("Invalid password");
    }

    private static string GenerateAccessToken(string username)
    {
        var now = DateTime.Now;
        var jwt = new JwtSecurityToken(
            claims: new[]
            {
                new Claim("name", username),
                new Claim("role", "role")
            },
            issuer: AuthenticationOptions.Issuer,
            audience: AuthenticationOptions.Audience,
            expires: now.Add(TimeSpan.FromMinutes(AuthenticationOptions.AccessTokenExpirationMinutes)),
            notBefore: now,
            signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        return encodedJwt;
    }
    
    private static string GenerateRefreshToken()
    {
        var token = Guid.NewGuid();
        return token.ToString();
    }
    
    /*
     * Конструктор вычисления хэша пароля для дальнейшего сравнения
     * по существующей базе пользователей и их хэшей.
     */
    private static string PasswordHash(string password, byte[] saltBytes)
    {
        var passwordHash = new StringBuilder(64);

        using (var algo = SHA256.Create())
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            var passwordWithSaltBytes = passwordBytes.Concat(saltBytes).ToArray();
            var hashBytes = algo.ComputeHash(passwordWithSaltBytes);

            foreach (byte b in hashBytes)
                passwordHash.Append(b.ToString("x2"));
        }

        return passwordHash.ToString();
    }
}