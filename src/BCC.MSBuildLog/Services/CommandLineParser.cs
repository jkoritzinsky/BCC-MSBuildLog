﻿using System;
using System.Text.RegularExpressions;
using BCC.Core.Services;
using BCC.MSBuildLog.Interfaces;
using Fclp;

namespace BCC.MSBuildLog.Services
{
    public class CommandLineParser: ICommandLineParser
    {
        private readonly Action<string> _helpCallback;
        private readonly IEnvironmentService _environmentService;
        private readonly FluentCommandLineParser<ApplicationArguments> _parser;

        public CommandLineParser(Action<string> helpCallback, IEnvironmentService environmentService)
        {
            _helpCallback = helpCallback;
            _environmentService = environmentService;
            _parser = new FluentCommandLineParser<ApplicationArguments>();

            _parser.Setup(arg => arg.InputFile)
                .As("input")
                .WithDescription("Input file")
                .Required();

            _parser.Setup(arg => arg.OutputFile)
                .As("output")
                .WithDescription("Output file")
                .Required();

            _parser.Setup(arg => arg.ConfigurationFile)
                .As("configuration")
                .WithDescription("Configuration file");

            _parser.Setup(arg => arg.CloneRoot)
                .As("cloneRoot")
                .WithDescription("Clone root");

            _parser.Setup(arg => arg.OwnerRepo)
                .As("ownerRepo")
                .WithDescription("Repository owner/name");

            _parser.Setup(arg => arg.Owner)
                .As("owner")
                .WithDescription("Repository owner");

            _parser.Setup(arg => arg.Repo)
                .As("repo")
                .WithDescription("Repository name");

            _parser.Setup(arg => arg.Hash)
                .As("hash")
                .WithDescription("Hash");

            _parser.SetupHelp("?", "help")
                .WithHeader(typeof(CommandLineParser).Assembly.FullName)
                .Callback(helpCallback);
        }

        public ApplicationArguments Parse(string[] args)
        {
            var commandLineParserResult = _parser.Parse(args);
            if (commandLineParserResult.HelpCalled)
            {
                return null;
            }

            if (commandLineParserResult.EmptyArgs || commandLineParserResult.HasErrors)
            {
                _parser.HelpOption.ShowHelp(_parser.Options);
                return null;
            }

            ApplicationArguments OutputError()
            {
                new CommandLineParser(_helpCallback, _environmentService).Parse(new string[0]);
                return null;
            }

            var applicationArguments = _parser.Object;
            if (!string.IsNullOrWhiteSpace(applicationArguments.OwnerRepo))
            {
                var regex = new Regex("^(?<owner>.*?)/(?<repo>.*?)$");
                if (regex.IsMatch(applicationArguments.OwnerRepo))
                {
                    var match = regex.Match(applicationArguments.OwnerRepo);
                    applicationArguments.Owner = match.Groups["owner"].Value;
                    applicationArguments.Repo = match.Groups["repo"].Value;
                }
                else
                {
                    return OutputError();
                }
            }

            applicationArguments.Owner = applicationArguments.Owner ?? _environmentService?.GitHubOwner;
            applicationArguments.Repo = applicationArguments.Repo ?? _environmentService?.GitHubRepo;
            applicationArguments.Hash = applicationArguments.Hash ?? _environmentService?.CommitHash;
            applicationArguments.CloneRoot = applicationArguments.CloneRoot ?? _environmentService?.BuildFolder;

            if (string.IsNullOrWhiteSpace(applicationArguments.Owner) ||
                string.IsNullOrWhiteSpace(applicationArguments.Repo) ||
                string.IsNullOrWhiteSpace(applicationArguments.Hash) ||
                string.IsNullOrWhiteSpace(applicationArguments.CloneRoot))
            {
                return OutputError();
            }

            return applicationArguments;
        }
    }
}