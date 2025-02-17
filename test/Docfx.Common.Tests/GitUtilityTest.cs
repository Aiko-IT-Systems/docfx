﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Docfx.Common.Git;
using Xunit;

namespace Docfx.Common.Tests;

[Collection("docfx STA")]
public class GitUtilityTest : IDisposable
{
    private string _originalBranchName;
    private const string envName = "DOCFX_SOURCE_BRANCH_NAME";
    public GitUtilityTest()
    {
        _originalBranchName = Environment.GetEnvironmentVariable(envName);
        Environment.SetEnvironmentVariable(envName, "special-branch");
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(envName, _originalBranchName);
    }

    [Fact]
    public void Environment_ForBranchName()
    {
        var info = GitUtility.TryGetFileDetail(Directory.GetCurrentDirectory());
        Assert.Equal("special-branch", info.RemoteBranch);
    }

    [Fact]
    public void Environment_ForGitTimeout()
    {
        Environment.SetEnvironmentVariable(GitUtility.GitTimeoutEnvVarName, "3000");
        Assert.Equal(3000, GitUtility.GetGitTimeout());

        Environment.SetEnvironmentVariable(GitUtility.GitTimeoutEnvVarName, "0");
        Assert.Equal(10_000, GitUtility.GetGitTimeout());

        Environment.SetEnvironmentVariable(GitUtility.GitTimeoutEnvVarName, "");
        Assert.Equal(10_000, GitUtility.GetGitTimeout());
    }

    [Obsolete]
    [Fact]
    public void TestParseGitRepoInfo()
    {
        var repoInfo = GitUtility.Parse("git@github.com:dotnet/docfx");
        Assert.Equal("dotnet", repoInfo.RepoAccount);
        Assert.Equal("docfx", repoInfo.RepoName);
        Assert.Equal(RepoType.GitHub, repoInfo.RepoType);

        repoInfo = GitUtility.Parse("https://github.com/dotnet/docfx");
        Assert.Equal("dotnet", repoInfo.RepoAccount);
        Assert.Equal("docfx", repoInfo.RepoName);
        Assert.Equal(RepoType.GitHub, repoInfo.RepoType);

        repoInfo = GitUtility.Parse("ssh://mseng@vs-ssh.visualstudio.com:22/FakeProject/_ssh/Docfx");
        Assert.Equal("mseng", repoInfo.RepoAccount);
        Assert.Equal("Docfx", repoInfo.RepoName);
        Assert.Equal("FakeProject", repoInfo.RepoProject);
        Assert.Equal(RepoType.Vso, repoInfo.RepoType);

        repoInfo = GitUtility.Parse("https://mseng.visualstudio.com/FakeProject/_git/Docfx");
        Assert.Equal("mseng", repoInfo.RepoAccount);
        Assert.Equal("Docfx", repoInfo.RepoName);
        Assert.Equal("FakeProject", repoInfo.RepoProject);
        Assert.Equal(RepoType.Vso, repoInfo.RepoType);
    }

    [Obsolete]
    [Fact]
    public void TestCombineGitUrl()
    {
        var repoInfo = GitUtility.Parse("git@github.com:dotnet/docfx");
        var url = GitUtility.CombineUrl(repoInfo.NormalizedRepoUrl.AbsoluteUri, "dev", "src/docfx/Program.cs", RepoType.GitHub);
        Assert.Equal("https://github.com/dotnet/docfx/blob/dev/src/docfx/Program.cs", url.AbsoluteUri);

        repoInfo = GitUtility.Parse("https://mseng.visualstudio.com/FakeProject/_git/Docfx");
        url = GitUtility.CombineUrl(repoInfo.NormalizedRepoUrl.AbsoluteUri, "dev", "src/docfx/Program.cs", RepoType.Vso);
        Assert.Equal("https://mseng.visualstudio.com/DefaultCollection/FakeProject/_git/Docfx?path=%2fsrc%2fdocfx%2fProgram.cs&version=GBdev&_a=contents", url.AbsoluteUri);
    }
}
