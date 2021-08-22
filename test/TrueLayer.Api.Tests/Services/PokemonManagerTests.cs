using TrueLayer.Api.Features.Translation;
using TrueLayer.Api.Models;
using TrueLayer.Api.Services;
using Xunit;

namespace TrueLayer.Api.Tests.Services
{
    public class PokemonManagerTests
    {
        private class Fixture
        {
            public PokemonManager Sut => new PokemonManager();
        }

        private readonly Fixture _fixture = new Fixture();
        
        private Pokemon Pokemon => new Pokemon("bulbasaur", "Description", "grassland", false);
        
        [Fact]
        public void ChooseTranslationLanguage_PokemonIsCave_IsYoda()
        {
            var testPokemon = Pokemon with {Habitat = "cave"};
            var result = _fixture.Sut.ChooseTranslationLanguage(testPokemon);

            Assert.Equal(TranslationLanguage.Yoda, result);
        }

        [Fact]
        public void ChooseTranslationLanguage_PokemonIsLegendary_IsYoda()
        {
            var testPokemon = Pokemon with {IsLegendary = true};
            var result = _fixture.Sut.ChooseTranslationLanguage(testPokemon);
            
            Assert.Equal(TranslationLanguage.Yoda, result);
        }

        [Fact]
        public void ChooseTranslationLanguage_PokemonIsNeitherCaveNorLegendary_IsShakespeare()
        {
            var testPokemon = Pokemon with {Habitat = "grassland", IsLegendary = false};

            var result = _fixture.Sut.ChooseTranslationLanguage(testPokemon);
            
            Assert.Equal(TranslationLanguage.Shakespeare, result);
        }

        [Fact]
        public void SetDescription_NewDescriptionIsNull_ReturnsPokemonWithSameDescription()
        {
            var pokemon = Pokemon;
            var updated = _fixture.Sut.UpdateDescription(pokemon, null);
            Assert.Equal(pokemon.Description, updated.Description);
        }

        [Fact]
        public void SetDescription_NewDescriptionIsString_ReturnsPokemonWithNewDescription()
        {
            const string newDescription = "New description";
            var pokemon = Pokemon;
            
            var updated = _fixture.Sut.UpdateDescription(pokemon, newDescription);
            
            Assert.Equal(newDescription, updated.Description);
        }

        [Fact]
        public void ToViewModel_PokemonIsNull_ReturnsNull()
        {
            var result = _fixture.Sut.ToViewModel(null);
            Assert.Null(result);
        }

        [Fact]
        public void ToViewModel_PokemonIsMapped()
        {
            var pokemon = Pokemon;
            var result = _fixture.Sut.ToViewModel(pokemon);
            
            Assert.Equal(pokemon.Name, result.Name);
            Assert.Equal(pokemon.Habitat, result.Habitat);
            Assert.Equal(pokemon.Description, result.Description);
            Assert.Equal(pokemon.IsLegendary, result.IsLegendary);
        }
    }
}