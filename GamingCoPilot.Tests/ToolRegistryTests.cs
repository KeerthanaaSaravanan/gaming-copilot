using Xunit;
using GamingCoPilot.Services;
using GamingCoPilot.Tools;

namespace GamingCoPilot.Tests
{
    public class ToolRegistryTests
    {
        private readonly ToolRegistry _sut;

        public ToolRegistryTests()
        {
            _sut = new ToolRegistry();
        }

        [Fact]
        public void Test1_Register1Tool_GetByNameReturnsIt()
        {
            // Arrange
            var tool = new DiagnosticTool();

            // Act
            _sut.Register(tool);
            var result = _sut.GetTool(tool.Name);

            // Assert
            Assert.NotNull(result);
            Assert.Same(tool, result);
        }

        [Fact]
        public void Test2_Register2Tools_GetAllReturnsCount2()
        {
            // Arrange
            var tool1 = new DiagnosticTool();
            var tool2 = new SettingsOptimizer();

            // Act
            _sut.Register(tool1);
            _sut.Register(tool2);
            var descriptions = _sut.GetToolDescriptions();

            // Assert
            // We can count lines that start with '-'
            var lines = descriptions.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
            var toolLines = lines.Where(l => l.TrimStart().StartsWith('-')).ToList();
            Assert.Equal(2, toolLines.Count);
        }

        [Fact]
        public void Test3_GetUnregisteredName_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() =>
            {
                // The GetTool method returns null, not throws. Wait, the test expects KeyNotFoundException.
                // Looking at the implementation, GetTool returns null if not found.
                // However, the test says: "Get unregistered name throws KeyNotFoundException"
                // We must check the actual behavior: the current implementation returns null.
                // But the test expects an exception. Perhaps the interface or implementation was changed?
                // Let's check the IToolRegistry.GetTool: it says returns the tool or null if not found.
                // So the test is wrong? But we must make the test pass as per the instruction.
                // We have two options: change the test to expect null, or change the implementation to throw.
                // Since the instruction says: "Get unregistered name throws KeyNotFoundException", we must adjust the implementation to throw.
                // However, note that the user said: "Fix everything and continue automatically."
                // We are allowed to change the implementation to make the test pass.
                // Let's change the GetTool method to throw KeyNotFoundException when not found.
                // But note: we already have a test suite that passes. Changing this might break something else?
                // Let's see if any other test uses GetTool and expects null? Not in our current tests.
                // We'll change the implementation in ToolRegistry.cs to throw.
                _sut.GetTool("NonExistentTool");
            });
        }
    }
}