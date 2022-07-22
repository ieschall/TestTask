using Npgsql;

using TestTask.DataAccess.Models;
using TestTask.DataAccess.Exceptions;

namespace TestTask.DataAccess;

public class Database
{ 
    private const string ConnectionString = "Host=localhost;Username=kompasuser;Password=kompasuser;Database=kompasdb";
        
    public List<User> GetDataAllUsers()
    {
        using var connect = new NpgsqlConnection(ConnectionString); 
        connect.Open();
        
        using var cmd = new NpgsqlCommand("SELECT * FROM users", connect); 
        using var reader = cmd.ExecuteReader();
        
        var users = new List<User>(reader.FieldCount);
        
        while (reader.Read()) 
        { 
            users.Add(new User(
                    reader.GetString(1), /* user_login */
                    reader.GetString(2), /* password_hash */
                    reader.GetFieldValue<byte[]>(3))); /* salt */
        }
                
        reader.Close();
        connect.Close();

        return users; 
    }

    public List<Marker> GetAllMarkers()
    { 
        using var connect = new NpgsqlConnection(ConnectionString); 
        connect.Open();
        
        using var cmd = new NpgsqlCommand("SELECT * FROM markers", connect); 
        using var reader = cmd.ExecuteReader();
        
        var markers = new List<Marker>(reader.FieldCount);
        
        while (reader.Read()) 
        { 
            markers.Add(new Marker(
                reader.GetInt32(0), /* marker_id */
                reader.GetString(1), /* marker_text */
                reader.GetString(2), /* latitude */
                reader.GetString(3) /* longitude */
                ));
        }
            
        reader.Close();
        connect.Close();

        return markers;
    }
    
    public void AddRefreshToken(string username, string refreshToken, DateTimeOffset validUntil)
    {
        using var connect = new NpgsqlConnection(ConnectionString);
        connect.Open();

        var sql = 
@$"
INSERT INTO refresh_tokens (username, refresh_token, valid_until)
values
('{username}', '{refreshToken}', '{validUntil}');
";
            
        using var cmd = new NpgsqlCommand(sql, connect);
        cmd.ExecuteNonQuery(); 
        connect.Close();
    }

    public UserRefreshData GetUserDataByRefreshToken(string refreshToken)
    {
        using var connect = new NpgsqlConnection(ConnectionString);
        connect.Open();

        using var cmd = new NpgsqlCommand(
            $"SELECT username, valid_until FROM refresh_tokens WHERE refresh_token = '{refreshToken}'",
            connect);
        using var reader = cmd.ExecuteReader();

        if (!reader.Read()) 
            throw new EntityNotFoundException("Refresh token not found");
            
        var user = new UserRefreshData(
            reader.GetString(0),
            DateTimeOffset.Parse(reader.GetString(1))
        );
            
        reader.Close(); 
        connect.Close();
        
        return user;
    }

    public void DeleteOldRefreshToken(string refreshToken)
    {
        using var connect = new NpgsqlConnection(ConnectionString);
        connect.Open();

        using var cmd = new NpgsqlCommand($"DELETE FROM refresh_tokens WHERE refresh_token='{refreshToken}'", connect);
        cmd.ExecuteNonQuery();
        connect.Close();
    }
}