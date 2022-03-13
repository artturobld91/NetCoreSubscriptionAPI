using SubscriptionCoreAPI;
using SubscriptionCoreAPI.Entidades;
using System;
using System.Threading.Tasks;

namespace SubscriptionCoreAPI.Servicios
{
    public class KeysService
    {
        private readonly ApplicationDbContext context;

        public KeysService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task CreateKey(string usuarioId, KeyType keytype)
        {
            var key = GenerateKey();
            var keyAPI = new APIKey
            {
                Active = true,
                Key = key,
                KeyType = keytype,
                UsuarioId = usuarioId,
            };

            context.Add(keyAPI);
            await context.SaveChangesAsync();
        }

        public string GenerateKey()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
