using solutions.starbucks.Model.Pocos;
using System.Collections.Generic;

namespace solutions.starbucks.Interfaces
{
    public interface IInvitesRepository
    {
        IEnumerable<Invites> GetInvitesByOperatorUmbracoUserID(int umbracoUserID, bool? isDeleted = false);
        IEnumerable<Invites> GetAccountInvitesByOperatorUmbracoUserID(int operatorUmbracoUserID, IMemberAttributesRepository memberAttributesRepository, bool? isDeleted = false);
        Invites GetByAccessToken(string accessToken);
        Invites GetById(int inviteId);
        Invites Save(Invites model);
        Invites Delete(Invites model);
    }
}