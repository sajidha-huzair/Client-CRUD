using Aipazz.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aipazz.Application.Queries
{
    public class GetClientByIdQuery : IRequest<Client>
    {
        public string Id { get; set; }

        public GetClientByIdQuery(string id)
        {
            Id = id;
        }
    }
}
