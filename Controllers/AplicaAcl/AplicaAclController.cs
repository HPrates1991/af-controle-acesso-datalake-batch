using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Application;

namespace Controllers
{
    public class AplicaAclController
    {
        private readonly IAplicaAclService _aplicaAclService;

        public AplicaAclController(IAplicaAclService aplicaAclService)
        {
            _aplicaAclService = aplicaAclService;
        }

        [FunctionName("ControleAcessoDatalakeBatch")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ControleAcessoDatalakeBatch")] HttpRequest req,
            ILogger log)
        {
            string resultado = await _aplicaAclService.CallBatchAcl();

            return new OkObjectResult(resultado);
            
        }
    }
}
