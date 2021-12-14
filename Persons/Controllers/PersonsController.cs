using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatBackend.Persons.Domain;
using ChatBackend.Persons.Domain.Operations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatBackend.Persons.Controllers
{
    /// <summary>
    /// Контроллер военнослужащих
    /// </summary>
    [ApiController]
    [Route("persons")]
    [Authorize]
    public sealed class PersonsController : Controller
    {
        private readonly Persons.Domain.Persons _persons;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="personsRealTimeService"></param>
        /// <param name="personsRepository"></param>
        public PersonsController(
            IPersonsRealTimeService personsRealTimeService, 
            IPersonsRepository personsRepository)
        {
            _persons = new Persons.Domain.Persons(personsRepository, personsRealTimeService);
        }

        /// <summary>
        /// Возвращает всех военнослужащих, пользующихся чатом
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Person>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersonsAsync(CancellationToken cancellationToken) => 
            Ok(await _persons.GetAsync(cancellationToken));
        
        /// <summary>
        /// Возвращает военнослужащего, пользующегося чатом, с указанным Guid
        /// </summary>
        [HttpGet("by-guid")]
        [ProducesResponseType(typeof(IEnumerable<Person>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersonByGuidAsync(Guid guid, CancellationToken cancellationToken)
        {
            if (!await _persons.DoesGuidExistAsync(guid, cancellationToken))
                return NotFound(guid);
            return Ok(await _persons.GetByGuidAsync(guid, cancellationToken));
        }
        
        /// <summary>
        /// Создаёт запись о военнослужащем, пользующемся чатом
        /// Доступно только администраторам
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(Person), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IEnumerable<ValidationFailure>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Person>>> CreatePersonAsync(
            CreatePerson createPerson,
            CancellationToken cancellationToken)
        {
            try
            {
                var person = await _persons.CreateAsync(createPerson, cancellationToken);
                return Created(person.Guid.ToString(), person);
            }
            catch (ValidationException validationException)
            {
                return BadRequest(validationException.Errors);
            }
        }
        
        /// <summary>
        /// Возвращает все воинские звания
        /// </summary>
        [HttpGet("ranks")]
        [ProducesResponseType(typeof(MilitaryRank[]), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public ActionResult<MilitaryRank[]> GetMilitaryRanks() => Ok(MilitaryRank.GetAll());
    }
}