using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UserCases.V1.Product;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<Product, Response.ProductResponse>();
    }
}
