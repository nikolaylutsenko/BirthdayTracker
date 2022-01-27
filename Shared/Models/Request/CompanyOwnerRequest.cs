using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayTracker.Shared.Requests
{
    public record CompanyOwnerRequest
    {
        [Required]
        [RegularExpression("[a-zA-Z '-]")]
        public string Name { get; set; }

        [Required]
        [RegularExpression("[a-zA-Z '-]")]
        public string Surname { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDay { get; set; }

        [Required]
        [RegularExpression(@"/^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$/gm")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string PositionName { get; set; }
    }
}
