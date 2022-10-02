using DataAccess.Model;
using System.Data;

namespace DataAccess.DataAccess;

public interface IDataAccess
{
    void WriteSale(Sale sale);
}

public class DataAccessCollection
{
    public void WriteSale(Sale sale)
    {
        foreach(var dataAccess in DataAccess)
        {
            dataAccess.WriteSale(sale);
        }
    }

    public List<IDataAccess> DataAccess { get; set; } = new List<IDataAccess>();
}