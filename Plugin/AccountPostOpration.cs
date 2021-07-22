using System;
using Microsoft.Xrm.Sdk;
using PluginMatheus;

namespace Plugin
{
    public class AccountPostOpration : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.MessageName.ToLower() == "update" && context.Mode == Convert.ToInt32(MeuEnum.Mode.Synchronous) &&
                context.Stage == Convert.ToInt32(MeuEnum.Stage.PostOperation))
            {
                var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                var serviceUser = serviceFactory.CreateOrganizationService(context.UserId);
                var trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                trace.Trace("Inicio Plugin");
                Entity entidadeContexto = null;
                Entity entidadePre = null;

                if (context.InputParameters.Contains("Target"))
                    entidadeContexto = (Entity)context.InputParameters["Target"];

                var contato = Guid.Empty;
                if (entidadeContexto.Attributes.Contains("primarycontactid"))
                {
                    contato = ((EntityReference)entidadeContexto.Attributes["primarycontactid"]).Id;

                    if (context.PreEntityImages.Contains("preImagem"))
                        entidadePre = (Entity)context.PreEntityImages["preImagem"];

                    if (entidadeContexto == null || entidadePre == null)
                        return;

                    if (entidadeContexto.Contains("primarycontactid") && entidadePre.Contains("primarycontactid")
                        && (entidadeContexto["primarycontactid"]) != entidadePre["primarycontactid"])
                        throw new InvalidPluginExecutionException("Não é possivel alteral o contato principal");
                
                }
            }
        }
    }
}
