namespace TestTask.DataAccess.Models;

public class User
{
    public string UserLogin { get; }
    public string PasswordHash { get; }
    public byte[] Salt { get; }

    /*
     * Конструктор объекта существующего пользователя из БД
     */
    public User(string userLogin, string passwordHash, byte[] salt)
    {
        UserLogin = userLogin;
        PasswordHash = passwordHash;
        Salt = salt;
    }
}