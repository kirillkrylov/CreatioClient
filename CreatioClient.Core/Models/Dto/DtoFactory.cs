using System;
using System.Collections.Generic;
using System.Text;

namespace CreatioClient.Core.Models.Dto
{
    internal static class DtoFactory
    {
       internal static T Create<T>() where T :class, new()
        {
            return new T();
        }
    }
}
