using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.DTOs.Response
{
    public class BaseResponse<T>
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public Dictionary<string, string> Errors { get; set; }

        public BaseResponse()
        {
            Errors = new Dictionary<string, string>();
        }

    }
}
