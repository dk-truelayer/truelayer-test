using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TrueLayer.Api.Features.PokemonCache.Memory;
using TrueLayer.Api.Models;
using Xunit;

namespace TrueLayer.Api.Tests.Features.PokemonCache
{
    public class MemoryCacheTests
    {
        private class Fixture
        {
            public MemoryCacheOptions MemoryCacheOptions { get; } = new MemoryCacheOptions();

            public MemoryCache MemoryCache => new MemoryCache(
                new OptionsWrapper<MemoryCacheOptions>(MemoryCacheOptions));

            public MemoryPokemonCache Sut => new MemoryPokemonCache(MemoryCache);
        }

        private readonly Fixture _fixture = new Fixture();
        
        private Pokemon Pokemon => new Pokemon("bulbasaur", "Description", "grassland", false);

        private Pokemon PokemonTranslated => Pokemon with {Description = "Translated "};

        [Fact]
        public void Get_NoPokemonInCache_ReturnsNull()
        {
            var result = _fixture.Sut.Get(Pokemon.Name);
            Assert.Null(result);
        }

        [Fact]
        public void Get_NoPokemonWithNameInCache_ReturnsNull()
        {
            var sut = _fixture.Sut;
            sut.Set(Pokemon with { Name = "Not this" });
            var result = sut.Get(Pokemon.Name);
            Assert.Null(result);
        }

        [Fact]
        public void Get_OnlyTranslatedPokemonWithNameInCache_ReturnsNull()
        {
            var sut = _fixture.Sut;
            sut.SetTranslated(PokemonTranslated);
            var result = sut.Get(Pokemon.Name);
            Assert.Null(result);
        }

        [Fact]
        public void Get_PokemonInCache_IsReturned()
        {
            var sut = _fixture.Sut;
            sut.Set(Pokemon);
            var result = sut.Get(Pokemon.Name);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetTranslated_NoPokemonInCache_ReturnsNull()
        {
            var result = _fixture.Sut.GetTranslated(Pokemon.Name);
            Assert.Null(result);
        }
        
        [Fact]
        public void GetTranslated_NoPokemonWithNameInCache_ReturnsNull()
        {
            var sut = _fixture.Sut;
            sut.SetTranslated(Pokemon with { Name = "Not this" });
            var result = sut.GetTranslated(Pokemon.Name);
            Assert.Null(result);
        }

        [Fact]
        public void GetTranslated_OnlyUntranslatedInCache_ReturnsNull()
        {
            var sut = _fixture.Sut;
            sut.Set(Pokemon);
            var result = sut.GetTranslated(Pokemon.Name);
            Assert.Null(result);
        }

        [Fact]
        public void GetTranslated_PokemonInCache_IsReturned()
        {
            var sut = _fixture.Sut;
            sut.SetTranslated(Pokemon);
            var result = sut.GetTranslated(Pokemon.Name);
            Assert.NotNull(result);
        }

        [Fact]
        public void Set_NoRecord_NewRecordIsCreated()
        {
            var sut = _fixture.Sut;
            sut.Set(Pokemon);
            
            Assert.NotNull(sut.Get(Pokemon.Name));
            Assert.Null(sut.GetTranslated(Pokemon.Name));
        }

        [Fact]
        public void Set_ExistingUntranslatedRecord_Overwritten()
        {
            var sut = _fixture.Sut;
            sut.Set(Pokemon with { Description = "old" });
            
            sut.Set(Pokemon);
            
            Assert.NotEqual("old", sut.Get(Pokemon.Name)!.Description);
        }

        [Fact]
        public void Set_ExistingTranslatedRecord_TranslatedRecordUntouched()
        {
            var sut = _fixture.Sut;
            sut.SetTranslated(PokemonTranslated);
            sut.Set(Pokemon);
            Assert.NotNull(sut.GetTranslated(Pokemon.Name));
        }

        [Fact]
        public void SetTranslated_NoRecord_NewRecordIsCreated()
        {
            var sut = _fixture.Sut;
            sut.SetTranslated(PokemonTranslated);
            
            Assert.NotNull(sut.GetTranslated(Pokemon.Name));
        }

        [Fact]
        public void SetTranslated_ExistingTranslatedRecord_Overwritten()
        {
            var sut = _fixture.Sut;
            sut.SetTranslated(PokemonTranslated with { Description = "old" });
            sut.SetTranslated(PokemonTranslated);
            Assert.NotEqual("old", sut.GetTranslated(Pokemon.Name)!.Description);
        }

        [Fact]
        public void SetTranslated_ExistingUntranslatedRecord_UntranslatedUntouched()
        {
            var sut = _fixture.Sut;
            sut.Set(Pokemon);
            sut.SetTranslated(PokemonTranslated);
            Assert.NotNull(sut.Get(Pokemon.Name));
        }
    }
}