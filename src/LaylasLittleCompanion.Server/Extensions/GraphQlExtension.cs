using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace LaylasLittleCompanion.Server.Extensions
{
	public static class GraphQlExtension
	{
		public static void AddGraphQLClient(this IServiceCollection services)
		{
			var graphQlClient = new GraphQLHttpClient(
				 "https://graphql.contentful.com/content/v1/spaces/5gi0c2vvptsm/environments/master",
				new NewtonsoftJsonSerializer());
			graphQlClient.HttpClient.DefaultRequestHeaders.Authorization
			= new AuthenticationHeaderValue("Bearer", "Tk0jMX-Z6addZMiJpd0WR_wN57LBUv6oQOScfU2n3qA");

			services.AddSingleton(sp => graphQlClient);
		}
	}
}
