﻿using System.Threading.Tasks;
using MSBLOC.Core.IntegrationTests.Utilities;

namespace MSBLOC.Core.IntegrationTests.Services
{
    public class GitHubGraphQLClientIntegrationTests : IntegrationTestsBase
    {
        [IntegrationTest]
        public async Task ShouldConstruct()
        {
            var gitHubGraphQLClient = CreateGitHubGraphQLTokenClient();
        }
    }
}