using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionCoreAPI.DTOs
{
    public class LibroDTOConAutores: LibroDTO
    {
        public List<AutorDTO> Autores { get; set; }
    }
}
