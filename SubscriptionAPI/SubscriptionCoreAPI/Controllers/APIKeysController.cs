using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SubscriptionCoreAPI.Servicios;
using System.Threading.Tasks;
using System.Linq;
using SubscriptionCoreAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SubscriptionCoreAPI.Controllers
{
    [ApiController]
    [Route("api/apikeys")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class APIKeysController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly KeysService keysService;

        public APIKeysController(ApplicationDbContext context,
                                 IMapper mapper,
                                 KeysService keysService)
        {
            this.context = context;
            this.mapper = mapper;
            this.keysService = keysService;
        }

        [HttpGet]
        public async Task<List<KeyDTO>> MyKeys()
        {
            var userId = GetUserId();
            var keys = await context.APIKeys.Where(x => x.UsuarioId == userId).ToListAsync();
            return mapper.Map<List<KeyDTO>>(keys);
        }

        [HttpPost]
        public async Task<ActionResult> CreateKey(CreateKeyDTO createKeyDTO)
        {
            var userId = GetUserId();

            if (createKeyDTO.KeyType == Entidades.KeyType.Free)
            {
                var userAlreadyOwnsFreeKey = await context.APIKeys.AnyAsync(x => x.UsuarioId == userId && x.KeyType == Entidades.KeyType.Free);

                if (userAlreadyOwnsFreeKey)
                {
                    return BadRequest("The user already has a free account.");
                }
            }

            await keysService.CreateKey(userId, createKeyDTO.KeyType);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateKey(UpdateKeyDTO updateKeyDTO)
        {
            var userId = GetUserId();
            var keyDB = await context.APIKeys.FirstOrDefaultAsync(x => x.Id == updateKeyDTO.KeyId);

            if (keyDB == null)
            {
                return NotFound();
            }

            if (userId != keyDB.UsuarioId)
            {
                return Forbid();
            }

            if (updateKeyDTO.UpdateKey)
            {
                keyDB.Key = keysService.GenerateKey();
            }

            keyDB.Active = updateKeyDTO.Active;
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
