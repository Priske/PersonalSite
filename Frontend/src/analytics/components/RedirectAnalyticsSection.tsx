import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { getRedirectAnalytics } from "../analyticsApi";
import { AnalyticsFilters } from "./AnalyticsFilters";
import { toIsoDate } from "./analyticsHelpers";
import { groupRedirectVisits } from "./redirectVisitGroups";

function referringWebsite(value: string) {
  try {
    const website = new URL(value);
    return ["http:", "https:"].includes(website.protocol) && website.hostname
      ? value
      : null;
  } catch {
    return null;
  }
}

export function RedirectAnalyticsSection({ resetKey }: { resetKey: number }) {
  const [search, setSearch] = useState("");
  const [sortBy, setSortBy] = useState("visits");
  const [descending, setDescending] = useState(true);
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");

  useEffect(() => {
    setSearch("");
    setSortBy("visits");
    setDescending(true);
    setFrom("");
    setTo("");
  }, [resetKey]);

  const request = {
    search: search || undefined,
    sortBy,
    descending,
    from: toIsoDate(from),
    to: toIsoDate(to, true),
  };
  const query = useQuery({
    queryKey: ["analytics", "redirect-links", request],
    queryFn: () => getRedirectAnalytics(request),
  });

  if (query.isPending) return <div className="analytics-section">Loading redirect visits...</div>;
  if (query.isError) return <div className="analytics-section">Could not load redirect visits.</div>;

  const groups = groupRedirectVisits(query.data.links, sortBy, descending);

  return (
    <section className="analytics-section analytics-section--referrers redirect-visits" aria-label="Redirect visits">
      <div className="analytics-section__header">
        <div>
          <p className="account-card__eyebrow">Document interest</p>
          <h3>Redirect visits</h3>
          <p className="redirect-visits-description">See which named links brought visitors to your website. Open a link's details for its campaign and referrer breakdown.</p>
        </div>
        <span className="redirect-visits-total"><strong>{query.data.totalVisits}</strong> matching {query.data.totalVisits === 1 ? "visit" : "visits"}</span>
      </div>
      <AnalyticsFilters
        search={search}
        searchPlaceholder="Search link, campaign or referrer..."
        sortBy={sortBy}
        sortOptions={[{ value: "visits", label: "Visits" }, { value: "slug", label: "Link" }]}
        descending={descending}
        from={from}
        to={to}
        onSearchChange={setSearch}
        onSortByChange={setSortBy}
        onDescendingChange={setDescending}
        onFromChange={setFrom}
        onToChange={setTo}
      />
      <p className="redirect-visits-note">Totals reflect your filters. Visits may include bots and link previews; they are not verified QR scans, unique people or email opens.</p>
      {groups.length === 0 ? (
        <p className="redirect-visits-empty">No redirect visits match these filters.</p>
      ) : (
        <div className="redirect-visits-list">
          <div className="redirect-visits-columns" aria-hidden="true"><span>Link</span><span>Visits</span><span /></div>
          {groups.map((group) => (
            <details className="redirect-visits-link" key={group.slug}>
              <summary aria-label={`${group.slug}: ${group.visits} ${group.visits === 1 ? "visit" : "visits"}. Show campaign and referrer details.`}>
                <span className="redirect-visits-name"><strong>{group.slug}</strong></span>
                <span className="redirect-visits-count"><strong>{group.visits}</strong></span>
                <span className="redirect-visits-toggle" aria-hidden="true" />
              </summary>
              <div className="redirect-visits-breakdown">
                <h4>Entry link: <code>/r/{group.slug}</code></h4>
                <p className="redirect-visits-detail-help">These visitors used this redirect. The campaign label is the optional label you assigned to it.</p>
                <ul>
                  {group.details.map((detail) => {
                    const website = referringWebsite(detail.referrer);
                    return (
                      <li key={JSON.stringify([detail.campaign, detail.referrer])}>
                        <dl>
                          <div><dt>Campaign label</dt><dd>{detail.campaign ?? "Not set"}</dd></div>
                          {website && <div><dt>Referring website</dt><dd>{website}</dd></div>}
                          <div className="redirect-visits-detail-count"><dt>Recorded visits</dt><dd>{detail.visits}</dd></div>
                        </dl>
                      </li>
                    );
                  })}
                </ul>
              </div>
            </details>
          ))}
        </div>
      )}
    </section>
  );
}
