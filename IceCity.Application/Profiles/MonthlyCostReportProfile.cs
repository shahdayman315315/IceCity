using AutoMapper;
using IceCity.Application.Dtos;
using IceCity.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCity.Application.Profiles
{
    public class MonthlyCostReportProfile:Profile
    {
        public MonthlyCostReportProfile()
        {
            CreateMap<MonthlyCostReport, MonthlyReportDto>().ReverseMap();
        }
    }
}
