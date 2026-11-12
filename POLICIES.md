# Repository Policies

This document provides an overview of all policies applied to the microsoft/PowerApps-Samples repository.

## Table of Contents

1. [License Policy](#license-policy)
2. [Security Policy](#security-policy)
3. [Code of Conduct](#code-of-conduct)
4. [Contributor License Agreement (CLA)](#contributor-license-agreement-cla)
5. [Pull Request Copilot Policy](#pull-request-copilot-policy)

---

## License Policy

**File**: [LICENSE](LICENSE)

This repository is licensed under the **MIT License**.

### Key Points:
- Copyright belongs to Microsoft Corporation
- Free to use, modify, distribute, sublicense, and sell
- Provided "AS IS" without warranty
- Copyright and permission notice must be included in all copies

**Full License**: See [LICENSE](LICENSE) file for complete terms.

---

## Security Policy

**File**: [SECURITY.md](SECURITY.md)

Microsoft takes the security of software products and services seriously.

### Reporting Security Vulnerabilities

**DO NOT** report security vulnerabilities through public GitHub issues.

#### Preferred Method:
Report to the Microsoft Security Response Center (MSRC):
- Online: [https://msrc.microsoft.com/create-report](https://aka.ms/opensource/security/create-report)
- Email: [secure@microsoft.com](mailto:secure@microsoft.com)
- Encrypted: Use [Microsoft Security Response Center PGP Key](https://aka.ms/opensource/security/pgpkey)

#### Response Time:
- Expected response within 24 hours
- Follow up via email if no response received

#### Information to Include:
- Type of issue (e.g., buffer overflow, SQL injection, XSS)
- Full paths of affected source files
- Location of affected source code (tag/branch/commit/URL)
- Special configuration required to reproduce
- Step-by-step reproduction instructions
- Proof-of-concept or exploit code (if possible)
- Impact assessment

#### Bug Bounty:
Complete reports may contribute to higher bounty awards through the [Microsoft Bug Bounty Program](https://aka.ms/opensource/security/bounty).

#### Disclosure Policy:
Microsoft follows the principle of [Coordinated Vulnerability Disclosure](https://aka.ms/opensource/security/cvd).

**Full Policy**: See [SECURITY.md](SECURITY.md) for complete details.

---

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).

### Resources:
- **FAQ**: [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
- **Contact**: [opencode@microsoft.com](mailto:opencode@microsoft.com) for questions or comments

### Key Principles:
- Be respectful and inclusive
- Welcome diverse perspectives
- Focus on what is best for the community
- Show empathy towards other community members

**Full Policy**: Visit [https://opensource.microsoft.com/codeofconduct/](https://opensource.microsoft.com/codeofconduct/)

---

## Contributor License Agreement (CLA)

All contributions require agreement to a Contributor License Agreement (CLA).

### How It Works:
1. Submit a pull request
2. CLA-bot automatically checks your CLA status
3. If needed, bot provides instructions to complete CLA
4. Follow bot instructions (required only once across all Microsoft repos)
5. Bot decorates PR with appropriate label/comment

### Purpose:
- Declares you have the right to grant Microsoft the rights to use your contribution
- Protects both contributors and Microsoft

**Learn More**: [Contributor License Agreements](https://cla.microsoft.com)

---

## Pull Request Copilot Policy

**File**: [.azuredevops/policies/pullrequestcopilot.yaml](.azuredevops/policies/pullrequestcopilot.yaml)

Automated AI-powered pull request review and summary generation.

### Configuration:

#### General Settings:
- **Status**: Enabled
- **Max Review Comments**: 3 comments per PR (range: 0-10)

#### Auto-Review on PR Creation:
- **Trigger**: `copilot: review` prompt
- **Auto-Resolve**: Comments marked as open (not auto-resolved)
- **Excluded File Types**:
  - Configuration files: `*.ini`, `*.yml`, `*.json`, `*.yaml`, `*.bicep`, `*.xml`
  - Project files: `*.csproj`, `*.sln`, `*.resx`, `*.proj`
  - Binary/Media files: `*.png`, `*.vsdx`, `*.pbix`, `*.pdf`, `*.pfx`, `*.bin`, `*.jpeg`

#### Auto-Summary Generation:
- **Trigger**: `copilot: summary` prompt
- **Action**: Updates PR description with AI-generated summary
- **Auto-Resolve**: Summary comments marked as closed (auto-resolved)

### Optional Features (Currently Disabled):
- Branch filtering
- Additional review instructions
- Severity threshold adjustment
- Focus area customization (performance, security, reliability, etc.)
- User-specific filtering
- Work item generation

**Full Configuration**: See [.azuredevops/policies/pullrequestcopilot.yaml](.azuredevops/policies/pullrequestcopilot.yaml)

---

## Additional Resources

- **Main Repository**: [microsoft/PowerApps-Samples](https://github.com/microsoft/PowerApps-Samples)
- **Power Apps Documentation**: [learn.microsoft.com/power-apps](https://learn.microsoft.com/power-apps/)
- **Contributing Guidelines**: See [README.md](README.md)

---

## Questions or Concerns?

For questions about these policies:
- **Security**: [secure@microsoft.com](mailto:secure@microsoft.com)
- **Code of Conduct**: [opencode@microsoft.com](mailto:opencode@microsoft.com)
- **General**: Open an issue in this repository

---

*Last Updated: 2025-11-18*
