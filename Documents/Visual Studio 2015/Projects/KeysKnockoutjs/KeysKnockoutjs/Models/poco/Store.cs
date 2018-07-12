using System.ComponentModel.DataAnnotations;

namespace KeysKnockoutjs.Models.poco
{
    public class Store
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name cannot be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address cannot be empty")]
        public string Address { get; set; }
    }
}
