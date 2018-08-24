using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.OData.OpenAPI;
using NJsonSchema;
using NSwag;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;


namespace odata2openapi
{
    class Program
    {
        
        static string GetDynamicsMetadata (IConfiguration Configuration )
        {
            string dynamicsOdataUri = Configuration["DYNAMICS_ODATA_URI"];
            string aadTenantId = Configuration["DYNAMICS_AAD_TENANT_ID"];
            string serverAppIdUri = Configuration["DYNAMICS_SERVER_APP_ID_URI"];
            string clientKey = Configuration["DYNAMICS_CLIENT_KEY"];
            string clientId = Configuration["DYNAMICS_CLIENT_ID"];

            var authenticationContext = new AuthenticationContext(
                "https://login.windows.net/" + aadTenantId);
                ClientCredential clientCredential = new ClientCredential(clientId, clientKey);
            var task = authenticationContext.AcquireTokenAsync(serverAppIdUri, clientCredential);
            task.Wait();
            var authenticationResult = task.Result;

            string result = null;
            var webRequest = WebRequest.Create(dynamicsOdataUri + "$metadata");
            HttpWebRequest request = (HttpWebRequest)webRequest;
            request.Method = "GET";
            //request.PreAuthenticate = true;
            request.Headers.Add("Authorization", authenticationResult.CreateAuthorizationHeader());
            //request.Accept = "application/json;odata=verbose";
            request.ContentType =  "application/json";

            // we need to add authentication to a HTTP Client to fetch the file.
            using (
                MemoryStream ms = new MemoryStream())
            {
                request.GetResponse().GetResponseStream().CopyTo(ms);
                var buffer = ms.ToArray();
                result = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }

            return result;

        }

        static void Main(string[] args)
        {
            bool getMetadata = false;

            // start by getting secrets.
            var builder = new ConfigurationBuilder()                
                .AddEnvironmentVariables();

            builder.AddUserSecrets<Program>();           
            var Configuration = builder.Build();
            string csdl;
            // get the metadata.
            if (getMetadata)
            {
                csdl = GetDynamicsMetadata(Configuration);
                File.WriteAllText("C:\\tmp\\dynamics-metadata.xml", csdl);
            }
            else
            {
                csdl = File.ReadAllText("C:\\tmp\\dynamics-metadata.xml");
            }               

            // fix the csdl.

            csdl = csdl.Replace("ConcurrencyMode=\"Fixed\"", "");

            Microsoft.OData.Edm.IEdmModel model = Microsoft.OData.Edm.Csdl.CsdlReader.Parse(XElement.Parse(csdl).CreateReader());

            OpenApiTarget target = OpenApiTarget.Json;
            OpenApiWriterSettings settings = new OpenApiWriterSettings
            {
                BaseUri = new Uri(Configuration["DYNAMICS_ODATA_URI"])
            };
           
            string swagger = null;

            using (MemoryStream ms = new MemoryStream())
            {
                model.WriteOpenApi(ms, target, settings);
                var buffer = ms.ToArray();
                string temp = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                // The Microsoft OpenAPI.Net library doesn't seem to work with MS Dynamics metadata, so we use NSwag here.

                var runner = SwaggerDocument.FromJsonAsync(temp);
                runner.Wait();
                var swaggerDocument = runner.Result;

                // fix the operationIds.
                foreach (var operation in swaggerDocument.Operations)
                {
                    string suffix = "";
                    switch (operation.Method)
                    {
                        case SwaggerOperationMethod.Post:
                            suffix = "Create";
                            // for creates we also want to add a header parameter to ensure we get the new object back.
                            SwaggerParameter swaggerParameter = new SwaggerParameter()
                            {
                                Type = JsonObjectType.String,
                                Name = "Prefer",
                                Default = "return=representation",
                                Description = "Required in order for the service to return a JSON representation of the object.",
                                Kind = SwaggerParameterKind.Header
                            };
                            
                            break;

                        case SwaggerOperationMethod.Patch:
                            suffix = "Update";
                            break;

                        case SwaggerOperationMethod.Put:
                            suffix = "Put";
                            break;

                        case SwaggerOperationMethod.Delete:
                            suffix = "Delete";
                            break;

                        case SwaggerOperationMethod.Get:
                            if (operation.Path.Contains("{"))
                            {
                                suffix = "GetByKey";
                            }
                            else
                            {
                                suffix = "Get";
                            }                            
                            break;
                    }

                    string prefix = "Unknown";
                    string firstTag = operation.Operation.Tags.FirstOrDefault();
                    if (firstTag != null)
                    {
                        prefix = firstTag;
                        // Capitalize the first character.
                        if (prefix.Length > 0)
                        {
                            prefix = ("" + prefix[0]).ToUpper() + prefix.Substring(1);
                        }
                        // remove any underscores.
                        prefix = prefix.Replace("_", "");
                    }

                    operation.Operation.OperationId = prefix + "_" + suffix;
                }                

                swagger = swaggerDocument.ToJson(SchemaType.Swagger2);

                // fix up the swagger file.

                swagger = swagger.Replace("('{", "({");
                swagger = swagger.Replace("}')", "})");
                
                // NSwag is almost able to generate the client as well.  It does it much faster than AutoRest but unfortunately can't do multiple files yet.
                // It does generate a single very large file in a few seconds.  We don't want a single very large file though as it does not work well with the IDE.
                // Autorest is used to generate the client using a traditional approach.

                /*
                var generatorSettings = new SwaggerToCSharpClientGeneratorSettings
                {
                    ClassName = "DynamicsClient",
                    CSharpGeneratorSettings =
                    {
                        Namespace = "<namespace>"
                    }
                };                

                var generator = new SwaggerToCSharpClientGenerator(swaggerDocument, generatorSettings);
                var code = generator.GenerateFile();

                File.WriteAllText("<filename>", code);
                */

            }

            Console.Out.WriteLine(swagger);
        }
    }
}
