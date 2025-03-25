using System;

public class User
{
    public string gmail;
    public string name;
    public string password;
    public string createdAt;

    public User(string gmail, string name, string password)
    {
        this.gmail = gmail;
        this.name = name;
        this.password = password;
        this.createdAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); // Store timestamp in UTC
    }
}
