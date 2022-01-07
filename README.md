# RadarrPusherApi
RadarrPusherApi allows you to communicate to a Web API (*pending*) via a Windows Worker Service to [Radarr](https://radarr.video/) that is installed on your computer. This is done without opening any ports on a computer but by using a message service from [Pusher](https://pusher.com/). The messaging service that [Pusher](https://pusher.com/) offers us is a "Pub/Sub" service, which is an asynchronous messaging service which is used by the computer and Web API (*pending*).

# RadarrPusherApi Worker Service Windows
This is the Windows Worker Service that you will install onto the computer so that the computer and the Web API (*pending*) can communicate. It is basically the same as a Windows Service, but more modern and uses [.NET](https://en.wikipedia.org/wiki/.NET) and not [.NET Framework](https://en.wikipedia.org/wiki/.NET). See this Stackoverflow [answer](https://stackoverflow.com/questions/59636097/c-sharp-worker-service-vs-windows-service#:~:text=Both%20are%20real%20services.,and%20stops%20with%20the%20application.) for more information.

## Status

[![GitHub issues](https://img.shields.io/github/issues/RadarrPusherApi/RadarrPusherApi.svg?maxAge=60&style=flat-square)](https://github.com/RadarrPusherApi/RadarrPusherApi/issues)
[![GitHub pull requests](https://img.shields.io/github/issues-pr/RadarrPusherApi/RadarrPusherApi.svg?maxAge=60&style=flat-square)](https://github.com/RadarrPusherApi/RadarrPusherApi/pulls)
[![GNU GPL v3](https://img.shields.io/badge/license-GNU%20GPL%20v3-blue.svg?maxAge=60&style=flat-square)](http://www.gnu.org/licenses/gpl.html)
[![Copyright 2022](https://img.shields.io/badge/copyright-2022-blue.svg?maxAge=60&style=flat-square)](https://github.com/RadarrPusherApi/Controlarr)

## Configuring the Development Environment

### Requirements

* [Visual Studio Community 2022](https://visualstudio.microsoft.com/vs/community/)
* [Git](https://git-scm.com/downloads)

### Setup

* Make sure all the required software mentioned above are installed
* Clone the repository into your development machine ([*info*](https://help.github.com/desktop/guides/contributing/working-with-your-remote-repository-on-github-or-github-enterprise))

### Development

* Open `RadarrPusherApi.sln` in Visual Studio 2019
* Make sure `RadarrPusherApi.WorkerService.Windows` is set as the startup project
* Press `F5`