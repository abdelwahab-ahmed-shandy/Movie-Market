using DAL.ViewModels.Character;
using DAL.ViewModels.Season;
using MovieMart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.TvSeries
{
    public class TvSeriesDetailsVM
    {
        public TvSeriesCharacterVM TvSeries { get; set; } = null!;
        public List<SeasonVM> Seasons { get; set; } = new();
        public List<CharacterTvSeriesCustomerVM> Characters { get; set; } = new();
    }
}
