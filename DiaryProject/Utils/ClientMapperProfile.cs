using AutoMapper;
using DiaryProject.Models;
using DiaryProject.Shared.Dtos;

namespace DiaryProject.Utils;

public class ClientMapperProfile : MapperConfigurationExpression
{
    public ClientMapperProfile()
    {
        CreateMap<MemoDto, MemoRecord>().ReverseMap();
    }
}