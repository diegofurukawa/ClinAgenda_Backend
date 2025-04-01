// AuthUseCase.cs
using System.Security.Cryptography;
using System.Text;
using ClinAgenda.src.Application.DTOs.Auth;
using ClinAgenda.src.Application.DTOs.User;
using ClinAgenda.src.Application.Services;
using ClinAgenda.src.Core.Entities;
using ClinAgenda.src.Core.Interfaces;

namespace ClinAgenda.src.Application.AuthUseCase
{
    public class AuthUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAuthRepository _authRepository;
        private readonly JwtService _jwtService;

        public AuthUseCase(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IAuthRepository authRepository,
            JwtService jwtService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDTO?> LoginAsync(UserLoginDTO login)
        {
            // Buscar usuário pelo nome de usuário
            var user = await _userRepository.GetUserByUsernameAsync(login.Username);
            if (user == null || !user.LActive)
                return null;

            // Verificar senha
            if (!VerifyPasswordHash(login.Password, user.PasswordHash))
            {
                // Incrementar contagem de tentativas falhas
                user.NFailedLoginAttempts++;
                await _userRepository.UpdateUserAsync(user);
                return null;
            }

            // Resetar contagem de tentativas falhas e atualizar último login
            user.NFailedLoginAttempts = 0;
            user.DLastLogin = DateTime.Now;
            await _userRepository.UpdateUserAsync(user);

            // Revogar tokens antigos
            await _authRepository.RevokeAllUserTokensAsync(user.UserId);

            // Obter papéis do usuário
            var userRoles = await _userRepository.GetUserRolesAsync(user.UserId);
            var roleNames = userRoles.Select(r => r.RoleName).ToList();

            // Gerar token JWT
            var token = _jwtService.GenerateToken(user.UserId, user.Username, roleNames, out DateTime expires);

            // Salvar token no banco
            await _authRepository.CreateTokenAsync(user.UserId, token, expires);

            // Retornar resposta
            return new AuthResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Token = token,
                Roles = roleNames,
                TokenExpires = expires
            };
        }

        public async Task<AuthResponseDTO?> RegisterAsync(UserRegistrationDTO registration)
        {
            // Verificar se o nome de usuário já existe
            if (await _userRepository.UserExistsAsync(registration.Username))
                throw new ArgumentException("Nome de usuário já está em uso");

            // Verificar se o email já existe
            if (await _userRepository.EmailExistsAsync(registration.Email))
                throw new ArgumentException("Email já está em uso");

            // Criar hash da senha
            string passwordHash = CreatePasswordHash(registration.Password);

            // Criar usuário
            var user = new User
            {
                Username = registration.Username,
                Email = registration.Email,
                PasswordHash = passwordHash,
                LActive = true
            };

            // Salvar usuário
            int userId = await _userRepository.CreateUserAsync(user);
            user.UserId = userId;

            // Atribuir papel padrão (usuário regular)
            var defaultRole = await _roleRepository.GetRoleByNameAsync("User");
            if (defaultRole == null)
            {
                // Criar papel padrão se não existir
                var newRole = new Role
                {
                    RoleName = "User",
                    Description = "Regular application user",
                    LActive = true
                };
                
                int roleId = await _roleRepository.CreateRoleAsync(newRole);
                defaultRole = new Role 
                { 
                    RoleId = roleId, 
                    RoleName = "User", 
                    Description = "Regular application user",
                    LActive = true
                };
            }

            await _userRepository.AssignRoleToUserAsync(userId, defaultRole.RoleId);

            // Se o usuário estiver vinculado a uma entidade (médico ou paciente)
            if (!string.IsNullOrEmpty(registration.EntityType) && registration.EntityId.HasValue)
            {
                // Aqui você pode implementar a vinculação do usuário à entidade
                // usando a tabela user_entity do seu banco de dados
            }

            // Gerar token JWT
            var roleNames = new List<string> { defaultRole.RoleName };
            var token = _jwtService.GenerateToken(userId, registration.Username, roleNames, out DateTime expires);

            // Salvar token no banco
            await _authRepository.CreateTokenAsync(userId, token, expires);

            // Retornar resposta
            return new AuthResponseDTO
            {
                UserId = userId,
                Username = registration.Username,
                Email = registration.Email,
                Token = token,
                Roles = roleNames,
                TokenExpires = expires
            };
        }

        public async Task<bool> LogoutAsync(string token)
        {
            return await _authRepository.RevokeTokenAsync(token);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var storedToken = await _authRepository.GetValidTokenAsync(token);
            
            if (storedToken == null || !storedToken.LActive || storedToken.DExpires < DateTime.Now)
                return false;

            return _jwtService.ValidateToken(token, out _);
        }

        // Métodos auxiliares para hash de senha
        private static string CreatePasswordHash(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private static bool VerifyPasswordHash(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            string computedHash = Convert.ToBase64String(hash);
            return computedHash == storedHash;
        }
    }
}