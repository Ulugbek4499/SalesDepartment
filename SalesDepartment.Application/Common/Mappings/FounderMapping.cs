using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SalesDepartment.Application.UseCases.Founders.Commands.CreateFounder;
using SalesDepartment.Application.UseCases.Founders.Commands.DeleteFounder;
using SalesDepartment.Application.UseCases.Founders.Commands.UpdateFounder;
using SalesDepartment.Application.UseCases.Founders.Response;
using SalesDepartment.Domain.Entities;

namespace SalesDepartment.Application.Common.Mappings
{
    public class FounderMapping : Profile
    {
        public FounderMapping()
        {
            CreateMap<CreateFounderCommand, Founder>().ReverseMap();
            CreateMap<DeleteFounderCommand, Founder>().ReverseMap();
            CreateMap<UpdateFounderCommand, Founder>().ReverseMap();
            CreateMap<FounderResponse, Founder>().ReverseMap();
        }
    }
}
