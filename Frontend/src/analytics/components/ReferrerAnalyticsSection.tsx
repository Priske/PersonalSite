import { useEffect, useState } from "react";
import { useReferrerAnalytics } from "../useReferrerAnalytics";
import { AnalyticsFilters } from "./AnalyticsFilters";
import { formatReferrer, toIsoDate } from "./analyticsHelpers";

type ReferrerAnalyticsSectionProps = {
  resetKey: number;
};

export function ReferrerAnalyticsSection({
  resetKey,
}: ReferrerAnalyticsSectionProps) {
  const [search, setSearch] = useState("");
  const [sortBy, setSortBy] = useState("count");
  const [descending, setDescending] = useState(true);
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");

  useEffect(() => {
    setSearch("");
    setSortBy("count");
    setDescending(true);
    setFrom("");
    setTo("");
  }, [resetKey]);

  const analyticsQuery = useReferrerAnalytics({
    search: search || undefined,
    from: toIsoDate(from),
    to: toIsoDate(to, true),
    sortBy,
    descending,
  });

  if (analyticsQuery.isPending) {
    return (
      <div className="analytics-section">
        <p>Loading referrer analytics...</p>
      </div>
    );
  }

  if (analyticsQuery.isError) {
    return (
      <div className="analytics-section">
        <p>Could not load referrer analytics.</p>
      </div>
    );
  }

  const analytics = analyticsQuery.data;

  return (
    <div className="analytics-section analytics-section--referrers">
      <div className="analytics-section__header">
        <div>
          <p className="account-card__eyebrow">Traffic</p>
          <h3>Referrers</h3>
        </div>

        <span>{analytics.totalPageViews} page views</span>
      </div>

      <AnalyticsFilters
        search={search}
        searchPlaceholder="Search referrer..."
        sortBy={sortBy}
        sortOptions={[
          { value: "count", label: "Views" },
          { value: "referrer", label: "Referrer" },
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
          <span>Referrer</span>
          <span>Views</span>
        </div>

        {analytics.referrers.map((item) => (
          <div className="analytics-referrers__row" key={item.referrer}>
            <span>{formatReferrer(item.referrer)}</span>
            <strong>{item.count}</strong>
          </div>
        ))}
      </div>
    </div>
  );
}
