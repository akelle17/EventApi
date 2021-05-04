using EventsAPI.Data;
using EventsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventsAPI.Controllers
{
    [Route("events")]
    public class EventsController : ControllerBase
    {
        private readonly EventsDataContext _context;
        private readonly ILookupEmployees _employeeService;

        public EventsController(EventsDataContext context, ILookupEmployees employeeService)
        {
            _context = context;
            _employeeService = employeeService;
        }

        [HttpPost]
        [ResponseCache(Location =ResponseCacheLocation.Any, Duration = 10)]
        public async Task<ActionResult> AddEvent([FromBody] PostEventRequest request)
        {
            if (ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } else
            {
                var eventToAdd = new Event()
                {
                    Name = request.Name,
                    HostedBy = request.HostedBy,
                    LongDescription = request.LongDescription,
                    StartDateAndTime = request.StartDateAndTime,
                    EndDateAndTime = request.EndDateAndTime
                    
                };
                _context.Events.Add(eventToAdd);
                await _context.SaveChangesAsync();
                return CreatedAtRoute("get-event-by-id", new { id = eventToAdd.Id }, eventToAdd);
            }
        }

        [HttpGet("{id:int}", Name = "get-event-by-id")]
        public async Task<ActionResult> GetById(int id)
        {
            return Ok();
        }

        [HttpGet("{id:int}/participants")]
        public async Task<ActionResult> GetParticipantsForEvent(int id)
        {
            var savedEvent = await _context.Events
                .Where(e => e.Id == id)
                .Select(e => new { Id = e.Id, Partipants = e.Participants
                    .Select(p => new GetParticipantResponse(p.Id, p.Name, p.EMail, p.Phone))
                })
                .SingleOrDefaultAsync();

            if (savedEvent == null)
            {
                return NotFound();
            }

            var participants = await _context.Events
                .Where(e => e.Id == id)
                .Select(e => e.Participants)
                .ToListAsync();

            return Ok(participants);
        }

        [HttpPost("{id:int}/participants")]
        public async Task<ActionResult> AddParticipant([FromBody] PostParticipantRequest request)
        {
            var savedEvent = await _context.Events.SingleOrDefaultAsync(e => e.Id == request.Id);

            if (savedEvent == null)
            {
                return NotFound();
            }

            EventParticipant participant = new()
            {
                EmployeeId = request.Id,
                Name = request.FirstName + " " + request.LastName,
                EMail = request.Email,
                Phone = request.Phone
            };

            if (savedEvent.Participants == null)
            {
                savedEvent.Participants = new List<EventParticipant>();
            }

            savedEvent.Participants.Add(participant);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] bool showPast = false)
        {
            var details = await _context.Events
                .Where(e => e.EndDateAndTime.Date > DateTime.Now.Date)
                .Select(e => new GetEventsResponseItem(e.Id, e.Name, e.StartDateAndTime, e.EndDateAndTime, e.Participants.Count()))
                .ToListAsync();

            return Ok(new GetResponse<GetEventsResponseItem>(details));

        }

        public record GetResponse<T>(IList<T> Data);

        public record GetEventsResponseItem(int Id, string Name, DateTime StartDate, DateTime EndDate, int NumberOfParticipants);

        public record PostEventRequest(
            [Required]
            string Name,
            string LongDescription,
            [Required]
            string HostedBy,
            DateTime StartDateAndTime,
            DateTime EndDateAndTime
        );


        public record GetParticipantResponse
        (
            int Id,
            string Name,
            string Email,
            string Phone
        );

        public record PostParticipantRequest
        {
            public int Id { get; init; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public string Department { get; init; }
            public int Salary { get; init; }
            public string Email { get; init; }
            public string Phone { get; init; }
        }
    }
}

