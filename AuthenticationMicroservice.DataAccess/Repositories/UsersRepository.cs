using AuthenticationMicroservice.DataAccess.Helpers;
using AuthenticationMicroservice.Domain.Abstractions.Repositories;
using AuthenticationMicroservice.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Serilog;

namespace AuthenticationMicroservice.DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly IServiceProvider _provider;

    public UsersRepository(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        try
        {
            await using NpgsqlConnection connection = _provider.GetRequiredService<NpgsqlConnection>();
            await connection.OpenAsync();

            string query = SqlLoader.Load("User/GetById.sql");

            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("id", id);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int idFromDb = reader.GetInt32(0);
                string email = reader.GetString(1);
                string passwordHash = reader.GetString(2);
                string salt = reader.GetString(3);
                bool isEmailConfirmed = reader.GetBoolean(4);
                int roleId = reader.GetInt32(5);
                string profileImageUrl = reader.GetString(6);

                Log.Information($"Got User With Id {idFromDb}");

                return new User
                {
                    Id = id,
                    Email = email,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    RoleId = roleId,
                    IsEmailConfirmed = isEmailConfirmed,
                    ProfileImageUrl = profileImageUrl
                };
            }

            return new User();
        }
        catch (Exception e)
        {
            Log.Error($"Error At UsersRepository::GetById, {e.Message}");
            return new User();
        }
    }


    public async Task<User> GetByEmailAsync(string email)
    {
        try
        {
            await using NpgsqlConnection connection = _provider.GetRequiredService<NpgsqlConnection>();
            await connection.OpenAsync();

            string query = SqlLoader.Load("User/GetByEmail.sql");

            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("email", email);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            Log.Information($"Got User With Email {email}");

            while (await reader.ReadAsync())
            {
                int userId = reader.GetInt32(0);
                string passwordHash = reader.GetString(2);
                string salt = reader.GetString(3);
                bool isEmailConfirmed = reader.GetBoolean(4);
                int roleId = reader.GetInt32(5);
                string profileImageUrl = reader.GetString(6);

                return new User
                {
                    Id = userId,
                    Email = email,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    RoleId = roleId,
                    IsEmailConfirmed = isEmailConfirmed,
                    ProfileImageUrl = profileImageUrl
                };
            }

            return new User();
        }
        catch (Exception e)
        {
            Log.Error($"Error At UsersRepository::GetByEmail, {e.Message}");
            return new User();
        }
    }

    public async Task<User> AddAsync(string email, string password, string salt, int roleId)
    {
        try
        {
            await using NpgsqlConnection connection = _provider.GetRequiredService<NpgsqlConnection>();
            await connection.OpenAsync();

            string query = SqlLoader.Load("User/AddUser.sql");

            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("email", email);
            command.Parameters.AddWithValue("passwordHash", password);
            command.Parameters.AddWithValue("salt", salt);
            command.Parameters.AddWithValue("isEmailConfirmed", false);
            command.Parameters.AddWithValue("roleId", 1);
            command.Parameters.AddWithValue("profileImageUrl", string.Empty);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            Log.Information("User Was Added");

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);

                return new User
                {
                    Id = id,
                    Email = email,
                    PasswordHash = password,
                    Salt = salt,
                    RoleId = roleId,
                    IsEmailConfirmed = false,
                    ProfileImageUrl = ""
                };
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error At UsersRepository::AddAsync, {e.Message}");
            return new User();
        }

        return new User();
    }

    // public async Task<User> LogInUserAsync(string login, string password)
    // {
    //     try
    //     {
    //         await using NpgsqlConnection connection = _provider.GetRequiredService<NpgsqlConnection>();
    //         await connection.OpenAsync();
    //
    //         string query = SqlLoader.Load("User/LogIn.sql");
    //
    //         await using NpgsqlCommand command = new NpgsqlCommand();
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError($"Error At UsersRepository::LogInUserAsync {e.Message}");
    //     }
    // }

    public async Task<RefreshToken> AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            await using NpgsqlConnection connection = _provider.GetRequiredService<NpgsqlConnection>();
            await connection.OpenAsync();

            string query = SqlLoader.Load("User/AddRefreshToken.sql");

            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", refreshToken.UserId);
            command.Parameters.AddWithValue("@token", refreshToken.Token);
            command.Parameters.AddWithValue("@expires", refreshToken.ExpiresAt);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            Log.Information($"Added Refresh Token");

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);

                if (id == 0)
                {
                    return new RefreshToken();
                }

                refreshToken.Id = id;

                return refreshToken;
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error At UsersRepository::AddRefreshTokenAsync {e.Message}");
            return new RefreshToken();
        }

        return new RefreshToken();
    }

    public async Task<RefreshToken> GetRefreshTokenByToken(string token)
    {
        try
        {
            await using NpgsqlConnection connection = _provider.GetRequiredService<NpgsqlConnection>();
            await connection.OpenAsync();

            string query = SqlLoader.Load("User/GetRefreshTokenByToken.sql");

            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("token", token);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            Log.Information("Got Refresh Token");

            while (await reader.ReadAsync())
            {
                int id = reader.GetInt32(0);
                DateTime expires = reader.GetDateTime(2);
                int userId = reader.GetInt32(3);

                return new RefreshToken
                {
                    Id = id,
                    Token = token,
                    ExpiresAt = expires,
                    UserId = userId
                };
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error At UsersRepository::GetRefreshTokenByToken {e.Message}");
            return new RefreshToken();
        }

        return new RefreshToken();
    }

    public async Task<bool> RemoveOldRefreshToken(string token)
    {
        try
        {
            await using NpgsqlConnection connection = _provider.GetRequiredService<NpgsqlConnection>();
            await connection.OpenAsync();

            string query = SqlLoader.Load("User/RemoveOldRefreshToken.sql");

            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("token", token);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            Log.Information($"Removed Old Refresh Token");

            while (await reader.ReadAsync())
            {
                return reader.GetBoolean(0);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error At UsersRepository::RemoveOldRefreshToken {e.Message}");
            return false;
        }

        return false;
    }

    public async Task<bool> RemoveUsersRefreshTokens(int userId)
    {
        try
        {
            await using NpgsqlConnection connection = _provider.GetRequiredService<NpgsqlConnection>();
            await connection.OpenAsync();

            string query = SqlLoader.Load("User/RemoveUserRefreshTokens.sql");

            await using NpgsqlCommand command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("userId", userId);

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

            Log.Information($"Removed {userId} All Old Refresh Tokens");

            while (await reader.ReadAsync())
            {
                bool rowsAffected = reader.GetBoolean(0);

                Console.WriteLine("rows affected - ", rowsAffected);

                return rowsAffected;
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error At UsersRepository::RemoveUsersRefreshTokens {e.Message}");
            return false;
        }

        return false;
    }
}
