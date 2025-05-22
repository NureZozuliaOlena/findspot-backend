using findspot_backend.Models;

namespace findspot_backend.Repositories
{
    public interface ITouristObjectRepository
    {
        IEnumerable<TouristObject> GetAll();
        TouristObject Get(Guid id);
        TouristObject Add(TouristObject touristObject);
        TouristObject Update(TouristObject touristObject);
        bool Delete(Guid id);
    }
}