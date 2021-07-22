using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Net;
using Microsoft.Xrm.Tooling.Connector;
//using ConexaoAlternativaExtending;

namespace TreinamentoExtending //namespace serve para empacotar para referenciar ele em outro lugar
{
    class Program
    {
        static void Main(string[] args)
        {
            //Descoberta();      // a varialvel serviceproxy herda toda propriedade da classe conexao
            var serviceproxy = new Conexao().Obter(); // essa linha cria a conexão com o dynamics
            //MeuCreate(serviceproxy); // essa executa o metodo desejado 
            //MeuUpdate(serviceproxy);
            //FetchXMLAgrefate(serviceproxy);
            TesteCreateContct(serviceproxy);
            //RetornarMultiplo(serviceproxy);
            //RetornarMultipliComLink(serviceproxy);
            //TesteCreate(serviceproxy);
            //TesteCreate(serviceproxy);

            Console.ReadKey();
        }
        #region Descoberta
        static void Descoberta()
        {
            Uri local = new Uri("https://disco.crm2.dynamics.com/XRMServices/2011/Discovery.svc");

            ClientCredentials clientcred = new ClientCredentials();   //class variavel = atribuir um valor, o comando new eu coloco se eu não sei qual o valor da variavel
            clientcred.UserName.UserName = "matheus.miksza@treinoDEV.onmicrosoft.com";   // atribuindo valor as propriedades 
            clientcred.UserName.Password = "7055550141/Pats";
            DiscoveryServiceProxy ponteCrm = new DiscoveryServiceProxy(local, null, clientcred, null);   // DiscoveryServiceProxy é uma class para se conectar com o Dynamics com os paremetro endereço e os dados do usuario para logar no CRM
            ponteCrm.Authenticate();   //.Authenticate pede para o dynamics validar o usuario
            RetrieveOrganizationsRequest perguntaCrm = new RetrieveOrganizationsRequest();   //RetrieveOrganizationsRequest é uma classe que faz a pergunta para o Dynamics
            perguntaCrm.AccessType = EndpointAccessType.Default;
            perguntaCrm.Release = OrganizationRelease.Current;   //quero pegar a organização mais recente dos dados

            RetrieveOrganizationsResponse respostaCrm = (RetrieveOrganizationsResponse)ponteCrm.Execute(perguntaCrm);    //a respostaCrm esta recbendo o valor da perginta que a ponteCrm fez
            foreach (var item in respostaCrm.Details)   // esta é a resposta
            {
                Console.WriteLine("Unique " + item.UniqueName);
                Console.WriteLine("Friendly " + item.FriendlyName);
                foreach (var endpoint in item.Endpoints)
                {
                    Console.WriteLine(endpoint.Key);
                    Console.WriteLine(endpoint.Value);
                }
            }
            Console.ReadKey();
        }
        #endregion

        #region Create
        static void MeuCreate(CrmServiceClient serviceProxy)
        {
            for (int i = 0; i < 10; i++)
            {
                var entidade = new Entity("account"); //Entiti representa uma tabela no banco de dados, e o accont diz qual é a tabela
                Guid registro = new Guid(); //Guid é uma chave do banco de dados
                String[] nome = new string[5] { "Osaka", "Game Set", "Premier Clube", "Mitelli Equipamentos", "Ace Zone" };


                entidade.Attributes.Add("name", nome);  //cria um registro no campo name com i valor especificado 
                // Criei no Dynamics

                registro = serviceProxy.Create(entidade);
            }
        }



        #endregion

        #region MeuUpdate

        static void MeuUpdate(CrmServiceClient serviceProxy)
        {
            for (int i = 0; i < 10; i++)
            {
                var entidade = new Entity("account");
                Guid idregistro = new Guid();

                var idTeste = Guid.NewGuid();
                entidade.Attributes.Add("accountid", idTeste);
                // Criei no Dynamics

                idregistro = serviceProxy.Create(entidade);

                if (idregistro == idTeste)
                    Console.WriteLine("igual");

                var registroDynamics = serviceProxy.Retrieve("account", idregistro, new ColumnSet("name"));
                if (registroDynamics.Attributes.Contains("name"))
                {

                    registroDynamics.Attributes["name"] = "Novo Valor " + i.ToString();
                }
                else
                {
                    registroDynamics.Attributes.Add("name", "Novo Valor " + i.ToString());
                }


                serviceProxy.Update(registroDynamics);

            }
        }


        #endregion

        #region Delete

