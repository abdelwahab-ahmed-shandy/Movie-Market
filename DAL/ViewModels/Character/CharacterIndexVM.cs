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
        public List<MovieIndexVM> Movies { get; set; } = new List<MovieIndexVM>();
        public List<TvSeriesVM> TvSeries { get; set; } = new List<TvSeriesVM>();
    }
}
