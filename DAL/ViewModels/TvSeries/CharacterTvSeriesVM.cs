using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.TvSeries
{
    public class CharacterTvSeriesVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ActorName { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
