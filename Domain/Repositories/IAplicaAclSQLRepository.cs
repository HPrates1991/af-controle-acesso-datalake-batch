using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.AplicaAcl;
using Domain.StatusSolicitacaoEnum;
using Domain.StatusStorageEnum;
using System.Data;

namespace Domain.Repositories
{
    public interface IAplicaAclSQLRepository
    {
        void SetStatusSolicitacao(Int64 IdSolicitacao, StatusSolicitacao StatusSol);
        DataTable getSolicitacoes(StatusSolicitacao StatusSol);
        void SetStatusInfoStorage(Int64 IdSolicitacao, Int64 IdStorage, StatusStorage StatusInfoS, string dsObs);
        DataTable getStorages(Int64 IdSolicitacao, StatusStorage StatusInfoS);
    }
}
