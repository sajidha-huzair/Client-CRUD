using Aipazz.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aipazz.Application
{
    public interface IClientRepository
    {
        Task<List<Client>> GetAllClients();
        Task<Client> GetClientById(string id);
        Task AddClient(Client client);
        Task UpdateClient(Client client);
        Task DeleteClient(string id);
    }
}
