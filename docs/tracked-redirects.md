# Tracked redirects

## What changed, and why

A QR code normally contains a fixed destination. A stable link such as
`https://beneeckman.be/r/cv-en` instead lets PersonalSite record a visit and send
the visitor to a destination you can change later. **`cv-en` is the slug**: the
unique public name of that link. A visit is an interest signal, not an email open.

This change adds:

| Area | Change | Reason |
| --- | --- | --- |
| Database | `RedirectLinks`: ID, unique slug, target URL, optional campaign, active flag, creation time | Keep the destination editable without changing the printed URL; no extra database |
| Administrator API | `GET /redirect-links`, `PUT /redirect-links/{slug}` | List/configure links without building another management screen |
| Public API | `GET /r/{slug}` | Look up the saved destination, record `LinkClicked`, respond with temporary HTTP 302 |
| Analytics | `GET /analytics/redirect-links`, **Redirect visits** section in the existing Site Activity page | See visits grouped by slug, campaign and referrer origin, separately from contact-link clicks |
| CareerKit | Existing saved letter **QR target URL** accepts the public redirect URL | Reuse the existing QR renderer, snapshot and PDF path, with no template changes |

Relevant source files:

- `PersonalSite.Api/Domain/Redirects/RedirectLink.cs`
- `PersonalSite.Api/Domain/Redirects/RedirectSlug.cs`, `RedirectTargetUrl.cs`, `RedirectCampaign.cs`
- `PersonalSite.Api/Storage/Configurations/RedirectLinkConfiguration.cs` and `Storage/AppDbContext.cs`
- `PersonalSite.Api/Migrations/20261101223529_AddRedirectLinks{,.Designer}.cs` and model snapshot
- `PersonalSite.Api/Application/Redirects/RedirectModels.cs` and `RedirectLinkHandler.cs`
- `PersonalSite.Api/Endpoints/Redirects/RedirectEndpoints.cs`
- `PersonalSite.Api/Wiring/WebApplicationExtensions.cs` (route registration only)
- `Frontend/src/analytics/components/RedirectAnalyticsSection.tsx`, `analyticsApi.ts`, `types.ts`
- `Frontend/src/account/AnalyticsAdminPage.tsx` (add the section)
- `PersonalSite.Api.Tests/IntegrationTests/Application/Analytics/RedirectLinkTests.cs`
- `PersonalSite.Api.Tests/Domain/Redirects/RedirectValueObjectTests.cs`

`/docs` was ignored. `.gitignore` now makes a narrow exception for **this file
only**, so this explanation can be committed with the implementation. Other local
docs remain ignored. No files were committed or pushed by implementing this change.

## What does NOT change

- No public CV/knowledge upload, deletion or replacement; no assistant/provider call.
- No new authentication, dependency, app/server, background worker or blob storage.
- No automatic production link creation, seed links, deployment, or process restart.
- No CareerKit schema, application-link relationship, QR button, template, font,
  rendering pipeline or PDF setting change. Per-application slugs are an optional
  naming convention, not automatic application tracking.
- Existing PDFs remain immutable. An old PDF with a **direct** URL cannot be
  retrofitted. Only a PDF already containing `/r/your-slug` benefits from later
  destination changes. This does not add QR codes to CVs.
- No unique-visitor tracking, IP/user-agent capture, cookies, or visitor user ID.

## Why the redirect fields are value objects

Following owner review, `RedirectLink.Slug`, `TargetUrl` and optional `Campaign`
use `RedirectSlug`, `RedirectTargetUrl` and `RedirectCampaign` rather than strings
inside the domain. These are immutable sealed records, matching PersonalSite's
existing value-object conventions: validated construction, value equality, a
read-only `Value`, string conversion and `ToString()`.

Previously the handler checked the rules but the entity could be assigned any
string. Now those invariants are enforced by the values themselves, including for
direct handler calls that bypass endpoint validation. The handler delegates
request validation to their constructors; it does not keep a second copy of the
format rules. Invalid inputs still produce the same field-keyed HTTP 400 responses.

- `RedirectSlug` owns the exact lowercase ASCII format and 80-character limit;
  it does not trim or silently lowercase a different public link.
- `RedirectTargetUrl` owns the credential-free HTTP(S) and 2048-character rules.
  It preserves valid URL spelling/encoding/query/fragment exactly. The shared
  `Domain/Common/Url` is intentionally unchanged: its broader rules and URL
  normalization do not match this redirect contract.
- `RedirectCampaign` is a trimmed, nonblank label of at most 200 characters.
  `CreateOptional` represents an absent/blank campaign as null, not an empty value
  object. Raw overlength/control characters are still rejected before clearing or
  trimming, so this refactor does not weaken validation for blank inputs.

