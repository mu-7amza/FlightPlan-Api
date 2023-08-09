using FlightPlanApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Globalization;

namespace FlightPlanApi.data
 
{
    public class MongoDbDatabase : IDatabaseAdapter   
    {
       private IMongoCollection<BsonDocument> GetCollection(string databaseName , string collectionName)
        {
            var client = new MongoClient();
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<BsonDocument>(collectionName);
            return collection;
        }
        private FlightPlan ConvertBsonToFlightPlan(BsonDocument document) 
        {
            if (document == null) return null;
            return new FlightPlan
            {
                FlightPlanId = document["flight_plan_id"].AsString,
                Altitude = document["altitude"].AsInt32,
                AirSpeed = document["airspeed"].AsInt32,
                AircraftIdentification = document["aircraft_identification"].AsString,
                AircraftType = document["aircraft_type"].AsString,
                ArrivalAirport = document["arrival_airport"].AsString,
                FlightType = document["flight_type"].AsString,
                DepartureAirport = document["departing_airport"].AsString,
                Departuretime = document["departure_time"].AsBsonDateTime.ToUniversalTime(),
                Arrivaltime = document["estimated_arrival_time"].AsBsonDateTime.ToUniversalTime(),
                Route = document["route"].AsString,
                Remarks = document["remarks"].AsString,
                FuelHours = document["fuel_hours"].AsInt32,
                FuelMinutes = document["fuel_minutes"].AsInt32,
                NumberOnBoard = document["number_onboard"].AsInt32
            };
        }

        public async Task<List<FlightPlan>> GetAllFlightPlans()
        {
            var collection = GetCollection("airDb","flight_plan");
            var documents = collection.Find(_ => true).ToListAsync();

            var flightPlanList = new List<FlightPlan>();

            if (documents == null) return flightPlanList;

            foreach (var document in await documents)
            {
                flightPlanList.Add(ConvertBsonToFlightPlan(document));
            }
            return flightPlanList;

        }

        public async Task<FlightPlan> GetFlightPlanById(string flightPlanId)
        {
            var collection = GetCollection("airDb", "flight_plan");
            var flightPlanCursor = await collection.FindAsync(Builders<BsonDocument>.Filter.Eq("flight_plan_id",flightPlanId));
            var document = flightPlanCursor.FirstOrDefault();
            var flightPlan = ConvertBsonToFlightPlan(document);
            if (flightPlan == null)
            {
                return new FlightPlan();
            }
            return flightPlan;
        }

        public async Task<TransactionResult> FileFlightPlan(FlightPlan flightPlan)
        {
            var collection = GetCollection("airDb", "flight_plan");

            var document = new BsonDocument 
            {
                {"flight_plan_id", Guid.NewGuid().ToString("N")},
                {"aircraft_identification", flightPlan.AircraftIdentification },
                {"aircraft_type", flightPlan.AircraftType },
                {"airspeed", flightPlan.AirSpeed },
                {"altitude", flightPlan.Altitude },
                {"flight_type", flightPlan.FlightType },
                {"fuel_hours", flightPlan.FuelHours },
                {"fuel_minutes", flightPlan.FuelMinutes },
                {"departure_time", flightPlan.Departuretime },
                {"arrival_time", flightPlan.Arrivaltime } ,
                {"departing_airport", flightPlan.DepartureAirport } ,
                {"arrival_airport", flightPlan.ArrivalAirport } ,
                {"route", flightPlan.Route },
                {"remarks", flightPlan.Remarks },
                {"number_onboard" , flightPlan.NumberOnBoard }
            };

            try
            {
                await collection.InsertOneAsync(document);
                if (document["_id"].IsObjectId)
                {
                    return TransactionResult.Success;
                }
                return TransactionResult.BadRequest;
            }
            catch
            {
                return TransactionResult.ServerError;
            }

        }

        public async Task<TransactionResult> UpdateFlightPlan(string flightPlanId, FlightPlan flightPlan)
        {
            var collection = GetCollection("airDb", "flight_plan");
            var filter = Builders<BsonDocument>.Filter.Eq("flight_plan_id",flightPlanId);
            var update = Builders<BsonDocument>.Update.
                Set("aircraft_identification", flightPlan.AircraftIdentification)
                .Set("aircraft_type", flightPlan.AircraftType)
                .Set("airspeed", flightPlan.AirSpeed)
                .Set("altitude", flightPlan.Altitude)
                .Set("flight_type", flightPlan.FlightType)
                .Set("fuel_hours", flightPlan.FuelHours)
                .Set("fuel_minutes", flightPlan.FuelMinutes)
                .Set("departure_time", flightPlan.Departuretime)
                .Set("arrival_time", flightPlan.Arrivaltime)
                .Set("departing_airport", flightPlan.DepartureAirport)
                .Set("arrival_airport", flightPlan.ArrivalAirport)
                .Set("route", flightPlan.Route)
                .Set("remarks", flightPlan.Remarks)
                .Set("number_onboard", flightPlan.NumberOnBoard);
            var result = await collection.UpdateOneAsync(filter, update);

            if(result.MatchedCount == 0)
            {
                return TransactionResult.NotFound;
            }

            if(result.ModifiedCount > 0)
            {
                return TransactionResult.Success;
            }

            return TransactionResult.ServerError;


        }

        public async Task<bool> DeleteFlightPlanById(string flightPlanId)
        {
            var collection = GetCollection("airDb", "flight_plan");
            var result = await collection.DeleteOneAsync(Builders<BsonDocument>.Filter.Eq("flight_plan_id",flightPlanId));
            return result.DeletedCount > 0;


        }
    }
}
