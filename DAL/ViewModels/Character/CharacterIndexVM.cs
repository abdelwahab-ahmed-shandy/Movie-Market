using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Character
{
    public class CharacterIndexVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Img { get; set; }
        public List<string> Movies { get; set; } = new List<string>();
        public List<string> TvSeries { get; set; } = new List<string>();
    }
}
