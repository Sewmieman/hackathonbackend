namespace FixMyCampus.Api.Models;

public enum UserRole
{
    Admin,
    Technician,
    Student
}

public class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}