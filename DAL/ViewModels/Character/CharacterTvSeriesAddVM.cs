using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Character
{
    public class CharacterTvSeriesAddVM
    {
        public Guid TvSeriesId { get; set; }
        public string TvSeriesTitle { get; set; }

        [Required(ErrorMessage = "Please select a character")]
        public Guid CharacterId { get; set; }

        public List<CharacterSelectionVM> AvailableCharacters { get; set; } = new List<CharacterSelectionVM>();
    }
}
