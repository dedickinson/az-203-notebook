# Google Web Auth Demo

Auth details are stored in an SQLite database (`app.db`) via the
following packages:

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Sqlite`

The Google auth client details are stored locally using `dotnet user-secrets`:

- `Authentication:Google:ClientId`
- `Authentication:Google:ClientSecret`
