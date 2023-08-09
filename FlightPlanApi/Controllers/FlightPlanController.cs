using FlightPlanApi.data;
using FlightPlanApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace FlightPlanApi.Controllers
{
    [Route("api/v1/flightplan")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private readonly IDatabaseAdapter _database;

        public FlightPlanController(IDatabaseAdapter database)
        {
            _database = database;
        }

        [HttpGet]
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "No Flight Plans Yet")]
        public async Task<IActionResult> GetFlightList()
        {
            var flightPlanList = await _database.GetAllFlightPlans();
            if(flightPlanList.Count == 0)
            {
                return NoContent();
            }

            return Ok(flightPlanList);
        }

        [HttpGet]
        [Route("{flightPlanId}")]
        [Authorize]
        public async Task<IActionResult> GetFlightPlanById(string flightPlanId)
        {
            var flightPlan = await _database.GetFlightPlanById(flightPlanId);
            if(flightPlan.FlightPlanId != flightPlanId)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return Ok(flightPlan);
        }

        [HttpPost]
        [Route("file")]
        [Authorize]
        public async Task<IActionResult> FileFlightPlan(FlightPlan flightPlan)
        {
            var transacctionResult = await _database.FileFlightPlan(flightPlan);
            switch(transacctionResult)
            {
                case TransactionResult.Success:
                    return Ok();
                case TransactionResult.BadRequest:
                    return StatusCode(StatusCodes.Status400BadRequest);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateFlightPlan(FlightPlan flightPlan)
        {
            var upateResult = await _database.UpdateFlightPlan(flightPlan.FlightPlanId,flightPlan);
            switch (upateResult)
            {
                case TransactionResult.Success:
                    return Ok();
                case TransactionResult.NotFound:
                    return StatusCode(StatusCodes.Status404NotFound);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpDelete]
        [Route("{flightPlanId}")]
        [Authorize]
        public async Task<IActionResult> DeleteFlightPlan(string flightPlanId)
        {
            var result = await _database.DeleteFlightPlanById(flightPlanId);

            if (result)
            {
                return Ok();
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpGet]
        [Route("airport/departure/{flightPlanId}")]
        [Authorize]
        public async Task<IActionResult> GetFlightPlanDepartureAirport(string flightPlanId)
        {
            var flightPlan = await _database.GetFlightPlanById(flightPlanId);

            if (flightPlan == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return Ok(flightPlan.DepartureAirport);
        }

        [HttpGet]
        [Route("route/{flightPlanId}")]
        [Authorize]
        public async Task<IActionResult> GetFlightPlanRoute(string flightPlanId)
        {
            var flightPlan = await _database.GetFlightPlanById(flightPlanId);

            if (flightPlan == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return Ok(flightPlan.Route);
        }

        [HttpGet]
        [Route("time/enroute/{flightPlanId}")]
        [Authorize]
        public async Task<IActionResult> GetFlightPlanTimeEnroute(string flightPlanId)
        {
            var flightPlan = await _database.GetFlightPlanById(flightPlanId);

            if (flightPlan == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            var estimatedtime = flightPlan.Arrivaltime - flightPlan.Departuretime;

            return Ok(estimatedtime);
        }


    }
}
