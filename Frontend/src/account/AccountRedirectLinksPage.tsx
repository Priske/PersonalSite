import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState, type FormEvent } from "react";
import { ApiError, apiRequest } from "../api";
import "./redirect-links.css";

type RedirectLink = {
  slug: string;
  targetUrl: string;
  campaign: string | null;
  isActive: boolean;
  createdAt: string;
};

type RedirectDraft = {
  slug: string;
  targetUrl: string;
  campaign: string;
  isActive: boolean;
};

const queryKey = ["redirect-links"];

function draftFor(link: RedirectLink | null): RedirectDraft {
  return {
    slug: link?.slug ?? "",
    targetUrl: link?.targetUrl ?? "",
    campaign: link?.campaign ?? "",
    isActive: link?.isActive ?? false,
  };
}

function hasControlCharacters(value: string) {
  return [...value].some((character) => {
    const code = character.charCodeAt(0);
    return code < 32 || (code >= 127 && code <= 159);
  });
}

function validate(draft: RedirectDraft): string | null {
  if (
    !draft.slug || draft.slug.length > 80 || draft.slug.trim() !== draft.slug ||
    !/^[a-z0-9]+(?:-[a-z0-9]+)*$/.test(draft.slug)
  ) {
    return "Use a slug of up to 80 lowercase letters, numbers and single separating hyphens.";
  }

  try {
    const target = new URL(draft.targetUrl);
    if (
      draft.targetUrl.length > 2048 || /\s|\\/.test(draft.targetUrl) ||
      hasControlCharacters(draft.targetUrl) ||
      !["http:", "https:"].includes(target.protocol) || !target.hostname ||
      target.username || target.password
    ) {
      return "Enter a complete HTTP(S) destination without credentials, spaces or control characters (up to 2048 characters).";
    }
  } catch {
    return "Enter a complete destination starting with https:// or http://.";
  }

  if (draft.campaign.length > 200 || hasControlCharacters(draft.campaign)) {
    return "Use a single-line campaign of up to 200 characters.";
  }
  return null;
}