        static void MeuDelete(CrmServiceClient serviceProxy)
        {
            for (int i = 0; i < 10; i++)
            {
                var entidade = new Entity("account");
                Guid idRegistro = new Guid();

                entidade.Attributes.Add("name", "Treinamento " + i.ToString());
                //Criei no Dyanamics

                idRegistro = serviceProxy.Create(entidade);

                serviceProxy.Delete("account", idRegistro);

            }
        }
        #endregion

        #region QueryExpression 1

        static EntityCollection RetornarMultiplo(CrmServiceClient serviceProxy)
        {
            QueryExpression queryExpression = new QueryExpression("account");

            queryExpression.Criteria.AddCondition("websiteurl", ConditionOperator.NotNull);
            queryExpression.ColumnSet = new ColumnSet("websiteurl");
            EntityCollection colecaoEtidades = serviceProxy.RetrieveMultiple(queryExpression);

            var retornoLista = RetornarMultiplo(serviceProxy);
            foreach (var item in retornoLista.Entities)
            {
                if (item.Attributes.Contains("websiteurl"))
                    Console.WriteLine(item["websiteurl"]);
                else
                    Console.WriteLine("Campo não encontrado");
            }
            Console.WriteLine("Po po por hoje é só pessoal");

            return colecaoEtidades;

        }
        #endregion

        #region QueryExpression 2
        static void RetornarMultipliComLink(CrmServiceClient serviceProxy)
        {
            QueryExpression queryExpression = new QueryExpression("account");
            queryExpression.ColumnSet = new ColumnSet(true);

            ConditionExpression condicao = new ConditionExpression("address1_city", ConditionOperator.Equal, "Natal");
            queryExpression.Criteria.AddCondition(condicao);

            LinkEntity link = new LinkEntity("account", "contact", "primarycontactid", "contactid", JoinOperator.Inner);
            link.Columns = new ColumnSet("firstname", "lastname");
            link.EntityAlias = "Contato";
            queryExpression.LinkEntities.Add(link);

            EntityCollection colecaoEntidades = serviceProxy.RetrieveMultiple(queryExpression);
            foreach (var entidade in colecaoEntidades.Entities)
            {
                Console.WriteLine("Id: " + entidade.Id);
                Console.WriteLine("Nome conta " + entidade["name"]);
                Console.WriteLine("Nome contato " + ((AliasedValue)entidade["Contato.firstname"]).Value);
                Console.WriteLine("Sobrenome Cotato " + ((AliasedValue)entidade["Contato.lastname"]).Value);
            }
        }

        #endregion

        #region Linq1
        static void ConsultaLinq(CrmServiceClient serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);

            var resultados = from a in context.CreateQuery("contact")
                             join b in context.CreateQuery("account")
                                    on a["contactid"] equals b["primarycontactid"]
                             //where a ["contactid"].ToString().Contains("en")
                             select new
                             {
                                 retorno = new
                                 {
                                     FirstName = a["firstname"],
                                     LastName = a["lastname"],
                                     NomeConta = b["name"]
                                 }
                             };
            foreach (var entidade in resultados)
            {
                Console.WriteLine("Nome : " + entidade.retorno.FirstName);
                Console.WriteLine("Sobrenome : " + entidade.retorno.LastName);
                Console.WriteLine("NomeConta : " + entidade.retorno.NomeConta);
            }



        }


        #endregion

        #region CRUD Linq

