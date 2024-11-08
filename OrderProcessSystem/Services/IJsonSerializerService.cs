using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessSystem.Services
{
    public interface IJsonSerializerService
    {
        T Deserialize<T>(string json);
    }
}
