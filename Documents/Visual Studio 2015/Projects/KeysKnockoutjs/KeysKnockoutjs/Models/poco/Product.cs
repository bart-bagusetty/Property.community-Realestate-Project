using System.ComponentModel.DataAnnotations;

namespace KeysKnockoutjs.Models.poco
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name cannot be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price cannot be empty")]
        public int Price { get; set; }
    }
}


