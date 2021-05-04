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

        [HttpGet("{id:int}/participants/{employeeId:int}")]
        public async Task<ActionResult> GetParticipantForEvent(int id, int employeeId)
        {

            // make sure that event exists, and that employee id registered for that event.
            return Redirect("http://localhost:1337/employees/" + employeeId);
        }

        // [HttpPut("{id:int}/participants/{participantId:int}")]


        [HttpPost("{id:int}/participants")]
        public async Task<ActionResult> AddParticipant(int id, [FromBody] PostParticipantRequest request)
        {
            // Validate it -- elided for class.
            // make sure there is event with that id.
            var savedEvent = await _context.Events.SingleOrDefaultAsync(e => e.Id == id);
            if (savedEvent == null)
            {
                return NotFound("No Event with that Id");
            }

            bool employeeIsActive = await _employeeService.CheckEmployeeIsActive(request.Id);

            if (!employeeIsActive)
            {
                return BadRequest("That employee is no longer active.");
            }

            // TODO: What if they are registered already? 
            //  - return a 400. Just saying "Nope".
            //  - return a conflict - this is saying "this thing conflicts with something else"
            //  - you could update the data with the request... maybe they want to use a different email address.
            //  - if they just posted twice (that kinda thing happens in the weird WWW)
            //  - return a redirect to their registration.
            // add a participant using the data from the request.
            //EventParticipant participant = new EventParticipant();
            //var particpant = new EventParticipant();
            EventParticipant participant = new()
            {
                EmployeeId = request.Id,
                Name = request.FirstName + " " + request.LastName,
                EMail = request.Email,
                Phone = request.Phone
            };

            // just ask the other API, does this employee exist?
            //  -- 
            // save it.
            if (savedEvent.Participants == null)
            {
                savedEvent.Participants = new List<EventParticipant>();
            }
            savedEvent.Participants.Add(participant);
            await _context.SaveChangesAsync();
            // return something... CreatedAtRoute
            return Ok();
        }

        [HttpGet("{id:int}/participants")] // GET /events/1/partipants
        public async Task<ActionResult> GetPartipantsForEvent(int id)
        {

            var data = await _context.Events
                .Where(e => e.Id == id)
                .Select(e => new
                {
                    e.Id,
                    Participants = e.Participants
                    .Select(p => new GetParticipantResponse(p.Id, p.Name, p.EMail, p.Phone))
                })
                .SingleOrDefaultAsync();

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(new { data = data.Participants });
            }




        }


        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 10)]
        public async Task<ActionResult> AddEvent([FromBody] PostEventRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                // Add it to the database
                // Return a 201 with a Location  Header (hard to do right now... but we will)
                var eventToAdd = new Event()
                {
                    Name = request.Name,
                    HostedBy = request.HostedBy,
                    LongDescription = request.LongDescription,
                    StartDateAndTime = request.StartDateAndTime.Value,
                    EndDateAndTime = request.EndDateAndTime.Value
                };
                _context.Events.Add(eventToAdd);
                await _context.SaveChangesAsync();
                //return Created()
                //var url = Url.Action(nameof(EventsController.GetById), nameof(EventsController), new { id = eventToAdd.Id });
                //return Created(url, eventToAdd); 
                return CreatedAtRoute("get-event-by-id", new { id = eventToAdd.Id }, eventToAdd); // this is a bit wrong.. stay with me.
            }

        }

        [HttpGet("{id:int}", Name = "get-event-by-id")]
        public async Task<ActionResult> GetById(int id)
        {
            return Ok();
        }


        [HttpGet] // GET /events, GET /events?showPast=true
        public async Task<ActionResult> Get([FromQuery] bool showPast = false)
        {

            var details = await _context.Events
                .Where(e => e.EndDateAndTime.Date > DateTime.Now.Date)
                .Select(e => new GetEventsResponseItem(e.Id, e.Name, e.StartDateAndTime, e.EndDateAndTime, e.Participants.Count()))
                .ToListAsync();

            return Ok(new GetResponse<GetEventsResponseItem>(details));
        }
    }

    public record GetResponse<T>(IList<T> Data);

    public record GetEventsResponseItem(int Id, string Name, DateTime StartDate, DateTime EndDate, int NumberOfParticipants);

    public record PostEventRequest(
        [Required]
        string Name,
        string LongDescription,
        [Required]
        string HostedBy,
        [Required]
        DateTime? StartDateAndTime,
        [Required]
        DateTime? EndDateAndTime
        );


    public record GetParticipantResponse(
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

        public string Email { get; init; }
        public string Phone { get; init; }
    }


}
