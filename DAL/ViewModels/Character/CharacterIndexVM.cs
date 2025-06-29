using DAL.ViewModels.TvSeries;
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
        public List<MovieCharacterIndexVM> Movies { get; set; } = new List<MovieCharacterIndexVM>();
        public List<TvSeriesCharacterVM> TvSeries { get; set; } = new List<TvSeriesCharacterVM>();
    }
}
