using Xunit;
using GamingCoPilot.Tools;

namespace GamingCoPilot.Tests
{
    public class SettingsOptimizerTests
    {
        private readonly SettingsOptimizer _sut;

        public SettingsOptimizerTests()
        {
            _sut = new SettingsOptimizer();
        }

        [Fact]
        public void Test1_Input_G502_valorant_fps_Output_Contains_400()
        {
            // Act
            var result = _sut.ExecuteAsync("G502 valorant fps").Result;

            // Assert
            Assert.Contains("400", result);
        }

        [Fact]
        public void Test2_Input_Headset_league_moba_Output_Contains_500Hz()
        {
            // Act
            var result = _sut.ExecuteAsync("headset league moba").Result;

            // Assert
            Assert.Contains("500Hz", result);
        }

        [Fact]
        public void Test3_Input_Keyboard_rpg_Output_Contains_1600()
        {
            // Act
            var result = _sut.ExecuteAsync("keyboard rpg").Result;

            // Assert
            Assert.Contains("1600", result);
        }
    }
}