using Xunit;
using GamingCoPilot.Agent;
using GamingCoPilot.Models;

namespace GamingCoPilot.Tests
{
    public class AgentMemoryTests
    {
        private readonly AgentMemory _sut;

        public AgentMemoryTests()
        {
            _sut = new AgentMemory();
        }

        [Fact]
        public void Test1_Add3Turns_GetHistoryReturns3Items()
        {
            // Act
            _sut.AddMessage(new Message("user", "Hello"));
            _sut.AddMessage(new Message("assistant", "Hi there!"));
            _sut.AddMessage(new Message("user", "How are you?"));

            // Assert
            Assert.Equal(3, _sut.GetHistory().Count);
        }

        [Fact]
        public void Test2_Clear_ResetsHistoryTo0Items()
        {
            // Arrange
            _sut.AddMessage(new Message("user", "Test"));
            _sut.AddMessage(new Message("assistant", "Test"));

            // Act
            _sut.Clear();

            // Assert
            Assert.Equal(0, _sut.GetHistory().Count);
        }

        [Fact]
        public void Test3_FirstTurnRoleIsUser()
        {
            // Act
            _sut.AddMessage(new Message("user", "First message"));
            _sut.AddMessage(new Message("assistant", "Response"));

            // Assert
            Assert.Equal("user", _sut.GetHistory()[0].Role);
        }
    }
}