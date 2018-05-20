using System.ComponentModel.DataAnnotations;

namespace API.ViewModel
{
    public class Request
    {
        [Required]
        [Url]
        public string Url { get; set; }
    }
}