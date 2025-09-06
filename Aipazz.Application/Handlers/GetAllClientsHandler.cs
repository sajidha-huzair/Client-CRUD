using Aipazz.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aipazz.Application.Queries;

namespace Aipazz.Application.Handlers
{
    public class GetAllClientsHandler : IRequestHandler<GetAllClientsQuery, List<Client>>
    {
        private readonly IClientRepository _repository;

        public GetAllClientsHandler(IClientRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Client>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllClients();
        }
    }
}
