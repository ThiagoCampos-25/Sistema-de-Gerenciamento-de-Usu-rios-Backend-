using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (await _unitOfWork.Users.GetByUsernameAsync(createUserDto.Username) != null)
            {
                throw new ArgumentException("Username already exists.");
            }
            if (await _unitOfWork.Users.GetByEmailAsync(createUserDto.Email) != null)
            {
                throw new ArgumentException("Email already exists.");
            }

            var user = new User
            {
                UserName = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password)
            };

            if (createUserDto.Roles != null && createUserDto.Roles.Any())
            {
                foreach (var roleName in createUserDto.Roles)
                {
                    var role = await _unitOfWork.Roles.GetByNameAsync(roleName);

                    if(role == null)
                    {
                        throw new ArgumentException($"Role \n{roleName}\n not found.");
                    }

                    user.UserRoles.Add(new UserRole { User = user, Role = role });
                }
            }

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreateAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(x => x.Role.Name).ToList()
            };
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
           var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null) return false;

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            return users.Select(x => new UserDto 
            { 
              Id = x.Id, 
              Username = x.UserName,
              Email = x.Email,
              CreatedAt = x.CreateAt,
              UpdatedAt = x.UpdatedAt,
              IsActive = x.IsActive,
              Roles = x.UserRoles.Select(r => r.Role.Name).ToList()

            });
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreateAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(x => x.Role.Name).ToList()
            };
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null) return null;

            var existingUserByUsername = await _unitOfWork.Users.GetByUsernameAsync(updateUserDto.UserName);

            if (existingUserByUsername != null && existingUserByUsername.Id != id)
            {
                throw new ArgumentException("Username already exists.");
            }

            var existingUserByEmail = await _unitOfWork.Users.GetByEmailAsync(updateUserDto.Email);

            if (existingUserByEmail != null && existingUserByEmail.Id != id) 
            {
                throw new ArgumentException("Email already exists.");
            }

            user.UserName = updateUserDto.UserName;
            user.Email = updateUserDto.Email;
            user.IsActive = updateUserDto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            user.UserRoles.Clear();
            if (updateUserDto.Roles != null && updateUserDto.Roles.Any())
            {

                foreach (var roleName in updateUserDto.Roles)
                {
                    var role = await _unitOfWork.Roles.GetByNameAsync(roleName);

                    if (role == null)
                    {
                        throw new ArgumentException($"Role \n{roleName}\n not found.");
                    }

                    user.UserRoles.Add(new UserRole { User = user, Role = role });
                }
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreateAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
        }
    }
}
