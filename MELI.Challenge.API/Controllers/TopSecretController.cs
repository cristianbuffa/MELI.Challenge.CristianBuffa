using AutoMapper;
using MELI.Challenge.API.Model;
using MELI.Challenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MELI.Desafio.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MELI.Challenge.API.Controllers
{

    [ApiController]
    public class TopSecretController : ControllerBase
    {
        private const string KENOBI = "kenobi";
        private const string SKYWALKER = "skywalker";
        private const string SATO = "sato";

        private readonly ICommunicationService service;
        private readonly IMapper mapper;
        private readonly IConfiguration Configuration;
        private readonly IMemoryCache memoryCache;

        public TopSecretController(ICommunicationService service, IMapper mapper,
            IConfiguration configuration, IMemoryCache memoryCache)
        {
            this.service = service;
            this.mapper = mapper;
            this.Configuration = configuration;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("/TopSecret/{Satellite}")]
        public ActionResult Get([FromRoute] string Satellite)
        {
            try
            {
                var distances = ValidateDistances();
                var messages = ValidateMessages();
                var coordinates = this.service.GetLocation(distances.ToList());
                var message = this.service.GetMessage(messages.ToList());

                return Ok(new TopSecretResponse()
                {
                    Position = new Position { X = coordinates.X, Y = coordinates.Y },
                    Message = message
                });
            }

            catch (Services.NotEnoughInfoException ex)
            {
                return NotFound(ex.Message);
            }

            catch (Services.PositionException ex)
            {
                return NotFound(ex.Message);
            }

            catch (Services.MessageException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Route("/TopSecret/{Satellite}")]
        public ActionResult Post([FromBody] SatelliteGettingData body, [FromRoute] string Satellite)
        {
            try
            {
                memoryCache.Set(Satellite,  new CacheValue { Distance = body.distance.ToString(), message = body.message });
                var distances = ValidateDistances();
                var messages = ValidateMessages();
                var coordinates = this.service.GetLocation(distances.ToList());
                var message = this.service.GetMessage(messages.ToList());

                return Ok(new TopSecretResponse()
                {
                    Position = new Position { X = coordinates.X , Y = coordinates.Y},
                    Message = message
                });
            }
            catch (Services.NotEnoughInfoException ex)
            {
                return NotFound(ex.Message);
            }

            catch (Services.PositionException ex)
            {
                return NotFound(ex.Message);
            }

            catch (Services.MessageException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private IEnumerable<Domain.SatelliteGettingDistance> ValidateDistances()
        {
            CacheValue cacheValue;
            var response = new List<Domain.SatelliteGettingDistance>();

            memoryCache.TryGetValue("kenobi", out cacheValue);
            if (cacheValue.Distance == "")
                throw new NotEnoughInfoException();
            response.Add(new Domain.SatelliteGettingDistance { name = KENOBI
                                                , distance = float.Parse(cacheValue.Distance)});
            memoryCache.TryGetValue("skywalker", out cacheValue);
            if (cacheValue.Distance == "")
                throw new NotEnoughInfoException();
            response.Add(new Domain.SatelliteGettingDistance { name = SKYWALKER
                                                , distance = float.Parse(cacheValue.Distance) });
            memoryCache.TryGetValue("sato", out cacheValue);
            if (cacheValue.Distance == "")
                throw new NotEnoughInfoException();
            response.Add(new Domain.SatelliteGettingDistance { name = SATO
                                            , distance = float.Parse(cacheValue.Distance) });

            return response;
        }

        private IEnumerable<Domain.SatelliteGettingMessage> ValidateMessages()
        {
            CacheValue cacheValue;
            var response = new List<Domain.SatelliteGettingMessage>();

            memoryCache.TryGetValue(KENOBI, out cacheValue);
            if (cacheValue == null)
                throw new NotEnoughInfoException();
            response.Add(new Domain.SatelliteGettingMessage
            {
                name = KENOBI,
                Message = cacheValue.message
            });
            memoryCache.TryGetValue(SKYWALKER, out cacheValue);
            if (cacheValue == null)
                throw new NotEnoughInfoException();
            response.Add(new Domain.SatelliteGettingMessage
            {
                name = SKYWALKER,
                Message = cacheValue.message
            });
            memoryCache.TryGetValue(SATO, out cacheValue);
            if (cacheValue == null)
                throw new NotEnoughInfoException();
            response.Add(new Domain.SatelliteGettingMessage
            {
                name = SATO,
                Message = cacheValue.message
            });

            return response;
        }

        [HttpPost]
        [Route("/TopSecret")]
        public ActionResult Post([FromBody] List<SatelliteGettingData> body)
        {
            try
            {
                var distances = this.mapper.Map<IEnumerable<Domain.SatelliteGettingDistance>>(body);
                var messages = this.mapper.Map<IEnumerable<Domain.SatelliteGettingMessage>>(body);
                var coordinates = this.service.GetLocation(distances.ToList());
                var message = this.service.GetMessage(messages.ToList());

                return Ok(new TopSecretResponse() { Position = new Position { X = coordinates.X, Y = coordinates.Y }
                                               , Message = message });
            }
            catch (Services.PositionException pex)
            {
                return NotFound(pex.Message);
            }
            catch (Services.MessageException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
