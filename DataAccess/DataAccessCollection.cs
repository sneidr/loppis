using DataAccess.Model;

namespace DataAccess.DataAccess;

public class DataAccessCollection : IDataAccessCollection
{
    public void Add(IDataAccess dataAccess)
    {
        _dataAccess.Add(dataAccess);
    }

    public void WriteSale(Sale sale)
    {
        foreach (var dataAccess in _dataAccess)
        {
            dataAccess.WriteSale(sale);
        }
    }

    public void RemoveSale(Sale sale)
    {
        foreach (var dataAccess in _dataAccess)
        {
            dataAccess.RemoveSale(sale);
        }
    }

    private List<IDataAccess> _dataAccess { get; set; } = new List<IDataAccess>();
}