using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Character
{
    public class CharacterAdminVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Img { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public CurrentState CurrentState { get; set; }
        public List<string> MovieTitles { get; set; } = new();
        public List<string> TvSeriesTitles { get; set; } = new();
    }
}
