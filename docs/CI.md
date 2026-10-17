# Continuous integration

One workflow, `.github/workflows/ci.yml`, on every push to `main` and every pull
request.

## What runs

| Job | Runs on | Does |
|---|---|---|
| Backend | always | restore, build, `dotnet test` |
| Frontend | always | `npm ci`, production build |
| Images | after both | builds the API and web images; pushes to GHCR only from `main` |
| Deploy frontend | push to `main` only | publishes to Cloudflare Workers |

Building both images on a pull request means a broken Dockerfile fails in review
rather than at deploy time. Nothing is published from a pull request: a fork has
no write access to the registry, and publishing unreviewed code would be wrong
regardless.

## How each end gets deployed

**The API deploys itself.** Render watches `main` and rebuilds on every merge,
including applying any new migration at startup. Nothing in this workflow
touches it.

**The frontend is published here**, in the last job, after the tests and both
image builds have passed. Before this existed it was a manual `wrangler deploy`,
which meant a merge could leave a new API live against an old bundle.

## Secrets

Set under Settings → Secrets and variables → Actions.

| Name | Where it comes from |
|---|---|
| `CLOUDFLARE_API_TOKEN` | Cloudflare → My Profile → API Tokens → Create Token → **Edit Cloudflare Workers** |
| `CLOUDFLARE_ACCOUNT_ID` | Cloudflare dashboard → Workers & Pages, shown in the right-hand column |

`GITHUB_TOKEN` is provided automatically and needs no setup.

The deploy job uses a `production` environment, so a required reviewer can be
added later under Settings → Environments if a merge should ever wait for a
person before going live.

## Things worth knowing

**A failed deploy does not roll back.** If the publish step fails, the previous
bundle keeps serving, which is the safe outcome, but the API may already have
moved on. Check the Actions log before assuming a merge went out.

**The frontend is built twice**, once to check it compiles and once to publish.
Passing the first build between jobs would need an artifact upload and download,
which costs more than the two minutes the rebuild takes.

**Version pinning.** The workflow asks for `dotnet-version: '10.0.x'` and Node
20. A new major on either side will not be picked up silently.
