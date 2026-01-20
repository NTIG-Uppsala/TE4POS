## How to setup the server

These instructions are written for Debian 12

1. Install dotnet 9.0 for [debian](https://learn.microsoft.com/sv-se/dotnet/core/install/linux-debian?tabs=dotnet10) or [other](https://learn.microsoft.com/sv-se/dotnet/core/install/) 
2. Install git using `apt install git`.
3. Clone the repository `git clone https://github.com/NTIG-Uppsala/TE4POS-Backend`
4. Run `cp ~/TE4POS-Backend/StockAPI.service.example /etc/systemd/system/StockAPI.service`
5. Change directory to the project directory. `cd ~/TE4POS-Backend`. 
6. Build the project. `dotnet publish -c Release -o /srv/StockAPI/`.  
It will succeed (but might be some warnings). 
7. Update the services and start the API service.  
```
systemctl daemon-reload
systemctl start StockAPI.service
```

## How to update the API on the server

1. Change directory to the project directory. `cd ~/TE4POS-Backend`.
2. Pull the new version using `git pull`
3. Rebuild the project and restart the service.  
```
dotnet publish -c Release -o /srv/StockAPI/
systemctl restart StockAPI.service
```
It will succeed but with a warning. 

---
[Go back to README](../README.md)