using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TrueLayer.Api.ViewModels;
using Xunit;

namespace TrueLayer.Api.Tests.Acceptance
{
    public class PokemonControllerTests : IClassFixture<TestAppFactory>
    {
        private readonly TestAppFactory _factory;

        public PokemonControllerTests(TestAppFactory testAppFactory)
        {
            _factory = testAppFactory;
        }
        
        [Fact]
        public async Task GetPokemonInformation_PokemonDoesntExist_ReturnsNotFound()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("pokemon/truelayasaur");
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetPokemonInformation_PokemonExists_ReturnsOk()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("pokemon/mewtwo");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<PokemonViewModel>();
            
            Assert.NotNull(json);
            Assert.Equal("mewtwo", json.Name);
        }

        [Fact]
        public async Task GetTranslatedPokemonInformation_PokemonDoesntExist_ReturnsNotFound()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("pokemon/translated/truelayasaur");
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetTranslatedPokemonInformation_PokemonExists_ReturnsOk()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("pokemon/translated/mewtwo");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<PokemonViewModel>();
            
            Assert.NotNull(json);
            Assert.Equal("mewtwo", json.Name);
        }

        [Fact]
        public async Task TranslatedAndUntranslatedPokemonDescriptionsDiffer()
        {
            var client = _factory.CreateClient();

            var untranslatedResponse = await client.GetAsync("pokemon/mewtwo");
            var untranslatedPokemon = await untranslatedResponse
                .EnsureSuccessStatusCode()
                .Content
                .ReadFromJsonAsync<PokemonViewModel>();
            
            var translatedResponse = await client.GetAsync("pokemon/translated/mewtwo");
            var translatedPokemon = await translatedResponse
                .EnsureSuccessStatusCode()
                .Content
                .ReadFromJsonAsync<PokemonViewModel>();
            
            Assert.NotEqual(untranslatedPokemon.Description, translatedPokemon.Description);
        }

    }
}