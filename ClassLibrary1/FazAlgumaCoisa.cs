using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
namespace ClassLibrary1
{
    public sealed class FazAlgumaCoisa : CodeActivity
    {
        [Output("Retorno")]
        public OutArgument<string> retorno { get; set; }

        [Input("Entrada")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> entrada { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            #region Serviços
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();

            Guid usuario = executionContext.GetValue<EntityReference>(entrada).Id;

            IOrganizationService serviceAdmin = serviceFactory.CreateOrganizationService(usuario);

            ITracingService trace = executionContext.GetExtension<ITracingService>();

            executionContext.SetValue<string>(retorno, usuario.ToString());

            retorno.Set(executionContext, usuario.ToString());
            #endregion

            if (1 == 2)
            {
                trace.Trace("nao entra");
            }
            else
            {
                trace.Trace("se nao");
                //throw new Exception("Erro");
            }
            trace.Trace("finaliza");

            //throw new NotImplementedException();
        }
    }
}