export function AccountRedirectLinksPage() {
  const queryClient = useQueryClient();
  const [selected, setSelected] = useState<RedirectLink | null>(null);
  const [draft, setDraft] = useState<RedirectDraft | null>(null);
  const [formError, setFormError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  const linksQuery = useQuery({
    queryKey,
    queryFn: ({ signal }) => apiRequest<RedirectLink[]>("/redirect-links", { signal }),
    retry: false,
    refetchOnWindowFocus: false,
  });

  const saveMutation = useMutation({
    mutationFn: (value: RedirectDraft) =>
      apiRequest<RedirectLink>(`/redirect-links/${encodeURIComponent(value.slug)}`, {
        method: "PUT",
        body: JSON.stringify({
          targetUrl: value.targetUrl,
          campaign: value.campaign.trim() || null,
          isActive: value.isActive,
        }),
      }),
    retry: false,
  });

  const dirty = draft !== null && JSON.stringify(draft) !== JSON.stringify(draftFor(selected));

  function mayDiscard() {
    return !dirty || window.confirm("Discard your unsaved redirect-link changes?");
  }

  function edit(link: RedirectLink | null) {
    if (!mayDiscard()) return;
    setSelected(link);
    setDraft(draftFor(link));
    setFormError(null);
    setMessage(null);
    saveMutation.reset();
  }

  function cancel() {
    if (!mayDiscard()) return;
    setSelected(null);
    setDraft(null);
    setFormError(null);
    setMessage(null);
    saveMutation.reset();
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!draft || saveMutation.isPending) return;
    setMessage(null);
    const error = validate(draft);
    if (error) {
      setFormError(error);
      return;
    }
    if (!selected && linksQuery.data?.some((link) => link.slug === draft.slug)) {
      setFormError("That slug already exists. Choose its Edit button to change the existing link.");
      return;
    }
    setFormError(null);
    try {
      const saved = await saveMutation.mutateAsync(draft);
      queryClient.setQueryData<RedirectLink[]>(queryKey, (links) =>
        [...(links ?? []).filter((link) => link.slug !== saved.slug), saved]
          .sort((left, right) => left.slug.localeCompare(right.slug)),
      );
      setSelected(saved);
      setDraft(draftFor(saved));
      setMessage(saved.isActive ? "Link saved and active." : "Link saved as inactive.");
    } catch (error) {
      if (error instanceof ApiError && (error.status === 401 || error.status === 403)) {
        setFormError("An active Administrator session is required to save links. Your edits are kept.");
      } else if (error instanceof ApiError && error.status === 400) {
        setFormError("The link was rejected. Check the slug, destination and campaign. Your edits are kept.");
      } else {
        setFormError("Could not confirm the save. Your edits are kept. Reload the links before retrying; the save may have reached the server.");
      }
    }
  }

  return (
    <article className="account-card redirect-links-page">
      <header className="account-card__header account-management__header">
        <div>
          <p className="account-card__eyebrow">Website management</p>
          <h2>Redirect links</h2>
          <p className="account-management__description">
            Create stable links for QR codes and shared documents. Change a destination
            later without replacing the original link.
          </p>
        </div>
        <button className="button" type="button" onClick={() => edit(null)} disabled={!linksQuery.data || saveMutation.isPending}>
          Add link
        </button>
      </header>

      <div className="account-management__body">
        <div className="redirect-links-list-heading">
          <h3>Saved links</h3>
          <button className="button button--secondary" type="button" onClick={() => void linksQuery.refetch()} disabled={linksQuery.isFetching || saveMutation.isPending}>
            {linksQuery.isFetching ? "Loading..." : "Reload links"}
          </button>
        </div>
        {linksQuery.isPending && <p role="status">Loading redirect links...</p>}
        {linksQuery.isError && (
          <p className="form-message form-message--error" role="alert">
            Could not load redirect links. Check your Administrator session and try Reload links.
          </p>
        )}
        {linksQuery.isSuccess && linksQuery.data.length === 0 && <p>No redirect links yet. Choose Add link to create one.</p>}
        {linksQuery.data && linksQuery.data.length > 0 && (
          <ul className="redirect-links-list">
            {linksQuery.data.map((link) => (
              <li key={link.slug} className="redirect-links-item">
                <div>
                  <h4>{link.slug} <span className="redirect-links-status">{link.isActive ? "Active" : "Inactive"}</span></h4>
                  <dl>
                    <dt>Redirect URL</dt><dd><code>{window.location.origin}/r/{link.slug}</code></dd>
                    <dt>Destination</dt><dd>{link.targetUrl}</dd>
                    <dt>Campaign</dt><dd>{link.campaign ?? "None"}</dd>
                  </dl>
                </div>
                <button className="button button--secondary" type="button" onClick={() => edit(link)} disabled={saveMutation.isPending}>
                  Edit {link.slug}
                </button>
              </li>
            ))}
          </ul>
        )}

        {draft && (
          <section className="redirect-links-editor" aria-labelledby="redirect-editor-title">
            <h3 id="redirect-editor-title">{selected ? `Edit ${selected.slug}` : "New redirect link"}</h3>
            <form className="edit-account-form" onSubmit={(event) => void submit(event)} aria-label="Redirect link">
              <fieldset disabled={saveMutation.isPending}>
                <legend className="redirect-links-legend">Link details</legend>
                <div className="edit-account-form__field">
                  <label htmlFor="redirect-slug">Slug</label>
                  <input id="redirect-slug" name="slug" value={draft.slug} onChange={(event) => setDraft({ ...draft, slug: event.target.value })}
                    readOnly={selected !== null} required maxLength={80} pattern="[a-z0-9]+(-[a-z0-9]+)*" aria-describedby="redirect-slug-help" autoComplete="off" />
                  <small id="redirect-slug-help">Lowercase letters, numbers and separating hyphens, for example cv. Saved slugs stay fixed so printed links keep working.</small>
                </div>
                <div className="edit-account-form__field">
                  <label htmlFor="redirect-target">Destination URL</label>
                  <input id="redirect-target" name="targetUrl" type="url" value={draft.targetUrl} onChange={(event) => setDraft({ ...draft, targetUrl: event.target.value })}
                    required maxLength={2048} placeholder="https://example.com/" autoComplete="off" />
                </div>
                <div className="edit-account-form__field">
                  <label htmlFor="redirect-campaign">Campaign (optional)</label>
                  <input id="redirect-campaign" name="campaign" value={draft.campaign} onChange={(event) => setDraft({ ...draft, campaign: event.target.value })}
                    maxLength={200} autoComplete="off" />
                </div>
                <label className="redirect-links-active">
                  <input name="isActive" type="checkbox" checked={draft.isActive} onChange={(event) => setDraft({ ...draft, isActive: event.target.checked })} />
                  Active
                </label>
                <p>Active links redirect visitors. Inactive links return a not-found page. Changes take effect when you save.</p>
                <div className="edit-account-form__messages" aria-live="polite">
                  {formError && <p className="form-message form-message--error" role="alert">{formError}</p>}
                  {message && <p className="form-message" role="status">{message}</p>}
                </div>
                <div className="edit-account-form__actions">
                  <button className="button button--secondary" type="button" onClick={cancel}>Cancel</button>
                  <button className="button" type="submit" disabled={linksQuery.isPending || linksQuery.isError}>
                    {saveMutation.isPending ? "Saving..." : "Save link"}
                  </button>
                </div>
              </fieldset>
            </form>
          </section>
        )}
      </div>
    </article>
  );
}
