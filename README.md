# Vintage-Monster-Movies
A C# ASP.NET Core Razor Pages application that catalogs classic monster horror films (e.g., Dracula, The Wolf Man, Creature from the Black Lagoon).
The app supports two roles—User and Admin—with authentication, movie browsing, detailed views, and full CRUD for administrators. Data is stored in SQLite.


# Public / User

Registration & Login (forms-based authentication).
Movies list in a table:

Columns: Title, Release Year, Writer, Director, Short Description.


Movie details page by clicking a movie title:

Displays extended details (from MovieDetails), such as Stars and Runtime.



# Admin
Admin dashboard listing all movies in a table with:

Title, Release Year, Writer, Director, Short Description
Edit and Delete actions per row


Add new movie (create form)
Add / edit details for a movie (MovieDetails table)
Secured via Admin role

# Data
SQLite database for persistence.

