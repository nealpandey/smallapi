using System;

namespace SmallApiFunctions
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Body { get; set; }

        public string Status { get; set; }
        public string Detail { get; set; }
    }
}
