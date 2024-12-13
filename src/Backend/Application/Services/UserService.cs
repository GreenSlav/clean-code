using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(string username, string email, string password)
    {
        // Проверяем, существует ли пользователь
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
        {
            return (false, "User with this email already exists.");
        }

        // Хэшируем пароль
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Создаём нового пользователя
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = hashedPassword
        };

        await _userRepository.AddAsync(user);
        return (true, "User registered successfully.");
    }

    public async Task<(bool Success, string Message)> LoginAsync(string email, string password)
    {
        // Проверяем, существует ли пользователь
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return (false, "Invalid email or password.");
        }

        // Проверяем пароль
        var passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!passwordValid)
        {
            return (false, "Invalid email or password.");
        }

        return (true, "Login successful.");
    }
}