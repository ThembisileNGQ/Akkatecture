# Azure Pipelines Build Definitions

Akkatecture is built in azure pipelines. A description of each of the build pipeline definitions can be found below.

**azure-pipelines-development-ci-cd.yaml** - Runs the pipeline for development builds that go into an internal feed. Packages are alpha.

**azure-pipelines-master-ci-cd.yaml** - Runs the pipeline for development builds that go into an internal feed. Packages are prerelease.

**azure-pipelines-release-ci-cd.yaml** - Runs the pipeline for production builds that go to NuGet. Packages are public and 'production ready'.

**azure-pipelines-static-ci** - Runs a smaller build pipeline used for gating pull requests into both master and dev. no artefacts come from this build.