EF value converters persist/materialize these objects through the **same existing
varchar columns**, with the same lengths, nullable campaign and unique slug index.
The atomic upsert unwraps validated values only at its SQL boundary. List/save
responses and analytics metadata explicitly unwrap them to strings, so **no API
payload, UI, URL behavior, schema or existing-data rewrite changes**. No additional
migration is needed for this refactor; the original feature's `AddRedirectLinks`
migration is still needed for an initial deployment. EF's no-pending-model-changes
check verifies the relational model stays unchanged.

Existing valid saved rows round-trip normally. A developer-written invalid raw
database value now fails validated materialization instead of silently entering
the domain; no automatic data repair or new database cleanup mechanism is added.
Tests cover constructors/equality/bounds, optional-campaign semantics, direct-call
validation, typed queries and EF writes/reads, raw upsert reads, null round-trips,
and unchanged string response contracts.

## Configure and use a link

After the owner deploys both the API and frontend, use the existing authenticated
API tooling (for example Swagger) with a **genuine PersonalSite Administrator
session**. Never paste a token into documentation/chat or commit it. Tests use only
fictional localhost credentials. CareerKit does not call these management endpoints.

List current links: `GET /redirect-links`.

Create or replace a link: `PUT /redirect-links/cv-en`, JSON:

```json
{
  "targetUrl": "https://beneeckman.be/files/cv",
  "campaign": "english-cv",
  "isActive": true
}
```

This is an example, **not an automatically created link or approved production
write**. Choose your intended destination before sending a real request. The
endpoint returns HTTP 200 and the saved slug, target, campaign, status and creation
time. Saving an existing slug replaces its target/campaign/active flag, not its
creation time. Concurrent saves are last-write-wins; a unique index plus an atomic
database upsert prevents duplicate slugs. There is no delete or rename operation.

Then in CareerKit's existing letter editor:

1. Enable **Include QR code**.
2. Enter `https://beneeckman.be/r/cv-en` in **QR target URL**.
3. Save and export normally. The saved version freezes that stable URL.
4. Open PersonalSite's administrator **Site Activity → Redirect visits** to see visits.

For an application-specific link, repeat with an opaque slug such as
`application-2026-01` and an optional campaign. Slugs and URLs are public, not
passwords; do not include names, email addresses or private application details.
Campaigns appear in administrator analytics but should also avoid sensitive data.

To change a printed QR's destination, PUT the **same slug** with the new target,
preserving the intended campaign and active flag. No PDF regeneration is needed.
To disable it, PUT the same complete JSON with `isActive: false`. A disabled link
returns 404; existing history remains. Reactivating it restores the same public URL.

## Exact behavior and safety limits

- Slugs are case-sensitive: 1–80 lowercase ASCII letters/numbers, separated by
  single hyphens. `cv-en` is valid; `CV-en`, `cv_en`, `-cv` and `cv--en` are not.
- Target is an absolute HTTP(S) URL up to 2048 characters, without credentials,
  whitespace, backslashes or control characters. Campaign is optional, at most
  200 characters, single-line; blank clears it. Invalid saves return field-keyed
  HTTP 400 and leave the previous row intact. Authentication failures are 401/403.
- Only administrators choose destinations. A visitor's `target`, `campaign` or
  other query parameter cannot change the saved redirect. The server does not
  fetch the target. This is not an anonymous open-redirect service.
- Choose a real final destination, not another redirect slug that might create a
  loop. No external target health-check or redirect-chain crawler is introduced.
- Valid active GETs return **302**, `Cache-Control: no-store` and
  `Referrer-Policy: no-referrer`. Unknown, invalid or inactive slugs return 404,
  also no-store. A CDN must respect no-store; do not configure `/r/*` to cache.
- A visit records `LinkClicked`, anonymous user ID, and `Redirect` metadata with
  `slug`, `campaign`, `referrer`. Only the HTTP(S) **origin** of Referer is stored;
  credentials, private paths, queries and fragments are discarded. Missing,
  malformed, overlong or non-HTTP(S) referrers become `Direct / unavailable`.
  QR scanners often omit a referrer; a blank referrer does not prove a scan.
- Event and metadata saves are transactional: a failure rolls back the partial
  event. Analytics-write failure logs a generic warning without URL/header values
  and **still redirects**, so a printed link does not break just because telemetry
  failed. Such visits are missing from counts. Database lookup failure can still
  prevent redirecting; there is no secondary destination store.
