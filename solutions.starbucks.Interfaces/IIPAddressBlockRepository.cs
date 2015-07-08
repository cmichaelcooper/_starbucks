
namespace solutions.starbucks.Interfaces
{
    public interface IIPAddressBlockRepository
    {
        string GetCountryForIPAddress(string ipAddress);
    }
}