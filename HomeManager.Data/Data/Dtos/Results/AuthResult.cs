using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Dtos.Results
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}
