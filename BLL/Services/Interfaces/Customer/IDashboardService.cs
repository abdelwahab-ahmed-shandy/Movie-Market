using DAL.ViewModels.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Customer
{
    public interface IDashboardService
    {
        Task<DashboardVM> GetDashboardDataAsync();
    }
}
