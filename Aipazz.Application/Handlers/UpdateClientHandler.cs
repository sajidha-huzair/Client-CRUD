using Aipazz.Application.Commands;
using Aipazz.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aipazz.Application.Handlers
{
    public class UpdateClientHandler : IRequestHandler<UpdateClientCommand, Client>
    {
        private readonly IClientRepository _repository;

        public UpdateClientHandler(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<Client> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            var client = await _repository.GetClientById(request.Id);
            if (client == null)
            {
                throw new KeyNotFoundException($"Client with Id {request.Id} not found.");
            }

            // Update properties
            client.Name = request.Name;
            client.Age = request.Age;
            client.Gender = request.Gender;
            client.Email = request.Email;
            client.Contact = request.Contact;

            await _repository.UpdateClient(client);
            return client;
        }
    }
}
