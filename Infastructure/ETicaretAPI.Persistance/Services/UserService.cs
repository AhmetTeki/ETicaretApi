using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserResponse> CreateAsync(CreateUser model)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Email = model.Email,
                NameSurname = model.NameSurname,

            }, model.Password);

            CreateUserResponse response = new() { Succeded = result.Succeeded };
            if (result.Succeeded)
            {
                response.Message = "kullanıcı başarılı ile oluşturulmuştur";
            }
            else
            {
                throw new UserCreatedFailedException();
            }
            return response;
        }

        public async Task UpdateRefreshToken(string refreshToken,AppUser user, DateTime accessTokenDate, int refreshTokenLifeTime)
        {
       
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate= accessTokenDate.AddMinutes(refreshTokenLifeTime);
                await _userManager.UpdateAsync(user);
            }
            else { throw new UserCreatedFailedException(); }
            
          
        }
    }
}
