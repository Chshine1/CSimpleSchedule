using AutoMapper;
using DiaryProject.Api.Context;
using DiaryProject.Shared.Dtos;

namespace DiaryProject.Api.Extension;

public class ServerMapperProfile : MapperConfigurationExpression
{
    public ServerMapperProfile()
    {
        CreateMap<Memo, MemoDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
    }
}