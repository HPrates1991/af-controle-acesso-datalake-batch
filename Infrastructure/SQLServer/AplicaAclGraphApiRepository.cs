using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Domain.Repositories;

namespace Infrastructure.GraphApi
{
    class AplicaAclGraphApiRepository : IAplicaAclGraphApiRepository
    {
        public const string clientId = "6ae7b43d-975a-4e33-b959-5e77bfb413b5";
        public const string aadInstance = "https://login.microsoftonline.com/{0}";
        public const string tenant = "viavarejo.onmicrosoft.com";
        public const string resource = "https://graph.microsoft.com";
        public const string appKey = "DWj_L36LrFVmhDOhtSPWUsfZ._a4T~160m";
        static string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        public static HttpClient httpClient = new HttpClient();
        public static AuthenticationContext context = new AuthenticationContext(authority);
        public static ClientCredential credential = new ClientCredential(clientId, appKey);

        public async Task<string> GetToken()
        {
            AuthenticationResult result = null;
            string token = null;
            result = await context.AcquireTokenAsync(resource, credential);
            token = result.AccessToken;
            return token;
        }
        public async Task<string> GetUserObjectId(string userEmail)
        {
            string userInfo = null;
            string token = await GetToken();
            string objectId = null;
            var uri = "https://graph.microsoft.com/v1.0/users/" + userEmail + "/?$select=displayName,userPrincipalName,Id";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await httpClient.GetAsync(uri);

            if (result.ReasonPhrase == "Not Found")
            {
                return null;
            }

            if (result.Content != null)
            {
                userInfo = await result.Content.ReadAsStringAsync();
                dynamic userData = JsonConvert.DeserializeObject(userInfo);
                objectId = userData.id;
            }
            return objectId;
        }
        public async Task<string> GetGroupObjectId(string groupName)
        {
            string groupInfo = null;
            string token = await GetToken();
            string objectId = null;

            var uri = "https://graph.microsoft.com/v1.0/groups?$filter=displayName eq '" + groupName + "'&$select=displayName,id";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await httpClient.GetAsync(uri);

            if (result.Content != null)
            {
                groupInfo = await result.Content.ReadAsStringAsync();
                dynamic groupData = JsonConvert.DeserializeObject(groupInfo);
                string displayName = groupData.value[0].displayName;
                objectId = groupData.value[0].id;
            }
            return objectId;
        }
        public async Task<string> GetServicePrincipalObjectId(string servicePrincipalName)
        {
            string servicePrincipalInfo = null;
            string token = await GetToken();
            string objectId = null;

            var uri = "https://graph.microsoft.com/v1.0/serviceprincipals/?$filter=displayName eq '" + servicePrincipalName + "'&$select=appDisplayName,appId";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await httpClient.GetAsync(uri);

            if (result.Content != null)
            {
                servicePrincipalInfo = await result.Content.ReadAsStringAsync();
                dynamic groupData = JsonConvert.DeserializeObject(servicePrincipalInfo);
                string displayName = groupData.value[0].displayName;
                objectId = groupData.value[0].appId;
            }
            return objectId;
        }
    }
}