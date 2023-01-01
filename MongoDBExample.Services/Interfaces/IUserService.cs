﻿using MongoDBExample.Dto.Request;
using MongoDBExample.Dto.Response;

namespace MongoDBExample.Services.Interfaces;

public interface IUserService
{
    Task<BaseResponseGeneric<string>> RegisterAsync(RegisterUserDtoRequest request);

    Task<LoginDtoResponse> LoginAsync(LoginDtoRequest request);

    Task<BaseResponse> RequestTokenToResetPasswordAsync(DtoRequestPassword request);

    Task<BaseResponse> ResetPasswordAsync(DtoResetPassword request);

    Task<BaseResponse> ChangePasswordAsync(DtoChangePassword request);
}