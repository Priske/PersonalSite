import { useEffect, useState } from "react";
import { useContactLinkAnalytics } from "../useContactLinkAnalytics";
import { AnalyticsFilters } from "./AnalyticsFilters";
import { toIsoDate } from "./analyticsHelpers";

type ContactLinkAnalyticsSectionProps = {
  resetKey: number;
};

export function ContactLinkAnalyticsSection({
  resetKey,
}: ContactLinkAnalyticsSectionProps) {
  const [search, setSearch] = useState("");
  const [sortBy, setSortBy] = useState("clicks");
  const [descending, setDescending] = useState(true);
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");

  useEffect(() => {
    setSearch("");
    setSortBy("clicks");
    setDescending(true);
    setFrom("");
    setTo("");
  }, [resetKey]);

  const analyticsQuery = useContactLinkAnalytics({
    search: search || undefined,
    from: toIsoDate(from),
    to: toIsoDate(to, true),
    sortBy,
    descending,
  });

  if (analyticsQuery.isPending) {
    return <div className="analytics-section">Loading contact analytics...</div>;
  }

  if (analyticsQuery.isError) {
    return <div className="analytics-section">Could not load contact analytics.</div>;
  }

  const analytics = analyticsQuery.data;

  return (
    <div className="analytics-section analytics-section--referrers">
      <div className="analytics-section__header">
        <div>
          <p className="account-card__eyebrow">Interaction</p>
          <h3>Contact links</h3>
        </div>

        <span>{analytics.totalClicks} clicks</span>
      </div>

      <AnalyticsFilters
        search={search}
        searchPlaceholder="Search contact link..."
        sortBy={sortBy}
        sortOptions={[
          { value: "clicks", label: "Clicks" },
          { value: "label", label: "Link" },
        ]}
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
        <div className="analytics-referrers__header">
          <span>Contact link</span>
          <span>Clicks</span>
        </div>

        {analytics.links.map((link) => (
          <div className="analytics-referrers__row" key={link.label}>
            <span>{link.label}</span>
            <strong>{link.clicks}</strong>
          </div>
        ))}
      </div>
    </div>
  );
}
