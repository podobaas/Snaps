<h4 align="center">A simple multi-threaded command line tool for creating scheduled snapshots for DigitalOcean</h4>

<h3 align="center"><a href="">Download binaries</a></h3>
  

## Build

##### Install dotnet runtime
```bash
wget -q https://packages.microsoft.com/config/ubuntu/19.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-runtime-3.1
```

##### Build project
```bash
cd <path to Snaps.sln>
dotnet build -c Release
```

##### Create alias
```bash
nano ~/.bashrc
alias snaps="dotnet /xxx/xxx/Snaps.dll>"
```

## Usage

##### Download binaries and create alias
```bash
nano ~/.bashrc
alias snaps="dotnet /xxx/xxx/Snpas.dll"
```

##### Set API token
```bash
snaps settings -t <Your DigitanlOcean API token>
```

##### Get your list droplets or volumes
```bash
snaps list -d # Get your list droplets
snaps list -v # Get your list volumes
```

##### Create snaphots
```bash
snaps snapshot <droplet ids> -d # Create a snapshot for droplets
snaps snapshot <volume ids> -v # Create a snapshot for volumes
```
For example:
```bash
snaps snapshot 1 2 3 4 -d
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
snaps list -d -o /home/dproplets.csv # Export a droplet ids to file
snaps list -v -o /home/volumes.csv # Export a volume ids to file
crontab -e
* * * * * snaps snapshot -d -f /home/dproplets.csv # Import a droplet ids from file
* * * * * snaps snapshot -v -f /home/dproplets.csv # Import a volume ids from file
```

## Built With
+ [.NET Core](https://github.com/dotnet/core)
+ [DigitalOcean API .NET](https://github.com/trmcnvn/DigitalOcean.API)
+ [ConsoleTables](https://github.com/khalidabuhakmeh/ConsoleTables)

## References
+ [LICENSE](LICENSE)
