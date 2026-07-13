# Hack-Slash

Unity WebGL game, live at [hs.brycebohling.com](https://hs.brycebohling.com).

## Publishing a new build

The site deploys automatically from this repo — a workflow
(`.github/workflows/deploy.yml`) uploads the `dist/` folder to Cloudflare
Pages on every push to `main`.

1. **Build in Unity** — File → Build Settings → **WebGL** → Build, and
   choose the `dist/` folder in this repo (replace its contents).
2. **Commit and push**:
   ```
   git add dist
   git commit -m "new build"
   git push
   ```
3. **Check it worked** — the Actions tab shows a green run, then the game is
   live at https://hs.brycebohling.com (hard-refresh: Cmd+Shift+R).

⚠️ Keep every file in `dist/` under 25 MB — Cloudflare Pages rejects bigger
files. Current largest is ~11 MB (`dist.wasm.unityweb`). If a build grows
past the limit, ask Dad.

> Infra background: the Pages project (`hs`), its custom domain, and DNS are
> managed in the `bbohling/cf-infra` repo. This repo's deploy token
> (`ci-hs`) only has Pages/Workers permissions.
