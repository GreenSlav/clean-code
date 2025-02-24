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
        // Проверяем уникальность email
        var existingUserByEmail = await _userRepository.GetByEmailAsync(email);
        if (existingUserByEmail != null)
        {
            return (false, "User with this email already exists.");
        }

        // Проверяем уникальность username
        var existingUserByUsername = await _userRepository.GetByUsernameAsync(username);
        if (existingUserByUsername != null)
        {
            return (false, "User with this username already exists.");
        }

        // Хэшируем пароль
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Создаём нового пользователя
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = hashedPassword,
            Role = "User" // Присваиваем роль по умолчанию
        };

        await _userRepository.AddAsync(user);
        return (true, "User registered successfully.");
    }

    public async Task<(bool Success, string Message, User? User)> LoginAsync(string identifier, string password)
    {
        // Ищем пользователя по email или username
        var user = await _userRepository.GetByEmailAsync(identifier) ?? await _userRepository.GetByUsernameAsync(identifier);

        if (user == null)
        {
            return (false, "Invalid email/username or password.", null);
        }

        // Проверяем пароль
        var passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!passwordValid)
        {
            return (false, "Invalid email/username or password.", null);
        }

        return (true, "Login successful.", user);
    }
}