using Aipazz.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Aipazz.Application.Queries
{
    public record GetAllClientsQuery() : IRequest<List<Client>>;
}
