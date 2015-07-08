using solutions.starbucks.Model.Pocos;

namespace solutions.starbucks.Interfaces
{
    public interface IPartnerAccountsRepository
    {
        PartnerAdmin GetPartnerByEmail(string emailAddress);
        SuperPartnerAdmin GetSuperPartnerByEmail(string emailAddress);
    }
}