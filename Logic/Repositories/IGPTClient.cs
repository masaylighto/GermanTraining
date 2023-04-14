using AnyOfTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Repositories;

public interface IGPTClient
{
    public Task<AnyOf<String, Exception>> GetAnswer(string Question);
}