- Counts are successful recorded requests, **not verified human scans**, email
  opens, unique people or proof of interest. Bots, link previews, refreshes and
  direct clicks may count. No fraud prevention, deduplication or retention worker
  is added. The existing public analytics ingestion also remains untrusted data.
- Historical visits retain the campaign at visit time. Editing the link does not
  rewrite history. Existing contact clicks remain a separate analytics section.

Analytics accepts the existing-style `from`, `to` (inclusive UTC timestamps),
`search` (slug/campaign/referrer), `sortBy=visits|slug`, and `descending=true|false`.
Defaults: visits descending. Each row is one slug/campaign/referrer-origin group;
the total reflects the current filters. No visitor-level export is added. Like
the existing contact analytics, the query loads LinkClicked metadata before
grouping; at larger volumes this may need a separate performance review.

## Deployment and verification checklist

**The owner controls deployment. Local implementation is not evidence that the
public site has changed.** No production endpoint was called during development.

1. Review the source and this contract; back up PostgreSQL through normal operations.
2. Deploy the API and frontend together through the existing process. Existing
   startup migration applies `20261101223529_AddRedirectLinks`: one new empty table
   and unique index only. Existing data is not rewritten. No new configuration,
   signing keys, credentials or CORS change is required. Keep the existing
   `/r/*` route reaching the API, not a reverse proxy's SPA-only fallback.
3. Confirm unauthenticated management/analytics reads return 401 and ordinary users
   get 403. Confirm unknown `/r/not-configured` returns 404, not the SPA HTML.
4. With explicit approval for the production write, configure one intended link.
   Inspect its GET response **without following redirects**: 302, exact saved
   Location and no-store. This test visit will itself be counted.
5. Check the **Redirect visits** section: correct slug/campaign and one visit; no
   increase in **Contact links**. Test search/date filters. Do not expect referrer
   details from a scanner that sends none.
6. If desired, approve a deliberate target change for that same slug, repeat the
   GET and confirm the new Location with the old QR URL. Do not change live printed
   destinations just for testing without owner approval.
7. Test inactive/unknown links return 404 and do not add visits. Changing a real
   active link to inactive is a separate deliberate production action.

Rollback: rolling back app code leaves the additive table/data in place but
disables the new routes/UI. Removing the migration via its Down drops the table
and **loses link configuration**; do not do that as a routine rollback. Earlier
LinkClicked history remains in the existing analytics tables. A deployed rollback
also makes already printed redirect URLs stop working; plan accordingly.

Local automated checks (disposable PostgreSQL; no production settings or targets):

```bash
dotnet build PersonalSite.sln --no-restore
TESTCONTAINERS_RYUK_DISABLED=true dotnet test PersonalSite.sln --no-restore --filter 'FullyQualifiedName~Redirect' -- xUnit.MaxParallelThreads=1
TESTCONTAINERS_RYUK_DISABLED=true dotnet test PersonalSite.sln --no-restore -- xUnit.MaxParallelThreads=1
npm --prefix Frontend run lint
npm --prefix Frontend run build
git diff --check
```

Tests cover authorization, saved-only targets, anonymous 302/no-store, sanitized
metadata, missing referrers, persisted history, target changes without slug changes,
disable/missing links, validation with no partial writes, concurrent creation,
filters/contact-count separation and telemetry rollback with redirect continuation.
CareerKit's existing frozen-letter export test also uses a `/r/application-2026-01`
QR URL and checks snapshot/download persistence without modifying templates.

### Verification status

Original redirect implementation verification: **465 PersonalSite backend tests passed**, zero failures or
skips, including all **27 new redirect tests**. CareerKit's full Release suite
passed **548 tests**, including the unchanged QR/export path exercised with a
redirect URL; all **152 frontend helper tests** passed. Both repositories' builds
and frontend lint/build passed. PersonalSite Debug/Release builds retain four
pre-existing nullable warnings; CareerKit retains its existing Vite bundle-size
advisory. EF reports no pending model changes (the installed EF CLI is 8.0.29 and
runtime 8.0.33). Diff checks passed. No browser-driven review of the new analytics
section or production requests were performed.

Value-object follow-up final verification: all **502 PersonalSite backend tests
pass**, zero failures/skips, including **64 focused redirect tests** (37 new
constructor/direct-call/persistence cases). Debug/Release builds, frontend lint/build,
diff checks and the no-pending-model-changes check pass. The four existing nullable
warnings and EF CLI/runtime version notice remain unchanged. Existing staged work is preserved;
the refactor and this doc update are unstaged, with no commit or push performed.

No PersonalSite API is currently running locally, so no existing process has been
restarted. Live deployment and production acceptance are **not yet verified**.
