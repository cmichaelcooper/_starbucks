using System;

namespace solutions.starbucks.web.Infrastructure.Store
{
    public class Nonce
    {
        public string Context { get; set; }

        public string Code { get; set; }

        public DateTime Timestamp { get; set; }
    }
}