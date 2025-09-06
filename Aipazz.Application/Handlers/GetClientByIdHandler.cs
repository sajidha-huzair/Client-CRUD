using Aipazz.Application.Queries;
using Aipazz.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aipazz.Application.Handlers
{
    public class GetClientByIdHandler : IRequestHandler<GetClientByIdQuery, Client>
    {
        private readonly IClientRepository _repository;

        public GetClientByIdHandler(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<Client> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetClientById(request.Id);
        }
    }
}
