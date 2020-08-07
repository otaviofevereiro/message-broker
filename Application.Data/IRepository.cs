using System.Threading.Tasks;

namespace Application.Data
{
    public interface IRepository<TModel>
        where TModel : class
    {
        void Add(TModel model);
    }
}