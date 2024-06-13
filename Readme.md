# Agenda Manager with RavenDB and ASP.NET Core

This is a demo application for an Agenda manager (yes, a todo app) that uses ASP.NET Core and RavenDB. 

To get started you'll need to run a RavenDB instance using Docker with the following command.

```bash
docker run -p 8080:8080 ravendb/ravendb:ubuntu-latest
```

You'll need to initialize the database, and create a new database called **"Productivity"**.

From there you can run this project.

## Advantages to RavenDB

- The document model is a great development experience
- Fine tuning queries with indexes

## RavenDB considerations

- Remembering to create static indexes
- Those string Ids can be a challenge to work around (See `EncryptedParameter`)
- Stale results (not an issue in this sample)

## Notes

- Created using [JetBrains Rider](https://jetbrains.com/rider)
- Used JetBrains AI Assistant for some Bootstrap and JQuery stuff
- RavenDB instance is a development instance so there's no security
- Application assumes one user but could be made to support multiple users with the addition of identity.

🍻 Cheers!