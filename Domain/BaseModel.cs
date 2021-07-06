using System;
using System.Numerics;
using System.Collections.Generic;

namespace Domain
{
    public class BaseModel
    {
        public BaseModel() { }

        public BaseModel(   Int64   idSolicitacao, 
                            string  tpSolicitacao, 
                            string  nmChaveAcesso, 
                            string  stSolicitacao,
                            string  tpChaveAcesso,
                            Int64   idInfoStorage,
                            string  nmDiretorio,
                            string  tpAcesso,
                            string  stInfoStorage,
                            string  nmStorageAccount,
                            string  nmContainer,
                            bool    flRecursivo)
        {
            IdSolicitacao = idSolicitacao;
            TpSolicitacao = tpSolicitacao;
            NmChaveAcesso = nmChaveAcesso;
            StSolicitacao = stSolicitacao;
            TpChaveAcesso = tpChaveAcesso;
            IdInfoStorage = idInfoStorage;
            NmDiretorio = nmDiretorio;
            TpAcesso = tpAcesso;
            StInfoStorage = stInfoStorage;
            NmStorageAccount = nmStorageAccount;
            NmContainer = nmContainer;
            FlRecursivo = flRecursivo;
        }

        public Int64 IdSolicitacao { get; set; }
        public string TpSolicitacao { get; set; }
        public string NmChaveAcesso { get; set; }
        public string StSolicitacao { get; set; }
        public string TpChaveAcesso { get; set; }
        public Int64  IdInfoStorage { get; set; }
        public string NmDiretorio { get; set; }
        public string TpAcesso { get; set; }
        public string StInfoStorage { get; set; }
        public string NmStorageAccount { get; set; }
        public string NmContainer { get; set; }
        public bool FlRecursivo { get; set; }
    }
}