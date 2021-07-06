using System.Threading.Tasks;
using Domain.AplicaAcl;
using System;

namespace Application
{
    public interface IAplicaAclService
    {
        Task<string> CallBatchAcl();
    }
}