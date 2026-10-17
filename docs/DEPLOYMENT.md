# Deployment

The API runs on Render as a Docker service. The database is Neon, in Singapore.
The frontend is deployed separately.

Nothing secret is in the repository. Every value below is set in the hosting
dashboard, and `.env` is ignored by git.

## Running the whole stack locally

```
cp .env.example .env
```

Fill in `POSTGRES_PASSWORD` and `JWT_KEY`, then:

```
docker compose up --build
```

The API comes up on `http://localhost:8080` and applies its own migrations.
`docker compose down -v` wipes the database.

For day-to-day work, with the API in Visual Studio, use `docker-compose.dev.yml`
instead and leave the API out of Docker. See `DATABASE.md`.

## Environment variables

Double underscores map to configuration sections, so `Jwt__Key` is `Jwt:Key`.

| Variable | Notes |
|---|---|
| `ConnectionStrings__DefaultConnection` | Neon string, with `Timezone=UTC` appended |
| `Jwt__Key` | At least 32 characters. `openssl rand -base64 48` |
| `Jwt__Issuer`, `Jwt__Audience` | `TalaanNiAna`, `TalaanNiAnaUsers` |
| `Cors__AllowedOrigins__0` | The frontend origin, exactly, no trailing slash |
| `Hosting__BehindReverseProxy` | `true` on Render |
| `Hosting__RunMigrationsOnStartup` | `true` on Render |

A missing connection string or signing key stops the app at startup with a
message naming the variable, rather than failing later at the first request.

## Health

- `GET /health` is liveness. Render polls this, and it touches nothing.
- `GET /health/ready` also checks the database answers. Useful right after a
  deploy; too expensive to poll.

## Things that bite

**Free tier sleeps.** A Render free service stops after idling, and the first
request afterwards waits for a cold start, which on this image is tens of
seconds. A demo link is worth warming before anyone clicks it.

**Timezone.** The Neon connection string needs `Timezone=UTC`. Without it the
daily sales chart groups into the wrong days and never errors.

**CORS and credentials.** The refresh token is a cookie, so the policy sends
credentials, and a wildcard origin is not allowed with that. The origin has to
be the exact deployed URL.

**Reverse proxy.** `Hosting__BehindReverseProxy` does two jobs. It stops the
HTTPS redirect, which would otherwise loop behind Render's TLS, and it makes
the rate limiter read the forwarded client address instead of treating every
caller as the proxy.

**Data protection keys.** ASP.NET writes them inside the container, so they are
lost on every redeploy and differ between instances. Nothing here depends on
them surviving: authentication is JWT, and refresh tokens are hashed in the
database. It would matter the moment anything uses the Identity token providers,
such as password reset.

**Migrations on boot.** Convenient on a free plan with no shell, and not what
a production system should do: two instances starting together both try to
migrate. Fine for one free instance, worth revisiting if this ever scales out.
