using BCC.MSBuildLog.Services;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BCC.MSBuildLog.Tests.Services
{
    public class CommandLineParserTests
    {
        private static readonly Faker Faker;

        static CommandLineParserTests()
        {
            Faker = new Faker();
        }

        [Fact]
        public void ShouldCallForHelpIfNothingSent()
        {
            var listener = Substitute.For<ICommandLineParserCallBackListener>();
            var commandLineParser = new CommandLineParser(listener.Callback);
            var applicationArguments = commandLineParser.Parse(new string[0]);

            applicationArguments.Should().BeNull();

            listener.Received(1).Callback(Arg.Any<string>());
        }

        [Fact]
        public void ShouldParseForHelpIfNothingSent()
        {
            var listener = Substitute.For<ICommandLineParserCallBackListener>();
            var commandLineParser = new CommandLineParser(listener.Callback);
            var applicationArguments = commandLineParser.Parse(new []{"-?"});

            applicationArguments.Should().BeNull();

            listener.Received(1).Callback(Arg.Any<string>());
        }

        [Fact]
        public void ShouldRequireRequiredArguments()
        {
            var listener = Substitute.For<ICommandLineParserCallBackListener>();
            var commandLineParser = new CommandLineParser(listener.Callback);

            var inputPath = Faker.System.FilePath();
            var outputPath = Faker.System.FilePath();
            var cloneRoot = Faker.System.DirectoryPath();

            var applicationArguments = commandLineParser.Parse(new[]
            {
                "-i", $@"""{inputPath}""",
                "-o", $@"""{outputPath}""",
                "-c", $@"""{cloneRoot}"""
            });

            listener.DidNotReceive().Callback(Arg.Any<string>());

            applicationArguments.Should().NotBeNull();
            applicationArguments.InputFile.Should().Be(inputPath);
            applicationArguments.OutputFile.Should().Be(outputPath);
            applicationArguments.CloneRoot.Should().Be(cloneRoot);
            applicationArguments.ConfigurationFile.Should().BeNull();

            listener = Substitute.For<ICommandLineParserCallBackListener>();
            commandLineParser = new CommandLineParser(listener.Callback);

            applicationArguments = commandLineParser.Parse(new[]
            {
                "--input", $@"""{inputPath}""",
                "--output", $@"""{outputPath}""",
                "--cloneRoot", $@"""{cloneRoot}"""
            });

            listener.DidNotReceive().Callback(Arg.Any<string>());

            applicationArguments.Should().NotBeNull();
            applicationArguments.InputFile.Should().Be(inputPath);
            applicationArguments.OutputFile.Should().Be(outputPath);
            applicationArguments.CloneRoot.Should().Be(cloneRoot);
            applicationArguments.ConfigurationFile.Should().BeNull();
        }

        [Fact]
        public void ShouldBeAbleToSetConfigurationArgument()
        {
            var listener = Substitute.For<ICommandLineParserCallBackListener>();
            var commandLineParser = new CommandLineParser(listener.Callback);

            var inputPath = Faker.System.FilePath();
            var configurationFile = Faker.System.FilePath();
            var outputPath = Faker.System.FilePath();
            var cloneRoot = Faker.System.DirectoryPath();

            var applicationArguments = commandLineParser.Parse(new[]
            {
                "-i", $@"""{inputPath}""",
                "-o", $@"""{outputPath}""",
                "-c", $@"""{cloneRoot}""",
                "--configuration", $@"""{configurationFile}"""
            });

            listener.DidNotReceive().Callback(Arg.Any<string>());

            applicationArguments.Should().NotBeNull();
            applicationArguments.InputFile.Should().Be(inputPath);
            applicationArguments.OutputFile.Should().Be(outputPath);
            applicationArguments.CloneRoot.Should().Be(cloneRoot);
            applicationArguments.ConfigurationFile.Should().Be(configurationFile);

            applicationArguments = commandLineParser.Parse(new[]
            {
                "--input", $@"""{inputPath}""",
                "--output", $@"""{outputPath}""",
                "--cloneRoot", $@"""{cloneRoot}""",
                "--configuration", $@"""{configurationFile}"""
            });

            listener.DidNotReceive().Callback(Arg.Any<string>());

            applicationArguments.Should().NotBeNull();
            applicationArguments.InputFile.Should().Be(inputPath);
            applicationArguments.OutputFile.Should().Be(outputPath);
            applicationArguments.CloneRoot.Should().Be(cloneRoot);
            applicationArguments.ConfigurationFile.Should().Be(configurationFile);
        }

        public interface ICommandLineParserCallBackListener
        {
            void Callback(string obj);
        }
    }
}