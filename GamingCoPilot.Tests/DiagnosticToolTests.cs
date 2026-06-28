using Xunit;
using GamingCoPilot.Tools;

namespace GamingCoPilot.Tests
{
    public class DiagnosticToolTests
    {
        private readonly DiagnosticTool _sut;

        public DiagnosticToolTests()
        {
            _sut = new DiagnosticTool();
        }

        [Fact]
        public void Test1_Input_DoubleClick_Output_Contains_SwitchDebounce()
        {
            // Act
            var result = _sut.ExecuteAsync("double click").Result;

            // Assert
            Assert.Contains("Switch debounce", result);
        }

        [Fact]
        public void Test2_Input_MicNotWorking_Output_Contains_Mute()
        {
            // Act
            var result = _sut.ExecuteAsync("mic not working").Result;

            // Assert
            Assert.Contains("Mute", result);
        }

        [Fact]
        public void Test3_Input_CursorDrift_Output_Contains_Sensor()
        {
            // Act
            var result = _sut.ExecuteAsync("cursor drift").Result;

            // Assert
            Assert.Contains("Sensor", result);
        }

        [Fact]
        public void Test4_Input_AudioCutting_Output_Contains_Wireless()
        {
            // Act
            var result = _sut.ExecuteAsync("audio cutting").Result;

            // Assert
            Assert.Contains("Wireless", result);
        }

        [Fact]
        public void Test5_Input_FpsDrops_Output_Contains_Dpi()
        {
            // Act
            var result = _sut.ExecuteAsync("fps drops").Result;

            // Assert
            Assert.Contains("DPI", result);
        }
    }
}