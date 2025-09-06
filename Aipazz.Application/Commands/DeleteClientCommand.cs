using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aipazz.Application.Commands
{
    public class DeleteClientCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;
    }
}
