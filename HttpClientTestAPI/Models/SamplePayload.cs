using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpClientTestAPI.Models
{
    public class SamplePayload
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
