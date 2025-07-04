using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels.Category
{
    public class CategoryAdminCreateEditVM
    {
        [Required(ErrorMessage = "Classification name is required")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
        public string? Description { get; set; }

        [Display(Name = "Status")]
        public CurrentState CurrentState { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile IconFile { get; set; }
    }
}
