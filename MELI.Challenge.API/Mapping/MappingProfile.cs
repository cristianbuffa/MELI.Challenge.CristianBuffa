using System;
using AutoMapper;
using MELI.Challenge.API.Model;
using MELI.Challenge.Domain;

namespace MELI.Challenge.API.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            AddApiToDomainMapping();
        }

        private void AddDomainToApiModelMapping()
        {
        }

        /// <summary>
        /// Mappings Enums From Api to Domain
        /// </summary>
        private void AddApiToDomainMapping()
        {
            CreateMap<Model.SatelliteGettingData, Domain.SatelliteGettingDistance>();
            CreateMap<Model.SatelliteGettingData, Domain.SatelliteGettingMessage>();
        }
    }

}
