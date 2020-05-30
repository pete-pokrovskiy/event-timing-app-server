using EventTiming.API.Contract;
using EventTiming.Domain;
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
        private readonly ICommandHandler<CreateEventCommand> _createEventCommand;
        private readonly IQueryHandler<GetEventQuery, GetEventQueryResult> _getEventQuery;

        public EventsController(ICommandHandler<CreateEventCommand> createEventCommand,
            IQueryHandler<GetEventQuery, GetEventQueryResult> getEventQuery)
        {
            _createEventCommand = createEventCommand;
            _getEventQuery = getEventQuery;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> Get(Guid eventId)
        {
            // TODO: not found result via global exception filter
            var result = await _getEventQuery.Execute(new GetEventQuery { EventId = eventId });
            return Ok(result.Event);
        }


        /// <summary>
        /// Создание анкетирования
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create([BindRequired][FromBody] EventDto @event)
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

        ///// <summary>
        ///// Обновление анетирования
        ///// </summary>
        ///// <returns></returns>
        //[HttpPut("{id}")]
        //public IActionResult Update(Guid id, [BindRequired][FromBody] AppraisalApiUpdateDto appraisalDto)
        //{
        //    if (appraisalDto == null || !ModelState.IsValid)
        //    {
        //        return BadRequest($"Некорректное входное сообщение. Подробности: {ModelStateHelper.GetErrors(ModelState)}");
        //    }

        //    var appraisal = _appraisalByFilterQuery.Execute(new AppraisalsByFilterQuery { AppraisalId = id }).Appraisals.FirstOrDefault();

        //    if (appraisal == null)
        //    {
        //        return NotFound();
        //    }

        //    var updateCommand = new UpdateAppraisalApiCommand { Appraisal = appraisalDto, AppraisalId = id };
        //    _updateAppraisalCommand.Execute(updateCommand);

        //    if (!updateCommand.ValidationResult.IsValid)
        //    {
        //        return BadRequest(updateCommand.ValidationResult.GetValidationErrors());
        //    }
        //    else
        //    {
        //        return NoContent();
        //    }

        //}

        ////Используем JSON Patch
        ////Content-Type: application/json-patch+json
        //[HttpPatch("{id}")]
        //public IActionResult PartialUpdate(Guid id, [BindRequired][FromBody] JsonPatchDocument<AppraisalApiUpdateDto> appraisalPatchDoc)
        //{
        //    if (appraisalPatchDoc == null || !ModelState.IsValid)
        //    {
        //        return BadRequest($"Некорректное входное сообщение. Подробности: {ModelStateHelper.GetErrors(ModelState)}");
        //    }

        //    var dbAppraisal = _appraisalByFilterQuery.Execute(new AppraisalsByFilterQuery { AppraisalId = id }).Appraisals.FirstOrDefault();

        //    if (dbAppraisal == null)
        //    {
        //        return NotFound();
        //    }

        //    //из объекта БД получаем dto
        //    var appraisalToPatch = _mapper.Map<AppraisalApiUpdateDto>(dbAppraisal);

        //    //применяем json patch
        //    appraisalPatchDoc.ApplyTo(appraisalToPatch);

        //    //отправляем полученный объект в команду
        //    //используем стандартную команду апдейта
        //    var commandData = new UpdateAppraisalApiCommand
        //    {
        //        AppraisalId = id,
        //        Appraisal = appraisalToPatch
        //    };

        //    _updateAppraisalCommand.Execute(commandData);

        //    if (!commandData.ValidationResult.IsValid)
        //    {
        //        return BadRequest(commandData.ValidationResult.GetValidationErrors());
        //    }
        //    else
        //    {
        //        return NoContent();
        //    }
        //}

        ///// <summary>
        ///// Удаление анкетировния
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpDelete("{id}")]
        //public IActionResult Delete(Guid? id)
        //{
        //    if (!id.HasValue)
        //    {
        //        return BadRequest("Не передан, либо некорректное значение идентификатора сущности");
        //    }

        //    var deleteCommand = new DeleteAppraisalApiCommand { AppraisalId = id.Value };
        //    _deleteAppraisalCommand.Execute(deleteCommand);

        //    if (!deleteCommand.ValidationResult.IsValid)
        //    {
        //        return BadRequest(deleteCommand.ValidationResult.GetValidationErrors());
        //    }
        //    else
        //    {
        //        return NoContent();
        //    }
        //}


        ///// <summary>
        ///// Получение всех анкетирований
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    var result = _getQuery.Execute(new GetAppraisalApiQuery());
        //    return Ok(result.Appraisals);
        //}

        ///// <summary>
        ///// Получение данных по идентификатору анкетирования
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet("{id}")]
        //public IActionResult Get(Guid? id)
        //{
        //    if (!id.HasValue)
        //    {
        //        return BadRequest("Не передан, либо некорректное значение идентификатора сущности");
        //    }

        //    //TODO: куда девать проверку на существование сущности?
        //    var resultRecord = _appraisalByFilterQuery.Execute(new AppraisalsByFilterQuery { AppraisalId = id }).Appraisals.FirstOrDefault();

        //    if (resultRecord == null)
        //    {
        //        return NotFound();
        //    }

        //    var result = _getQuery.Execute(new GetAppraisalApiQuery
        //    {
        //        AppraisalId = id.Value
        //    }).Appraisals.First();

        //    return Ok(result);
        //}
    }
}
