using System;
using System.Collections.Generic;
using System.Linq;
using MELI.Challenge.Domain;
using GeometricsShapes.Calculations;
using Microsoft.Extensions.Options;

namespace MELI.Challenge.Services
{
    public interface ICommunicationService
    {
        Coordinates GetLocation(List<SatelliteGettingDistance> distances);
        string GetMessage(List<SatelliteGettingMessage> satelliteGettingMessage);
    }

    public class CommunicationService: ICommunicationService
    {
        private SatelliteOptions satelliteOptions;

        private const string KENOBI = "kenobi";
        private const string SKYWALKER = "skywalker";
        private const string SATO = "sato";
        private const string EMPTY = "";
        private const int MAXAMOUNTOFWORDS = 5;


        public CommunicationService(IOptionsSnapshot<SatelliteOptions> satelliteOptions)
        {
            this.satelliteOptions = satelliteOptions.Value;
        }

        public Coordinates GetLocation(List<SatelliteGettingDistance> satelliteGettingDistances)
        {
            try
            { 
            //Validate there are three satelites.
            if (satelliteGettingDistances.Count() != 3)
                throw new PositionException();
            //Instance calculates library.
            var coordinatesDiscovery = new Calculations();

                var enemyCoordinates = coordinatesDiscovery.GetXYbyCircunsferencesIntersection(
                GetVariablesBySatellite(GetSatelliteGettingDataByName(satelliteGettingDistances, KENOBI))
                ,GetVariablesBySatellite(GetSatelliteGettingDataByName(satelliteGettingDistances, SKYWALKER))
                ,GetVariablesBySatellite(GetSatelliteGettingDataByName(satelliteGettingDistances, SATO)));

                if (enemyCoordinates == null)
                    throw new PositionException();

                return enemyCoordinates; 
            }
            catch (Exception)
            {
                throw new PositionException();
            }            
        }

        public string  GetMessage(List<SatelliteGettingMessage> satelliteGettingMessages)
        {
            var responseMessage = new string[MAXAMOUNTOFWORDS];

            satelliteGettingMessages.ForEach(m => ValidateAndInsertWordsToMessage(m.Message, ref responseMessage));
            ValidateMessage(responseMessage);
            return string.Join(" ", responseMessage);
        }

        private void ValidateAndInsertWordsToMessage(string[] SatelliteGettingMessage, ref string[] responseMessage)
        {
            var words = SatelliteGettingMessage.ToList();

            for (int i = 0; i < words.Count; i++)
            {
                if(words[i] != EMPTY)
                responseMessage[i] =  words[i];
            }
        }

        private void ValidateMessage(string[] responseMessage)
        {
            var message = responseMessage.ToList();
            if (message.Where(w => w == null).ToList().Count > 0)
                throw new MessageException();
        }

        private Circumference GetVariablesBySatellite(SatelliteGettingDistance satelliteGettingData)
        {
            var satelliteName = satelliteGettingData.name;
            var satelliteOption = GetSatelliteOptionByName(satelliteName);

            var circunsference = new Circumference(satelliteGettingData.distance, satelliteName
                , new Coordinates() { X = satelliteOption.X, Y = satelliteOption.Y });

            return circunsference;
        }
        private SatelliteGettingDistance GetSatelliteGettingDataByName(List<SatelliteGettingDistance> satellitesGettingDistances, string name)
        {
            return satellitesGettingDistances.SingleOrDefault<SatelliteGettingDistance>(s => s.name == name);
        }

        private Satellite GetSatelliteOptionByName(string name)
        {
            return this.satelliteOptions.Satellites.SingleOrDefault<Satellite>(s => s.Name == name);
        }
    }
}
