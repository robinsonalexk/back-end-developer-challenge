using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CharacterManagerAPI.Hubs;
using CharManagerAPI.Models;
using CharManagerAPI.Models.Enums;
using CharManagerAPI.Repositories;
using CharManagerAPI.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace CharManagerAPITests
{
    public class CharacterServiceTests
    {
        private const string Name = "Ryu";

        private readonly Mock<ICharacterRepository> _repo = new();
        private readonly Mock<IHubContext<CharacterHub, ICharacterClient>> _hub = new();
        private readonly Mock<IHubClients<ICharacterClient>> _clients = new();
        private readonly Mock<ICharacterClient> _allClients = new();
        private readonly CharacterService _sut;

        public CharacterServiceTests()
        {
            _hub.Setup(h => h.Clients).Returns(_clients.Object);
            _clients.Setup(c => c.All).Returns(_allClients.Object);
            _allClients.Setup(c => c.CharacterUpserted(It.IsAny<Character>())).Returns(Task.CompletedTask);

            _sut = new CharacterService(_repo.Object, _hub.Object);
        }

        // helpers

        private Character SetupCharacter(
            int maxHp = 50,
            int currentHp = 30,
            int tempHp = 0,
            List<Defenses>? defenses = null,
            bool updateResult = true)
        {
            var character = new Character
            {
                Name = Name,
                HitPoints = maxHp,
                CurrentHitPoints = currentHp,
                TempHitPoints = tempHp,
                Defenses = defenses ?? new List<Defenses>()
            };

            _repo.Setup(r => r.GetByIdAsync(Name)).ReturnsAsync(character);
            _repo.Setup(r => r.UpdateAsync(character)).ReturnsAsync(updateResult);
            return character;
        }

        private void SetupCharacterNotFound()
        {
            _repo.Setup(r => r.GetByIdAsync(Name)).ReturnsAsync((Character?)null);
        }

        private static Defenses Defense(DamageType type, DefenseType defense) =>
            new() { Type = type, Defense = defense };

        private void VerifyPersistedAndBroadcast(Character character)
        {
            _repo.Verify(r => r.UpdateAsync(character), Times.Once);
            _allClients.Verify(c => c.CharacterUpserted(character), Times.Once);
        }

        private void VerifyNoBroadcast()
        {
            _allClients.Verify(c => c.CharacterUpserted(It.IsAny<Character>()), Times.Never);
        }

        // DealDamageAsync

        [Fact]
        public async Task DealDamage_NoTempHp_ReducesCurrentHp()
        {
            var character = SetupCharacter(currentHp: 30, tempHp: 0);

            var result = await _sut.DealDamageAsync(Name, 10, DamageType.Fire);

            Assert.True(result);
            Assert.Equal(20, character.CurrentHitPoints);
            Assert.Equal(0, character.TempHitPoints);
            VerifyPersistedAndBroadcast(character);
        }

        [Fact]
        public async Task DealDamage_FullyAbsorbedByTempHp_OnlyTempHpReduced()
        {
            var character = SetupCharacter(currentHp: 30, tempHp: 10);

            await _sut.DealDamageAsync(Name, 4, DamageType.Fire);

            Assert.Equal(30, character.CurrentHitPoints);
            Assert.Equal(6, character.TempHitPoints);
        }

        [Fact]
        public async Task DealDamage_EqualToTempHp_TempHpZeroAndCurrentHpUnchanged()
        {
            var character = SetupCharacter(currentHp: 30, tempHp: 10);

            await _sut.DealDamageAsync(Name, 10, DamageType.Fire);

            Assert.Equal(30, character.CurrentHitPoints);
            Assert.Equal(0, character.TempHitPoints);
        }

        [Fact]
        public async Task DealDamage_ExceedsTempHp_OverflowCarriesToCurrentHp()
        {
            var character = SetupCharacter(currentHp: 30, tempHp: 5);

            await _sut.DealDamageAsync(Name, 12, DamageType.Fire);

            Assert.Equal(0, character.TempHitPoints);
            Assert.Equal(23, character.CurrentHitPoints);
        }

        [Theory]
        [InlineData(30, 0, 100)]
        [InlineData(10, 0, 10)]
        [InlineData(10, 5, 50)]
        public async Task DealDamage_Lethal_CurrentHpClampedAtZero(int currentHp, int tempHp, int damage)
        {
            var character = SetupCharacter(currentHp: currentHp, tempHp: tempHp);

            await _sut.DealDamageAsync(Name, damage, DamageType.Fire);

            Assert.Equal(0, character.CurrentHitPoints);
            Assert.Equal(0, character.TempHitPoints);
        }

        [Fact]
        public async Task DealDamage_ZeroAmount_NothingChanges()
        {
            var character = SetupCharacter(currentHp: 30, tempHp: 5);

            var result = await _sut.DealDamageAsync(Name, 0, DamageType.Fire);

            Assert.True(result);
            Assert.Equal(30, character.CurrentHitPoints);
            Assert.Equal(5, character.TempHitPoints);
        }

        [Fact]
        public async Task DealDamage_Immunity_TakesNoDamage()
        {
            var character = SetupCharacter(
                currentHp: 30, tempHp: 5,
                defenses: new() { Defense(DamageType.Fire, DefenseType.Immunity) });

            var result = await _sut.DealDamageAsync(Name, 20, DamageType.Fire);

            Assert.True(result);
            Assert.Equal(30, character.CurrentHitPoints);
            Assert.Equal(5, character.TempHitPoints);
            VerifyPersistedAndBroadcast(character);
        }

        [Theory]
        [InlineData(10, 5)]
        [InlineData(11, 5)]
        [InlineData(1, 0)]
        [InlineData(0, 0)]
        public async Task DealDamage_Resistance_HalvesDamageRoundedDown(int damage, int expectedTaken)
        {
            var character = SetupCharacter(
                currentHp: 30, tempHp: 0,
                defenses: new() { Defense(DamageType.Fire, DefenseType.Resistance) });

            await _sut.DealDamageAsync(Name, damage, DamageType.Fire);

            Assert.Equal(30 - expectedTaken, character.CurrentHitPoints);
        }

        [Fact]
        public async Task DealDamage_ImmunityAndResistance_ImmunityWins()
        {
            var character = SetupCharacter(
                currentHp: 30,
                defenses: new()
                {
                    Defense(DamageType.Fire, DefenseType.Resistance),
                    Defense(DamageType.Fire, DefenseType.Immunity)
                });

            await _sut.DealDamageAsync(Name, 20, DamageType.Fire);

            Assert.Equal(30, character.CurrentHitPoints);
        }

        [Fact]
        public async Task DealDamage_DefenseForDifferentDamageType_IsIgnored()
        {
            var character = SetupCharacter(
                currentHp: 30,
                defenses: new()
                {
                    Defense(DamageType.Cold, DefenseType.Immunity),
                    Defense(DamageType.Cold, DefenseType.Resistance)
                });

            await _sut.DealDamageAsync(Name, 10, DamageType.Fire);

            Assert.Equal(20, character.CurrentHitPoints);
        }

        [Fact]
        public async Task DealDamage_ResistanceAppliedBeforeTempHpAbsorption()
        {
            var character = SetupCharacter(
                currentHp: 30, tempHp: 3,
                defenses: new() { Defense(DamageType.Fire, DefenseType.Resistance) });

            await _sut.DealDamageAsync(Name, 10, DamageType.Fire);

            Assert.Equal(0, character.TempHitPoints);
            Assert.Equal(28, character.CurrentHitPoints);
        }

        [Fact]
        public async Task DealDamage_UpdateFails_ReturnsFalseAndDoesNotBroadcast()
        {
            var character = SetupCharacter(currentHp: 30, updateResult: false);

            var result = await _sut.DealDamageAsync(Name, 10, DamageType.Fire);

            Assert.False(result);
            _repo.Verify(r => r.UpdateAsync(character), Times.Once);
            VerifyNoBroadcast();
        }

        [Fact]
        public async Task DealDamage_CharacterNotFound_ThrowsAndDoesNotUpdate()
        {
            SetupCharacterNotFound();

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DealDamageAsync(Name, 10, DamageType.Fire));

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Character>()), Times.Never);
            VerifyNoBroadcast();
        }

        // HealDamageAsync

        [Fact]
        public async Task Heal_BelowMax_IncreasesCurrentHp()
        {
            var character = SetupCharacter(maxHp: 50, currentHp: 20);

            var result = await _sut.HealDamageAsync(Name, 10);

            Assert.True(result);
            Assert.Equal(30, character.CurrentHitPoints);
            VerifyPersistedAndBroadcast(character);
        }

        [Fact]
        public async Task Heal_ExactlyToMax_SetsToMax()
        {
            var character = SetupCharacter(maxHp: 50, currentHp: 40);

            await _sut.HealDamageAsync(Name, 10);

            Assert.Equal(50, character.CurrentHitPoints);
        }

        [Theory]
        [InlineData(50, 50, 5)]
        [InlineData(45, 50, 100)]
        [InlineData(49, 50, 2)]
        public async Task Heal_Overheal_ClampedToMaxHp(int currentHp, int maxHp, int amount)
        {
            var character = SetupCharacter(maxHp: maxHp, currentHp: currentHp);

            await _sut.HealDamageAsync(Name, amount);

            Assert.Equal(maxHp, character.CurrentHitPoints);
        }

        [Fact]
        public async Task Heal_DoesNotAffectTempHp()
        {
            var character = SetupCharacter(maxHp: 50, currentHp: 20, tempHp: 7);

            await _sut.HealDamageAsync(Name, 10);

            Assert.Equal(7, character.TempHitPoints);
        }

        [Fact]
        public async Task Heal_ZeroAmount_NoChange()
        {
            var character = SetupCharacter(maxHp: 50, currentHp: 20);

            var result = await _sut.HealDamageAsync(Name, 0);

            Assert.True(result);
            Assert.Equal(20, character.CurrentHitPoints);
        }

        [Fact]
        public async Task Heal_UpdateFails_ReturnsFalseAndDoesNotBroadcast()
        {
            SetupCharacter(maxHp: 50, currentHp: 20, updateResult: false);

            var result = await _sut.HealDamageAsync(Name, 10);

            Assert.False(result);
            VerifyNoBroadcast();
        }

        [Fact]
        public async Task Heal_CharacterNotFound_ThrowsAndDoesNotUpdate()
        {
            SetupCharacterNotFound();

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.HealDamageAsync(Name, 10));

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Character>()), Times.Never);
            VerifyNoBroadcast();
        }

        // AddTempHpAsync

        [Fact]
        public async Task AddTempHp_NoExistingTemp_SetsTempHp()
        {
            var character = SetupCharacter(tempHp: 0);

            var result = await _sut.AddTempHpAsync(Name, 8);

            Assert.True(result);
            Assert.Equal(8, character.TempHitPoints);
            VerifyPersistedAndBroadcast(character);
        }

        [Fact]
        public async Task AddTempHp_HigherThanExisting_ReplacesExisting()
        {
            var character = SetupCharacter(tempHp: 5);

            await _sut.AddTempHpAsync(Name, 8);

            Assert.Equal(8, character.TempHitPoints);
        }

        [Theory]
        [InlineData(10, 5)]
        [InlineData(10, 10)]
        [InlineData(10, 0)]
        public async Task AddTempHp_LowerOrEqualToExisting_KeepsExisting(int existing, int amount)
        {
            var character = SetupCharacter(tempHp: existing);

            var result = await _sut.AddTempHpAsync(Name, amount);

            Assert.True(result);
            Assert.Equal(existing, character.TempHitPoints);
        }

        [Fact]
        public async Task AddTempHp_DoesNotAffectCurrentHp()
        {
            var character = SetupCharacter(currentHp: 30, tempHp: 0);

            await _sut.AddTempHpAsync(Name, 8);

            Assert.Equal(30, character.CurrentHitPoints);
        }

        [Fact]
        public async Task AddTempHp_UpdateFails_ReturnsFalseAndDoesNotBroadcast()
        {
            SetupCharacter(tempHp: 0, updateResult: false);

            var result = await _sut.AddTempHpAsync(Name, 8);

            Assert.False(result);
            VerifyNoBroadcast();
        }

        [Fact]
        public async Task AddTempHp_CharacterNotFound_ThrowsAndDoesNotUpdate()
        {
            SetupCharacterNotFound();

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.AddTempHpAsync(Name, 8));

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Character>()), Times.Never);
            VerifyNoBroadcast();
        }
    }
}