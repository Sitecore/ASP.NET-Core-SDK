# Contribution to the Sitecore ASP.NET Core SDK

Thank you for your interest in contributing to our project. You can contribute with issues and PRs. Simply filing issues for problems you encounter is a great way to contribute. Contributing implementations is greatly appreciated.

## Code of Conduct
Please read the [Code of Conduct](./CODE_OF_CONDUCT.md) before participating, all contributors are expected to uphold this code. Please report unacceptable behavior to [community@sitecore.com](mailto:community@sitecore.com).

## Reporting Issues

We always welcome bug reports, feature requests and overall feedback. Here are a few tips on how you can make reporting your issue as effective as possible.

### Finding Existing Issues

Before filing a new issue, please search our [open issues](https://github.com/Sitecore/ASP.NET-Core-SDK/issues) to check if it already exists.

If you do find an existing issue, please include your own feedback in the discussion. Do consider upvoting (reaction) the original post, as this helps us prioritize popular issues.

### Use the right template

When creating a new issue, please use the appropriate template. We have the following templates available:

| Template                        | Description                                                                                                                                 |
| ------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| Bug Report                      | Use this template to report a bug.                                                                                                          |
| Feature Request                 | Use this template to request a new feature.                                                                                                 |
| Question                        | Use this template to ask a question. For implementation specific questions please use [StackExchange](https://sitecore.stackexchange.com/). |
| Report a security vulnerability | Use this template to report a security vulnerability.                                                                                       |

### Writing a Good Bug Report

Good bug reports make it easier for maintainers to verify and root cause the underlying problem. The better a bug report, the faster the problem will be resolved. Ideally, a bug report should contain the following information:

* A high-level description of the problem.
* A _minimal reproduction_, i.e. the smallest size of code/configuration required to reproduce the wrong behavior.
* A description of the _expected behavior_, contrasted with the _actual behavior_ observed.
* Information on the environment: Sitecore XM version, SDK version, etc.
* Additional information, e.g. is it a regression from previous versions? are there any known workarounds?

#### Why are Minimal Reproductions Important?

A reproduction lets maintainers verify the presence of a bug, and diagnose the issue using a debugger. A _minimal_ reproduction is the smallest possible application demonstrating that bug. Minimal reproductions are generally preferable since they:

1. Focus debugging efforts on a simple code snippet,
2. Ensure that the problem is not caused by unrelated dependencies/configuration,
3. Avoid the need to share production codebases.

#### Are Minimal Reproductions Required?

In certain cases, creating a minimal reproduction might not be practical (e.g. due to nondeterministic factors, external dependencies). In such cases you would be asked to provide as much information as possible, for example by sharing a memory dump of the failing application. If maintainers are unable to root cause the problem, they might still close the issue as not actionable. While not required, minimal reproductions are strongly encouraged and will significantly improve the chances of your issue being prioritized and fixed by the maintainers.

#### How to Create a Minimal Reproduction

The best way to create a minimal reproduction is gradually removing code and dependencies from a reproducing app, until the problem no longer occurs. A good minimal reproduction:

* Excludes all unnecessary types, methods, code blocks, source files, nuget dependencies and project configurations.
* Contains documentation or code comments illustrating expected vs actual behavior.
* If possible, avoids performing any unneeded IO or system calls.

## Contributiting Code
All code contributions must be submitted by the standard GitHub pull request flow, or by logging an issue.

### Accepting Contributions
Sitecore has a _"No commitment"_ approach to this repository. The required functionality of the SDK and related XM Cloud features will be the driver for acceptance decisions. We are open to contributions from the community, but make no commitment to accept or incorporate changes.

### Guidelines
* Create a [fork of the repository](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo)
* Create a branch for your changes. Use /feat/ for new features, /fix/ for bug fixes and /new/ for breaking changes.
* We observe strict code style rules, please ensure your changes follow the guidance provided by the analyzers.
  * [StyleCop](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
  * [File Scoped Namespaces](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/file-scoped-namespaces)
  * Do not use `var`
* We require all changes to be covered by tests.
* When you are ready to submit your changes, [create a pull request from your fork](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/creating-a-pull-request-from-a-fork) into this repository.
* This repository is configured to use `Squash Merges` for all accepted contributions. This decision has been made to keep the commit history clean and concise.