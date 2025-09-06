using Aipazz.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aipazz.Application.Commands;

namespace Aipazz.Application.Handlers
{
    public class CreateClientHandler : IRequestHandler<CreateClientCommand, Client>
    {
        private readonly IClientRepository _repository;

        public CreateClientHandler(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<Client> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var client = new Client
            {
                Id = request.Id,
                Name = request.Name,
                Age = request.Age,
                Gender = request.Gender,
                Email = request.Email,
                Contact = request.Contact
            };

            await _repository.AddClient(client);
            return client;
        }
    }
}
