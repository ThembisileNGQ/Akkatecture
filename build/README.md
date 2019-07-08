# Azure Pipelines Build Definitions

Akkatecture is built in azure pipelines. A description of each of the build pipeline definitions can be found below.

**azure-pipelines-development-ci-cd.yaml** - Runs the pipeline for development builds that go into an internal feed. Packages are alpha.

**azure-pipelines-master-ci-cd.yaml** - Runs the pipeline for development builds that go into an internal feed. Packages are prerelease.

**azure-pipelines-release-ci-cd.yaml** - Runs the pipeline for production builds that go to NuGet. Packages are public and 'production ready'.

**azure-pipelines-static-ci** - Runs a smaller build pipeline used for gating pull requests into both master and dev. no artefacts come from this build.

# FAKE Build Scripts

Right now it is still under development.

The point of these build scripts is to enable cross plat testing scenarios. Run the following to get the Fake builder to work:

`fake build`
`fake run build.fsx --target Build` 

This assumes that you have fake installed, and that you have followeded [these](http://fake.build/fake-commandline.html) steps. If not then run the shell or command scripts