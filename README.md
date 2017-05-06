# PayoneerDataGrid
A simple data grid with server side pagination, search, sorting, and caching.
Client side utilizes https://datatables.net/

Setting up the database:
1. In SQL Server Management Studio (SSMS), connect to the local database engine ".\SQLEXPRESS"
2. Add a new database called "Payoneer"
3. In Web.config the connection string should be: 
connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=Payoneer;Integrated Security=True;MultipleActiveResultSets=true"
4. Run populate_database.sql in SSMS to populate the db with 1000 rows

