** Use the below command to create a new migration **
** Ensure you've setted 'default project' as 'MoneyPlan.DAO' on Package Manager Console

> EntityFrameworkCore\Add-Migration -StartupProject MoneyPlan.API -Context "SavingsContext" NewMigration

====================================================

** Finally apply the migrations

> EntityFrameworkCore\Update-Database -Context "SavingsContext" -StartupProject "MoneyPlan.API" -Project "MoneyPlan.DAO"

