### InfoEducație Portal

## ASP NET Core Solution

### Projects Structure
* InfoEducatie.Contest
* InfoEducatie.Main

### Local Setup
 * `git clone`
 * `git submodule update --init --recursive`
 * DB: 
   * PostgreSQL 13+: example docker-compose file [here](./db-docker-compose.yml)
   * Migrations:
     * install dotnet ef tool: `dotnet tool install --global dotnet-ef`
     * `dotnet ef migrations add <name>` & `dotnet ef database update` from `InfoEducatie.Main` project directory
 * run with file watch: `./run.sh` (Linux) or `./run.bat` (Windows) from solution directory
 * Identity scaffold
    * [docs](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-3.1&tabs=netcore-cli#scaffold-identity-into-a-razor-project-with-authorization)
    * `dotnet aspnet-codegenerator identity -dc ApplicationDbContext --files "Account.Register;Account.Login"
`

### Environment variables 
  * InfoEducatie
    * `RESULTS_PATH`

  copy `.env` file to `.local.env` and adjust your settings \
  Docs: `./MCMS/README.md` (MCMS submodule directory)

