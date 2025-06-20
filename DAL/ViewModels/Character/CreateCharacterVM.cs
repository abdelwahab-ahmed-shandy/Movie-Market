using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Character
{
    public class CreateCharacterVM
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? Img { get; set; }

        [Display(Name = "Status")]
        public CurrentState CurrentState { get; set; }

    }
}
