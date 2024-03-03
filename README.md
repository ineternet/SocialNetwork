(Optional) Create a static class in namespace `SocialModel` and write to `SocialModel.DataSeed` in a static constructor to provide seed data
```cs
namespace SocialModel;

internal static partial class DataSeed
{
  static DataSeed()
  {
	  DataSeed.Users = x => x.HasData(new User() { ... });
	  DataSeed.Posts = x => x.HasData(new Post() { ... });
  }
}
```
Create the initial migration - it will be applied on startup
```bash
dotnet ef migrations add CreateFromRelease
```
Create a folder and provide a PKCS12 certificate `cert.pfx` that will be used for the website.
If the certificate requires a password then this can be supplied in the `appsettings.json`.
```bash
mkdir ssl
cp /certs/website.pfx ./ssl/cert.pfx
```
Create an .env file and provide the neccessary variables:
```dotenv
SQL_SA_PASSWORD="Sa_user_Pwd_1234"
SSL_DIR="./ssl"
```
Run as docker compose, or without docker:
Set the `SQL_CONNSTR` environment variable to a SQL Server connection string and run the `SocialSite` project.
