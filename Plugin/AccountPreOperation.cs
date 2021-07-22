using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PluginMatheus;

namespace Plugin
{
    public class AccountPreOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName.ToLower() == "create" && context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
                context.Stage == Convert.ToInt32(MeuEnum.Stage.PreOperation))
            {
                var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                var serviceUser = serviceFactory.CreateOrganizationService(context.UserId);
                var trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                trace.Trace("Inicio Plugin");
                Entity entidadeContexto = null;

                if (context.InputParameters.Contains("Target"))
                    entidadeContexto = (Entity)context.InputParameters["Target"];

                var contatoId = Guid.Empty;
                if (entidadeContexto.Attributes.Contains("primarycontactid"))
                    contatoId = ((EntityReference)entidadeContexto.Attributes["primarycontactid"]).Id;
                trace.Trace("contatoId: " + contatoId);
                QueryExpression queryExpression = new QueryExpression("account");

                queryExpression.Criteria.AddCondition("primarycontactid", ConditionOperator.Equal, contatoId);
                queryExpression.ColumnSet = new ColumnSet("primarycontactid");
                var colecaoEntidades = serviceUser.RetrieveMultiple(queryExpression);
                trace.Trace("teste: " + colecaoEntidades.Entities.Count);
                if (colecaoEntidades.Entities.Count > 0)
                    throw new InvalidPluginExecutionException("Contato ja utilizado em outra conta!");
            }
            
        }
    }
    

    
}
