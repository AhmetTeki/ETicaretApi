using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Exceptions
{
    public class Authantication : Exception
    {
        public Authantication()
        {
        }

        public Authantication(string? message) : base("kimlik doğrulama hatası")
        {
        }

        protected Authantication(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
