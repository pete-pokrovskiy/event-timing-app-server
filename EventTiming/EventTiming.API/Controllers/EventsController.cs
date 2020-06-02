using AutoMapper;
using EventTiming.API.Contract;
using EventTiming.Domain;
using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Contract.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace EventTiming.API.Controllers
{
    [Authorize]
    [Route("/api/v1/events")]    
    public class EventsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICommandHandler<CreateEventCommand> _createEventCommand;
        private readonly ICommandHandler<UpdateEventCommand> _updateEventCommand;
        private readonly ICommandHandler<DeleteEventCommand> _deleteEventCommand;
        private readonly IQueryHandler<GetEventQuery, GetEventQueryResult> _getEventQuery;
        private readonly IQueryHandler<GetAllEventsQuery, GetAllEventsQueryResult> _getAllEventsQuery;

        public EventsController(
            IMapper mapper,
            ICommandHandler<CreateEventCommand> createEventCommand,
            ICommandHandler<UpdateEventCommand> updateEventCommand,
            ICommandHandler<DeleteEventCommand> deleteEventCommand,
            IQueryHandler<GetEventQuery, GetEventQueryResult> getEventQuery,
            IQueryHandler<GetAllEventsQuery, GetAllEventsQueryResult> getAllEventsQuery)
        {
            _mapper = mapper;
            _createEventCommand = createEventCommand;
            _updateEventCommand = updateEventCommand;
            _deleteEventCommand = deleteEventCommand;
            _getEventQuery = getEventQuery;
            _getAllEventsQuery = getAllEventsQuery;
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult> Get(Guid eventId)
        {
            // TODO: not found result via global exception filter
            var result = await _getEventQuery.Execute(new GetEventQuery { EventId = eventId });
            return Ok(result.Event);
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _getAllEventsQuery.Execute(new GetAllEventsQuery());
            return Ok(result.Events);
        }


        /// <summary>
        /// Создание анкетирования
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([BindRequired][FromBody] EventInput @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Некорректное входное сообщение. Подробности: {ModelStateHelper.GetErrors(ModelState)}");
            }

            var createEventCommand = new CreateEventCommand
            {
                Name = @event.Name,
                Description = @event.Description,
                StartDateAndTime = @event.StartDateAndTime
            };

            await _createEventCommand.Execute(createEventCommand);

            // TODO: failed validation via global exception filter

            //if (!createCommand.ValidationResult.IsValid)
            //{
            //    return BadRequest(createCommand.ValidationResult.GetValidationErrors());
            //}
            //else
            //{
            return Created($"/api/v1/events/{createEventCommand.Id.Value}", new { id = createEventCommand.Id.Value });
            //}
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [BindRequired][FromBody] EventInput input)
        {
            if (input == null || !ModelState.IsValid)
            {
                return BadRequest($"Некорректное входное сообщение. Подробности: {ModelStateHelper.GetErrors(ModelState)}");
            }

            await _updateEventCommand.Execute(new UpdateEventCommand
            {
                EventId = id,
                Event = _mapper.Map<EventDto>(input)
            });

            return Ok();
           
        }

        [HttpDelete("{eventId}")]
        public async Task<ActionResult> Delete(Guid eventId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Некорректное входное сообщение. Подробности: {ModelStateHelper.GetErrors(ModelState)}");
            }

            await _deleteEventCommand.Execute(new DeleteEventCommand
            {
                EventId = eventId
            });

            return Ok();
        }
    }
}
