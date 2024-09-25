# Teamup

An app to handle huge friendly tournaments.
It features:
- Multiple games within a tournament
- Each game has its own fixed number of teams, and as many rewards
- Each players of a tournament has a score
- This score is based on rewards their team has won on each games, some bonuses and maluses their team could have gotten, and some bonnuses and maluses they themselves could have gotten.
- For each game, the teams are automatically generated, based on current players's scores, trying to even out the results.

## Migrate

`dotnet ef migrations add <MigrationName>`
`dotnet ef database update`

## Deploy

A simple sqlite database is used to store all datas of the application, including user data.
Since the project is being used for me and my friends, with no concurrent writes when a tournament is playing, it is fine.
However, note that on each deploy, the database will be rebuild from scratch, loosing all previous datas.

`fly deploy`