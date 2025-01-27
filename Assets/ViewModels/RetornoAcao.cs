using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Models
{
    [Serializable]
    public class RetornoAcao<T>
    {
        public T result;
        public Error[] errors;
        public bool isFailed;
    }

    [Serializable]
    public struct Error
    {
        public string code;
        public string message;
    }
}
