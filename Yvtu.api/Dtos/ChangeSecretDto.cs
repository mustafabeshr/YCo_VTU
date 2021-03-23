using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yvtu.api.Dtos
{
    public class ChangeSecretDto
    {
        public int oldSecret { get; set; }
        public int newSecret { get; set; }
    }
}
