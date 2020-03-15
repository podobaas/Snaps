<p align="center">
<img src="/snaps.png" width="150" hight="150"/>
</p>
<h1 align="center">Snaps</h1>
<h4 align="center">A simple multi-threaded command line tool for creating scheduled snapshots for DigitalOcean</h4>

<p align="center">
<img src="https://api.codacy.com/project/badge/Grade/e4b53e249ee04916b28227000340540f"/><img src="https://img.shields.io/github/license/podobaas/Snaps"/>
</p>

<h3 align="center"><a href="https://github.com/podobaas/Snaps/releases/tag/1.0">Download binaries</a></h3>
  
## About
+ [Install dotnet runtime](https://docs.microsoft.com/ru-ru/dotnet/core/install/linux-package-manager-ubuntu-1804)
+ [Build project](README.md#build-project)
+ [Create alias](README.md#create-alias)
+ [Usage](README.md#usage)
    + [Set API token](README.md#set-api-token)
    + [Set max parallel tasks](README.md#set-max-parallel-tasks)
    + [Create snapshot for droplets](README.md#create-snapshot-for-droplets)
    + [Create snapshot for volumes](README.md#create-snapshot-for-volumes)
    + [Create cron job](README.md#create-cron-job)
    + [Crete cron job from a file](README.md#create-cron-job-from-a-file)


## Build project
```bash
cd <path to Snaps.sln>
dotnet build -c Release
```

## Create alias
```bash
nano ~/.bashrc
alias snaps="dotnet /xxx/xxx/Snaps.dll>"
```

## Usage

### Set API token
```bash
snaps settings -t <Your DigitanlOcean API token>
```

### Set max parallel tasks
```bash
snaps settings -mc <numebr> #Max degree of concurrency for create snapshot (default 5)
```

### Create snapshot for droplets
```bash
snaps list -d # Get your list droplets
snaps snapshot <droplet ids> -d # Create a snapshot for droplets
```
For example:
```bash
snaps list -d
snaps snapshot 1 2 3 4 -d
```

### Create snapshot for volumes
```bash
snaps list -v # Get your list volumes
snaps snapshot <volume ids> -v # Create a snapshot for volumes
```
For example:
```bash
snaps list -v
snaps snapshot 1 2 3 4 -v
```

##### Create cron job
```bash
crontab -e
* * * * * snaps snapshot <droplet ids> -d # Create a snapshot for droplets
* * * * * snaps snapshot <volume ids> -v # Create a snapshot for volumes
```

##### Create cron job from a file
```bash
snaps list -d -o /home/dproplets.csv # Export droplet ids to a file
snaps list -v -o /home/volumes.csv # Export volume ids to a file
crontab -e
* * * * * snaps snapshot -d -f /home/dproplets.csv # Import droplet ids from a file
* * * * * snaps snapshot -v -f /home/dproplets.csv # Import volume ids from a file
```

## Built With
+ [.NET Core](https://github.com/dotnet/core)
+ [DigitalOcean API .NET](https://github.com/trmcnvn/DigitalOcean.API)
+ [ConsoleTables](https://github.com/khalidabuhakmeh/ConsoleTables)

## References
+ [LICENSE](LICENSE)
