using System;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using System.Collections.Generic;
using Domain.Repositories;
using Domain.StatusSolicitacaoEnum;
using Domain.StatusStorageEnum;
using System.Data;
using Infrastructure.SQLServer;


namespace Application.Impl
{
    public class AplicaAclService : IAplicaAclService {
        
        public readonly IAplicaAclSQLRepository _aplicaAclSQLRepository;
        public readonly IAplicaAclGraphApiRepository _aplicaAclGraphApiRepository;

        public AplicaAclService(IAplicaAclSQLRepository aplicaAclSQLRepository, IAplicaAclGraphApiRepository aplicaAclGraphApiRepository)
        {
            _aplicaAclSQLRepository = aplicaAclSQLRepository;
            _aplicaAclGraphApiRepository = aplicaAclGraphApiRepository;
        }
        
        public async Task<string> CallBatchAcl()
        {
            try
            {
                //Colunas da tabela SQL TbSolicitacao
                Int64 idSolicitacao = -1;
                string tpSolicitacao = "";
                string nmChaveAcesso = "";
                string stSolicitacao = "";
                string tpChaveAcesso = "";

                //Colunas da tabela SQL TbInfoStorage
                Int64 idInfoStorage = -1;
                string nmDiretorio = "";
                string tpAcesso = "";
                string stInfoStorage = "";
                string nmStorageAccount = "";
                string nmContainer = "";
                bool flRecursivo = false;

                DataTable tbSolicitacao = _aplicaAclSQLRepository.getSolicitacoes(StatusSolicitacao.AGUARDANDO_PROCESSAMENTO);

                
                for (int i = 0; i < tbSolicitacao.Rows.Count; i++) 
                {
                    idSolicitacao = Convert.ToInt64(tbSolicitacao.Rows[i]["IdSolicitacao"]);
                    tpSolicitacao = Convert.ToString(tbSolicitacao.Rows[i]["TpSolicitacao"]);
                    nmChaveAcesso = Convert.ToString(tbSolicitacao.Rows[i]["NmChaveAcesso"]);
                    stSolicitacao = Convert.ToString(tbSolicitacao.Rows[i]["StSolicitacao"]);
                    tpChaveAcesso = Convert.ToString(tbSolicitacao.Rows[i]["TpChaveAcesso"]);

                    //log.LogWarning(idSolicitacao + " | " + tpSolicitacao + " | " + nmChaveAcesso + " | " + stSolicitacao + " | " + tpChaveAcesso);   
                    
                    // ALtera o campo StSolicitacao para "EM_PROCESSAMENTO"
                    _aplicaAclSQLRepository.SetStatusSolicitacao(idSolicitacao, StatusSolicitacao.EM_PROCESSAMENTO);

                    DataTable tbSorage = _aplicaAclSQLRepository.getStorages(idSolicitacao, StatusStorage.AGUARDANDO_PROCESSAMENTO);
                        for (int f = 0; f < tbSorage.Rows.Count; f++) 
                        {
                            idInfoStorage = Convert.ToInt64(tbSorage.Rows[f]["IdInfoStorage"]);;
                            nmDiretorio = Convert.ToString(tbSorage.Rows[f]["NmDiretorio"]);
                            tpAcesso = Convert.ToString(tbSorage.Rows[f]["TpAcesso"]);
                            stInfoStorage = Convert.ToString(tbSorage.Rows[f]["StInfoStorage"]);
                            nmStorageAccount = Convert.ToString(tbSorage.Rows[f]["NmStorageAccount"]);
                            nmContainer = Convert.ToString(tbSorage.Rows[f]["NmContainer"]);
                            flRecursivo = Convert.ToBoolean(tbSorage.Rows[f]["FlRecursivo"]);

                            //log.LogWarning(idInfoStorage + " | " + nmDiretorio + " | " + tpAcesso + " | " + stInfoStorage + " | " + nmStorageAccount + " | " + nmContainer + " | " + flRecursivo);

                            _aplicaAclSQLRepository.SetStatusInfoStorage(idSolicitacao, idInfoStorage, StatusStorage.EM_PROCESSAMENTO, "n/a");
                            try
                            {
                                await Task.Run( () => AplicaAcl(  idSolicitacao,
                                                            tpSolicitacao,
                                                            nmChaveAcesso, 
                                                            stSolicitacao,
                                                            tpChaveAcesso,
                                                            idInfoStorage,
                                                            nmDiretorio,
                                                            tpAcesso,
                                                            stInfoStorage,
                                                            nmStorageAccount,
                                                            nmContainer,
                                                            flRecursivo
                                                        )
                                );
                            }
                            catch (SystemException e)
                            {
                                _aplicaAclSQLRepository.SetStatusInfoStorage(idSolicitacao, idInfoStorage, StatusStorage.PROCESSADO_COM_ERRO, e.Message);
                            }
                            finally
                            {
                                Console.WriteLine("Exception caught");
                            
                            }
                            _aplicaAclSQLRepository.SetStatusInfoStorage(idSolicitacao, idInfoStorage, StatusStorage.PROCESSADO, "n/a");
                        }
                }
            }
            catch(Exception e)
            {
                return $"Falha no processamento. Erro: {e.Message}";
            }
            return "ok";
        }
        private async void AplicaAcl(Int64 idSolicitacao, string tpSolicitacao, string nmChaveAcesso, string stSolicitacao, string tpChaveAcesso, Int64 idInfoStorage, string nmDiretorio, string tpAcesso, string stInfoStorage, string nmStorageAccount, string nmContainer, bool flRecursivo) {


            string uri = "https://" + nmStorageAccount + ".blob.core.windows.net";

            Azure.Storage.Files.DataLake.Models.RolePermissions concatAcessos = new Azure.Storage.Files.DataLake.Models.RolePermissions();
            if (tpAcesso.Contains("R")){concatAcessos = concatAcessos | RolePermissions.Read;}
            //if (tpAcesso.Contains("W")){_aplicaAclRepository("SQL").SetStatusInfoStorage(idSolicitacao, idInfoStorage, StatusStorage.PROCESSADO_COM_ERRO, "Não é permitido conceder acesso para escrever nesse diretório");}
            if (tpAcesso.Contains("X")){concatAcessos = concatAcessos | RolePermissions.Execute;}

            string storageAccountKey = "";
            if (nmStorageAccount == "vvdevstgdatalake"){
                storageAccountKey = "7i4Dr1WI92ej+UgSGndPfH/uO/Hgtqim93oOn4A7vk+9sBPolvgr2zI5a3fgYSo08hIh6p8U9/Frm8N4Lg+21g==";
            }else if(nmStorageAccount == "vvhlgstgdatalake"){
                storageAccountKey = "____fSseCp+e33P4Svu5S/ioU3a81af0j0VaU+Myi1sTPDfoDHdg0ct/8/vk3VMQo1/JPdd5HxcnIpuHs2W4Nvf+DQ==";
            }else if(nmStorageAccount == "vvprdstgdatalake"){
                storageAccountKey = "____p4iPN0I0EsYWOSi3AXBKwP19Blf7ZnnKhMjxxfVooELwWkP+Q5ni9s38Q3/AcsauEaDzLd42m9/TysEXQfMrfw==";
            }
            
            // Recupera o objectId usando GraphAPI
            string objectId = "";
            Azure.Storage.Files.DataLake.Models.AccessControlType tipo = new Azure.Storage.Files.DataLake.Models.AccessControlType();
            if (tpChaveAcesso == "USUARIO")
            {
                tipo = AccessControlType.User;
                objectId = await _aplicaAclGraphApiRepository.GetUserObjectId(nmChaveAcesso);
            }
            else if(tpChaveAcesso == "GRUPO_USUARIO")
            {
                tipo = AccessControlType.Group;
                objectId = await _aplicaAclGraphApiRepository.GetGroupObjectId(nmChaveAcesso);
            }
            else if(tpChaveAcesso == "CHAVE_SERVICO")
            {
                tipo = AccessControlType.Group;
                objectId = await _aplicaAclGraphApiRepository.GetServicePrincipalObjectId(nmChaveAcesso);
            }

            if(objectId == null){
                _aplicaAclSQLRepository.SetStatusInfoStorage(idSolicitacao, idInfoStorage, StatusStorage.PROCESSADO_COM_ERRO, "Chave de acesso " + nmChaveAcesso + " não encontrada");
                //await Task.Run( () => throw new SystemException("Chave de acesso " + nmChaveAcesso + " não encontrada"));
            }

/////////////////////////////////////////////////////////////////////////////
// Insere, Atualiza ACL recursivamente ou não
/////////////////////////////////////////////////////////////////////////////

            // Create DataLakeServiceClient using StorageSharedKeyCredentials
            StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(nmStorageAccount, storageAccountKey);
            DataLakeServiceClient serviceClient = new DataLakeServiceClient(new Uri(uri), sharedKeyCredential);   

            List<string> listaNmDiretorio = new List<string>(){""};
            listaNmDiretorio.AddRange(nmDiretorio.Split('/'));
            Int64 count_lista = listaNmDiretorio.Count;
            bool recursivo = false;

            for (int i = 0; i < listaNmDiretorio.Count; i++) 
            {
                if (i == 0){
                    nmDiretorio = listaNmDiretorio[i];
                }else if (i > 0){
                    nmDiretorio = nmDiretorio + "/" + listaNmDiretorio[i];
                }

                if (listaNmDiretorio.Count - i > 1){
                    recursivo = false;
                }else if (listaNmDiretorio.Count - i == 1){
                    recursivo = flRecursivo;
                }

                DataLakeFileSystemClient filesystem = serviceClient.GetFileSystemClient((nmContainer));
                DataLakeDirectoryClient directoryClient = filesystem.GetDirectoryClient(nmDiretorio);
                PathAccessControl directoryAccessControl = await directoryClient.GetAccessControlAsync();

                List<PathAccessControlItem> accessControlListUpdate = (List<PathAccessControlItem>)directoryAccessControl.AccessControlList;

                int index = -1;
                
                if (tpSolicitacao == "INCLUSAO"){
                    foreach (var item in accessControlListUpdate)
                    {
                        if(item.EntityId == objectId)
                        {
                            index = accessControlListUpdate.IndexOf(item);
                            break;
                        }
                    }

                    if (index > -1)
                    {
                        // Atualiza
                        accessControlListUpdate[index] = new PathAccessControlItem(
                            tipo,
                            concatAcessos
                            , false, objectId);

                        if (recursivo == true){
                            await directoryClient.UpdateAccessControlRecursiveAsync(accessControlListUpdate, null);
                        }else if (recursivo == false){
                            directoryClient.SetAccessControlList(accessControlListUpdate);
                        }                    
                    }else
                    {
                        // Adiciona
                        accessControlListUpdate.Add(new PathAccessControlItem(
                            tipo,
                            concatAcessos
                            , false, objectId)
                        );

                        if (recursivo == true){
                            await directoryClient.UpdateAccessControlRecursiveAsync(accessControlListUpdate, null);
                        }else if (recursivo == false){
                            directoryClient.SetAccessControlList(accessControlListUpdate);    
                        } 

                    }
                }
            }
            
            
/////////////////////////////////////////////////////////////////////////////
// Exclui ACL recursivamente
/////////////////////////////////////////////////////////////////////////////

            if(tpSolicitacao == "EXCLUSAO")
            {
                DataLakeFileSystemClient filesystem = serviceClient.GetFileSystemClient((nmContainer));
                DataLakeDirectoryClient directoryClient = filesystem.GetDirectoryClient(nmDiretorio);
                PathAccessControl directoryAccessControl = await directoryClient.GetAccessControlAsync();

                //List<PathAccessControlItem> accessControlListUpdate = (List<PathAccessControlItem>)directoryAccessControl.AccessControlList;
                List<RemovePathAccessControlItem> accessControlListForRemoval = new List<RemovePathAccessControlItem>()
                {
                new RemovePathAccessControlItem(
                    tipo, 
                    false,
                    objectId)
                };

            await directoryClient.RemoveAccessControlRecursiveAsync(accessControlListForRemoval, null);
            
            }
        }
        
    }
}