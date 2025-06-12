using DAL.ViewModels.Special;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces.Customer
{
    public interface ICustomerSpecialService
    {
        Task<IEnumerable<SpecialIndexVM>> GetActiveSpecialAsync();
        Task<SpecialDetailsVM?> GetSpecialDetailsAsync(Guid id);
    }
}
