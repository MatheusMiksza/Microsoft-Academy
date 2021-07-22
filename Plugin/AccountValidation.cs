using Microsoft.Xrm.Sdk;
using System;



namespace AccountPreValidation//namespace serve para empacotar para referenciar ele em outro lugar

{   //plugin é uma forma de execuar um codigo atraves de açoes
    //
    //plugin sincrono espera o retorno para prosseguir
    //plugin assincrono não fica esperando uma resposta imediata
    #region Plugin para preenchimento do campo contato primario
    public class AccountValidation : IPlugin  //essa interface obriga que a classe faça o metodo a baixo 
    {
        public void Execute(IServiceProvider serviceProvider) //recebe informaçoas do serviceProvider
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));  //GetService pega as informaçoes do IPluginExecutionContext/typeof pega o tipo da classe 

            // var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // var trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity entidadeContexto = null; // variavel do tipo Entity

            if(context.InputParameters.Contains("Target"))
                entidadeContexto = (Entity)context.InputParameters["Target"];//verifiva se na propriedade InputParameters tem o objeto Target que é a informação do 


            else
                return;

                                  //Contains se não achar nada no primarycontactid ele lança o erro
            if (!entidadeContexto.Contains("primarycontactid"))//se nao tiver o primarycontactid faça e não salva o registro
                throw new InvalidPluginExecutionException("Contato principal obrigatorio!");// é um disparo de esceção
        }       //throw é um operador

    }
    #endregion

}
