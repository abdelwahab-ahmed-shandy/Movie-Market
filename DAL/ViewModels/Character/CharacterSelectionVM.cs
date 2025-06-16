using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Character
{
    public class CharacterSelectionVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ActorName { get; set; }
    }
}
