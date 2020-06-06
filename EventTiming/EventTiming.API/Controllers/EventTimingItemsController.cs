using AutoMapper;
using EventTiming.API.Contract;
using EventTiming.Logic.Contract.Dto;
using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Contract.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventTiming.API.Controllers
{
    [Authorize]
    [Route("/api/v1/events/{eventId}/items")]
    public class EventTimingItemsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICommandHandler<CreateTimingItemCommand> _createTimingItemCommand;
        private readonly ICommandHandler<UpdateTimingItemCommand> _updateTimingItemCommand;
        private readonly IQueryHandler<GetEventTimingItemsQuery, GetEventTimingItemsQueryResult> _getEventTimingItemsQuery;

        public EventTimingItemsController(IMapper mapper,
            ICommandHandler<CreateTimingItemCommand> createTimingItemCommand,
            ICommandHandler<UpdateTimingItemCommand> updateTimingItemCommand,
            IQueryHandler<GetEventTimingItemsQuery, GetEventTimingItemsQueryResult> getEventTimingItemsQuery
            )
        {
            _mapper = mapper;
            _createTimingItemCommand = createTimingItemCommand;
            _updateTimingItemCommand = updateTimingItemCommand;
            _getEventTimingItemsQuery = getEventTimingItemsQuery;
        }

       [HttpGet("{itemId}")]
        public async Task<ActionResult> Get(Guid eventId, Guid itemId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Некорректное входное сообщение. Подробности: {ModelStateHelper.GetErrors(ModelState)}");
            }

            var result = await _getEventTimingItemsQuery.Execute(new GetEventTimingItemsQuery
            {
                EventId = eventId,
                Id = itemId
            });

            if (!result.TimingItems.Any())
            {
                return NotFound();
            }

            return Ok(result.TimingItems.First());
        }

        [HttpGet]
        public async Task<ActionResult> GetAllByEvent(Guid eventId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Некорректное входное сообщение. Подробности: {ModelStateHelper.GetErrors(ModelState)}");
            }

            var result = await _getEventTimingItemsQuery.Execute(new GetEventTimingItemsQuery
            {
                EventId = eventId
            });

            return Ok(result.TimingItems);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Guid eventId,[BindRequired][FromBody] EventTimingItemInput input)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest($"Некорректное входное сообщение. Подробности: {ModelStateHelper.GetErrors(ModelState)}");
            }

            var createTimingItemCommand = new CreateTimingItemCommand
            {

                EventId = eventId,
                TimingItem = _mapper.Map<EventTimingItemDto>(input)
            };

            await _createTimingItemCommand.Execute(createTimingItemCommand);

            return Created($"/api/v1/events/{eventId}/items/{createTimingItemCommand.TimingItem.Id}", new { id = createTimingItemCommand.TimingItem.Id });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid eventId, Guid id, [BindRequired][FromBody] EventTimingItemInput input)
        {
            if (input == null || !ModelState.IsValid)
            {
                return BadRequest($"Некорректное входное сообщение. Подробности: {ModelStateHelper.GetErrors(ModelState)}");
            }

            await _updateTimingItemCommand.Execute(new UpdateTimingItemCommand
            {
                EventId = eventId,
                TimingItemId = id,
                TimingItem = _mapper.Map<EventTimingItemDto>(input)
            });

            return Ok();

        }
    }
}
