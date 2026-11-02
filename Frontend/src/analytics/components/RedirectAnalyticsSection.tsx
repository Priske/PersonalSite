import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { getRedirectAnalytics } from "../analyticsApi";
import { AnalyticsFilters } from "./AnalyticsFilters";
import { toIsoDate } from "./analyticsHelpers";

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

  return (
    <div className="analytics-section analytics-section--referrers">
      <div className="analytics-section__header">
        <div>
          <p className="account-card__eyebrow">Document interest</p>
          <h3>Redirect visits</h3>
          <p>Visits may include bots and link previews. They are not verified QR scans or email opens.</p>
        </div>
        <span>{query.data.totalVisits} visits</span>
      </div>
      <AnalyticsFilters
        search={search}
        searchPlaceholder="Search slug, campaign or referrer..."
        sortBy={sortBy}
        sortOptions={[{ value: "visits", label: "Visits" }, { value: "slug", label: "Slug" }]}
        descending={descending}
        from={from}
        to={to}
        onSearchChange={setSearch}
        onSortByChange={setSortBy}
        onDescendingChange={setDescending}
        onFromChange={setFrom}
        onToChange={setTo}
      />
      <div className="analytics-referrers">
        <div className="analytics-referrers__header"><span>Link</span><span>Visits</span></div>
        {query.data.links.length === 0 && <p>No redirect visits in this selection.</p>}
        {query.data.links.map((link) => (
          <div className="analytics-referrers__row" key={JSON.stringify([link.slug, link.campaign, link.referrer])}>
            <span><strong>{link.slug}</strong><br />{link.campaign ?? "No campaign"}<br />{link.referrer}</span>
            <strong>{link.visits}</strong>
          </div>
        ))}
      </div>
    </div>
  );
}
