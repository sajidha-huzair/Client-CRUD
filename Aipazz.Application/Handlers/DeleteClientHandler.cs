using Aipazz.Application.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aipazz.Application.Handlers
{
    public class DeleteClientHandler : IRequestHandler<DeleteClientCommand, bool>
    {
        private readonly IClientRepository _repository;

        public DeleteClientHandler(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var client = await _repository.GetClientById(request.Id);
            if (client == null)
            {
                throw new KeyNotFoundException($"Client with Id {request.Id} not found.");
            }

            await _repository.DeleteClient(request.Id);
            return true;
        }
    }
}
