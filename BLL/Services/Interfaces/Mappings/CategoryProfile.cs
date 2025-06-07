using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Mappings
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryAdminCreateEditVM, Category>()

                .ForMember(dest => dest.Movies, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDateUtc, opt => opt.Ignore())
                .ForMember(dest => dest.BlockedBy, opt => opt.Ignore())
                .ForMember(dest => dest.BlockedDateUtc, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDateUtc, opt => opt.Ignore())
                .ForMember(dest => dest.RestoredBy, opt => opt.Ignore())
                .ForMember(dest => dest.RestoredDateUtc, opt => opt.Ignore())
                .ForMember(dest => dest.LastAction, opt => opt.Ignore())

                .AfterMap((src, dest, context) =>
                {
                    var httpContextAccessor = context.Items["HttpContextAccessor"] as IHttpContextAccessor;
                    var userName = httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

                    if (dest.Id == Guid.Empty) // Create
                    {
                        dest.CreatedBy = userName;
                        dest.CreatedDateUtc = DateTime.UtcNow;
                    }
                    else // Update
                    {
                        dest.UpdatedBy = userName;
                        dest.UpdatedDateUtc = DateTime.UtcNow;
                    }
                });
        }
    }
}
