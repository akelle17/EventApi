using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventsAPI.Data
{
    public enum EventRegistrationStatus { Pending, Approved, Denied }
    public class EventRegistration
    {
        public int Id { get; set; }
        public int EmployeedId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public EventRegistrationStatus Status { get; set; }
        public string ReasonForDenial { get; set; }
    }
}
