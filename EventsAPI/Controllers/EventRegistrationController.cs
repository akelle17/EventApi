using EventsAPI.Data;
using EventsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventsAPI.Controllers
{
    public class EventRegistrationController : ControllerBase
    {
        private readonly EventsDataContext _context;
        private readonly EventRegistrationChannel _channel;

        public EventRegistrationController(EventsDataContext context, EventRegistrationChannel channel)
        {
            _context = context;
            _channel = channel;
        }

        [HttpPost("events/{eventId:int}/registrations")]
        public async Task<ActionResult> AddRegistration(int eventId, [FromBody] PostReservationRequest request)
        {
            var savedEvent = await _context.Events.SingleOrDefaultAsync(e => e.Id == eventId);
            if (savedEvent == null)
            {
                return NotFound();
            }

            EventRegistration registration = new()
            {
                EmployeedId = request.Id,
                Name = request.FirstName + " " + request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Status = EventRegistrationStatus.Pending
            };

            savedEvent.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            var worked = await _channel.AddRegistration(new EventRegistrationChannelRequest(registration.Id));

            return CreatedAtRoute("get-event-registration",
                new { eventId = savedEvent.Id, registrationId = registration.Id },
                registration);
        }

        [HttpGet("events/{eventId:int}/registrations/{registrationId:int}", Name = "get-event-registration")]
        public async Task<ActionResult> LookupRegistration(int eventId, int registrationId)
        {
            var response = await _context.Events
                .Where(e => e.Id == eventId)
                .Select(e => e.Registrations.Where(r => r.Id == registrationId)).SingleOrDefaultAsync();

            if (response == null)
            {
                return NotFound();
            } else
            {
                return Ok(response.First());
            }
        }

        [HttpGet("events/{eventId:int}/registrations")]
        public async Task<ActionResult> GetRegistrationsForEvent(int eventId)
        {
            return Ok();
        }
    }

    public record PostReservationRequest
    {
        public int Id { get; init; }

        public string FirstName { get; init; }
        public string LastName { get; init; }

        public string Email { get; init; }
        public string Phone { get; init; }
    }

    public record GetReservationResponse
    {
        public int Id { get; init; }
        public int EmployeeId { get; set; }
        public string FirstName { get; init; }
        public string LastName { get; init; }

        public string Email { get; init; }
        public string Phone { get; init; }
        public EventRegistration Status { get; set; }
        public string DeniedReason { get; set; }
    }
}