        static void CriacaoLinq(CrmServiceClient serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);
            for (int i = 0; i < 10; i++)
            {
                Entity account = new Entity("account");
                account["name"] = "Conta Linq" + i;
                context.AddObject(account); // adicionando um array de registros 

            }
            context.SaveChanges();
        }

        static void UpdateLinq(CrmServiceClient serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);
            var resultados = from a in context.CreateQuery("contact")
                             where ((string)a["firstname"]) == "Dan"
                             select a;

            foreach (var item in resultados)
            {
                item.Attributes["firstname"] = "Daniel Geraldeli";
                context.UpdateObject(item);
            }
            context.SaveChanges();
        }

        static void ExcluirLinq(CrmServiceClient serviceProxy)
        {
            OrganizationServiceContext context = new OrganizationServiceContext(serviceProxy);
            var resultados = from a in context.CreateQuery("account")
                             where ((string)a["name"]) == "Treinamento Extending 2"
                             select a;

            foreach (var item in resultados)
            {
                context.DeleteObject(item);

            }
            context.SaveChanges();
        }

        #endregion

        #region FetchXML
        static void FetchXML(CrmServiceClient serviceProxy)
        {

            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name= 'account'>
                                <attribute name='name' />
                                <attribute name='primarycontactid' />
                                <attribute name='telephone1'/>
                                <attribute name='accountid'/>
                                <attribute name='createdon'/>
                                <order attribute= 'name' descending='false'/>
                                <filter type = 'and'>
                                    <condition attribute= 'name' operator='eq' value='Tlsv Eng'/>
                                    <condition attribute= 'accountnumber' operator='not-null'/>
                                </filter>
                              </entity>
                            </fetch>";

            EntityCollection colecao = serviceProxy.RetrieveMultiple(new FetchExpression(query));
            foreach (var item in colecao.Entities)
            {
                Console.WriteLine(item["name"]);
            }
        }

        #endregion
        
        #region FetchXML2

        static void FetchXMLAgrefate(CrmServiceClient serviceProxy)
        {
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                <entity name='account'>
                                    <attribute name='creditlimit' alias='creditlimit_soma' aggregate='avg'/>
                                </entity>
                            </fetch>";

            EntityCollection colecao = serviceProxy.RetrieveMultiple(new FetchExpression(query));

            foreach (var item in colecao.Entities)
            {
                Console.WriteLine(((Money)((AliasedValue)item["creditlimit_soma"]).Value).Value.ToString());
            }
        }

        #endregion

        #region TratamentoDigitacao

        static string InicialMaiuscula(string texto)
        {
            string[] textoExplode = texto.Split(' '); //separa o nome em várias variáveis pelo espaço
            texto = "";
            for (int j = 0; j < textoExplode.Length; j++)
            {
                textoExplode[j] = textoExplode[j].ToLower(); //converte todas as palavras para minusculo

                if (textoExplode[j].Length > 2) //não realiza a função em palavras de 2 letras
                    textoExplode[j] = char.ToUpper(textoExplode[j][0]) + textoExplode[j].Substring(1); //coloca a inicial em maiuscula

                texto += textoExplode[j]; //reconstroi o nome
                if (j != (textoExplode.Length - 1)) //se for o último, não adiciona um nome ao final
                    texto += " ";
            }
            return texto;
        }



        #endregion


        #region TesteCreateConta

        static void TesteCreate(CrmServiceClient serviceProxy)
        {
            Console.WriteLine("Vamos fazer o teste de de cadastro de contas.");
            Console.WriteLine(" ");
            string resp = "s";
            while (resp == "s")
            {
                var entidade = new Entity("account");
                Guid registro = new Guid();
                
                Console.WriteLine("Qual o nome da empresa: ");
                string nome = Console.ReadLine();
                entidade.Attributes.Add("name", nome);


                Console.WriteLine("Qual o telefone: ");
                string fone1 = Console.ReadLine();
                entidade.Attributes.Add("telephone1", fone1);

                Console.WriteLine("Quer cadastrar mais alguma conta s/n ? ");
                resp = Console.ReadLine();

                registro = serviceProxy.Create(entidade);
            }
        }

        #endregion

        #region TesteCreateContato

        static void TesteCreateContct(CrmServiceClient serviceProxy)
        {
            Console.WriteLine("Vamos fazer o teste de cadastro de contas.\n");
            string resp = "S";
            while (resp == "S")
            {
                var entidade = new Entity("contact");
                Guid registro = new Guid();
                //Adição do nome do contado
                Console.WriteLine("Qual é o Primeiro nome: ");
                string nome1 = Console.ReadLine();
                entidade.Attributes.Add("firstname", nome1);

                

                //Adição do sobrenome do contado
                Console.WriteLine("Qual o sobrenome: ");
                string nome2 = Console.ReadLine();
                entidade.Attributes.Add("lastname", nome2);
                
                //Adição do sobrenome do contato
                Console.WriteLine("Qual o cargo: ");
                string cargo = Console.ReadLine();
                entidade.Attributes.Add("jobtitle", cargo);

                //Adição da conta filiada desse contato
                //Console.WriteLine("A qual conta está filiado: ");
                //string conta = Console.ReadLine();
                //entidade.Attributes.Add("parentcustomerid", conta);

                //Adiçãp de email
                Console.WriteLine("Qual o e-mail: ");
                string email = Console.ReadLine();
                entidade.Attributes.Add("emailaddress1", email);

                Console.WriteLine("Quer cadastrar mais alguma conta s/n ? ");
                resp = Console.ReadLine();
                resp = resp.ToUpper();


                registro = serviceProxy.Create(entidade);
            }
        }

        #endregion
    }
}




