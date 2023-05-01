using System.ComponentModel.DataAnnotations;

namespace Producer.Dto
{
    public class JobQueryRequest
    {
        public string SearchTerm { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        public string Location { get; set; }
    }
}
