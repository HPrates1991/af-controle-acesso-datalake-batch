using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.AplicaAcl;
using Domain.StatusSolicitacaoEnum;
using Domain.StatusStorageEnum;
using System.Data;

namespace Domain.Repositories
{
    public interface IAplicaAclGraphApiRepository
    {
        Task<string> GetToken();
        Task<string> GetUserObjectId(string userEmail);
        Task<string> GetGroupObjectId(string groupName);
        Task<string> GetServicePrincipalObjectId(string servicePrincipalName);
    }
}
