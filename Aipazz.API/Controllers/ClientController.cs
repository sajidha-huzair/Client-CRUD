using Aipazz.Application;
using Aipazz.Application.Commands;
using Aipazz.Application.Queries;
using Aipazz.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aipazz.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IMediator _mediator;  //Declare IMediator

        // Inject MediatR via Constructor
        public ClientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Client
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllClientsQuery());
            return Ok(result);
        }

        // GET: api/Client/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetClientByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/Client
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientCommand command)
        {
            if (command == null) return BadRequest("Invalid request.");
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { Id = result.Id }, result);
        }

        // PUT: api/Client/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateClientCommand command)
        {
            if (command == null) return BadRequest("Invalid request.");
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // DELETE: api/Client/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _mediator.Send(new DeleteClientCommand { Id = id });
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
