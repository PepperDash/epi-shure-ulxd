# Internal Essentials Plugin Template (c) 2020

## License

Provided under MIT license

## Overview

Use this repo as a template when creating a new plugin for Essentials. For more information about plugins, refer to the Essentials Wiki [Plugins](https://github.com/PepperDash/Essentials/wiki/Plugins) article.

## Github Actions

This repo contains two Github Action workflows that will build this project automatically. Modify the SOLUTION_PATH and SOLUTION_FILE environment variables as needed. Any branches named `feature/*`, `release/*`, `hotfix/*` or `development` will automatically be built with the action and create a release in the repository with a version number based on the latest release on the master branch. If there are no releases yet, the version number will be 0.0.1. The version number will be modified based on what branch triggered the build:

- `feature` branch builds will be tagged with an `alpha` descriptor, with the Action run appended: `0.0.1-alpha-1`
- `development` branch builds will be tagged with a `beta` descriptor, with the Action run appended: `0.0.1-beta-2`
- `release` branches will be tagged with an `rc` descriptor, with the Action run appended: `0.0.1-rc-3`
- `hotfix` branch builds will be tagged with a `hotfix` descriptor, with the Action run appended: `0.0.1-hotfix-4`

Builds on the Master branch will ONLY be triggered by manually creating a release using the web interface in the repository. They will be versioned with the tag that is created when the release is created. The tags MUST take the form `major.minor.revision` to be compatible with the build process. A tag like `v0.1.0-alpha` is NOT compatabile and may result in the build process failing.

If you have any questions about the action, contact Andrew Welker or Neil Dorin.
<!-- START Minimum Essentials Framework Versions -->
### Minimum Essentials Framework Versions

- 1.12.0
<!-- END Minimum Essentials Framework Versions -->
<!-- START Supported Types -->
### Supported Types

- shure-ulx-d
- ulxd
- shureulxd
<!-- END Supported Types -->
<!-- START Join Maps -->

<!-- END Join Maps -->
<!-- START Interfaces Implemented -->
### Interfaces Implemented

- IDeviceInfoProvider
<!-- END Interfaces Implemented -->
<!-- START Base Classes -->
### Base Classes

- EssentialsBridgeableDevice
- JoinMapBaseAdvanced
<!-- END Base Classes -->
<!-- START Public Methods -->
### Public Methods

- public void UpdateDeviceInfo()
- public void UpdateDeviceInfo()
<!-- END Public Methods -->
<!-- START Bool Feedbacks -->
### Bool Feedbacks

- ReceiverOnlineFeedback
- ChargerOnlineFeedback
- ChargerOnlineFeedback2
- Charger2IsPresentFeedback
- ReceiverOnlineFeedback
- ChargerOnlineFeedback
- ChargerOnlineFeedback2
- Charger2IsPresentFeedback
<!-- END Bool Feedbacks -->
<!-- START Int Feedbacks -->
### Int Feedbacks

- NumberOfChargersFeedback
- NumberOfChargersFeedback
<!-- END Int Feedbacks -->
<!-- START String Feedbacks -->
### String Feedbacks

- ErrorFeedback
- ErrorFeedback
<!-- END String Feedbacks -->
