using Microsoft.EntityFrameworkCore;

namespace ASP_PV411.Data
{
    // DAL - Data Access Layer - "централізація" логіки доступу до даних
    public class DataAccessor(DataContext dataContext)
    {
        public List<Entities.Group> GetSiteGroups()
        {
            return dataContext
                .Groups
                .Where(g => g.DeleteAt == null)
                .ToList();
        }

        public Entities.Group? GetGroupBySlug(String slug)
        {
            return dataContext
                .Groups
                .AsNoTracking()
                .Include(g => g.Products)
                .FirstOrDefault(g => g.Slug == slug);
        }
    }
}
