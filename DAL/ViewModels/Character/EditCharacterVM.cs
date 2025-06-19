using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Character
{
    public class EditCharacterVM
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [Display(Name = "Character Icon")]
        [DataType(DataType.Upload)]
        public IFormFile Img { get; set; }
        public CurrentState CurrentState { get; set; }
        public List<Guid>? SelectedMovieIds { get; set; }
        public List<Guid>? SelectedTvSeriesIds { get; set; }
    }
}
