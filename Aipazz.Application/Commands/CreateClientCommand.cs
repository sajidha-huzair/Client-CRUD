using Aipazz.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aipazz.Application.Commands
{
    public record CreateClientCommand(string Id, string Name, int Age, string Gender, string Email, string Contact) : IRequest<Client>;
    
}
