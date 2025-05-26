using AutoMapper;
using HomeManager.Data.Data.Dtos;
using HomeManager.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Services.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Home, HomeDto>();
            CreateMap<Conversation, ConversationDto>();
        }
    }
}
