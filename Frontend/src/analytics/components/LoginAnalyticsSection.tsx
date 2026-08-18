import { useEffect, useState } from "react";
import { useLoginAnalytics } from "../useLoginAnalytics";
import { AnalyticsFilters } from "./AnalyticsFilters";
import { AnalyticsPagination } from "./AnalyticsPagination";
import { formatFailureReason, toIsoDate } from "./analyticsHelpers";

type LoginAnalyticsSectionProps = {
  resetKey: number;
};

export function LoginAnalyticsSection({
  resetKey,
}: LoginAnalyticsSectionProps) {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState("");
  const [successful, setSuccessful] = useState<boolean | undefined>();
  const [sortBy, setSortBy] = useState("createdAt");
  const [descending, setDescending] = useState(true);
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");

  useEffect(() => {
    setPage(1);
    setPageSize(20);
    setSearch("");
    setSuccessful(undefined);
    setSortBy("createdAt");
    setDescending(true);
    setFrom("");
    setTo("");
  }, [resetKey]);

  const analyticsQuery = useLoginAnalytics({
    search: search || undefined,
    successful,
    from: toIsoDate(from),
    to: toIsoDate(to, true),
    sortBy,
    descending,
    page,
    pageSize,
  });

  if (analyticsQuery.isPending) {
    return (
      <div className="analytics-section">
        <p>Loading login analytics...</p>
      </div>
    );
  }

  if (analyticsQuery.isError) {
    return (
      <div className="analytics-section">
        <p>Could not load login analytics.</p>
      </div>
    );
  }

  const analytics = analyticsQuery.data;

  return (
    <>
      <div className="analytics-summary">
        <div className="analytics-summary__item">
          <span>Total attempts</span>
          <strong>{analytics.summary.totalAttempts}</strong>
        </div>

        <div className="analytics-summary__item">
          <span>Successful logins</span>
          <strong>{analytics.summary.successfulLogins}</strong>
        </div>

        <div className="analytics-summary__item">
          <span>Failed logins</span>
          <strong>{analytics.summary.failedLogins}</strong>
        </div>

        <div className="analytics-summary__item">
          <span>Unknown email</span>
          <strong>{analytics.summary.unknownEmailAttempts}</strong>
        </div>

        <div className="analytics-summary__item">
          <span>Incorrect password</span>
          <strong>{analytics.summary.incorrectPasswordAttempts}</strong>
        </div>
      </div>

      <div className="analytics-section">
        <div className="analytics-section__header">
          <div>
            <p className="account-card__eyebrow">Authentication</p>
            <h3>Login Activity</h3>
          </div>

          <span>{analytics.totalItems} events</span>
        </div>

        <AnalyticsFilters
          search={search}
          searchPlaceholder="Search user ID or reason..."
          sortBy={sortBy}
          sortOptions={[
            { value: "createdAt", label: "Date" },
            { value: "userId", label: "User" },
          ]}
          descending={descending}
          from={from}
          to={to}
          onSearchChange={(value) => {
            setSearch(value);
            setPage(1);
          }}
          onSortByChange={(value) => {
            setSortBy(value);
            setPage(1);
          }}
          onDescendingChange={(value) => {
            setDescending(value);
            setPage(1);
          }}
          onFromChange={(value) => {
            setFrom(value);
            setPage(1);
          }}
          onToChange={(value) => {
            setTo(value);
            setPage(1);
          }}
        >
          <label className="analytics-filter">
            <span>Status</span>

            <select
              value={
                successful === undefined
                  ? "all"
                  : successful
                    ? "successful"
                    : "failed"
              }
              onChange={(event) => {
                const value = event.target.value;

                setSuccessful(
                  value === "all" ? undefined : value === "successful",
                );

                setPage(1);
              }}
            >
              <option value="all">All</option>
              <option value="successful">Successful</option>
              <option value="failed">Failed</option>
            </select>
          </label>
        </AnalyticsFilters>

        <div className="analytics-table">
          <div className="analytics-table__header">
            <span>Status</span>
            <span>User</span>
            <span>Reason</span>
            <span>Date</span>
          </div>

          {analytics.items.map((activity) => (
            <div className="analytics-table__row" key={activity.id}>
              <span>
                <strong
                  className={
                    activity.successful
                      ? "analytics-status analytics-status--success"
                      : "analytics-status analytics-status--failed"
                  }
                >
                  {activity.successful ? "Successful" : "Failed"}
                </strong>
              </span>

              <span>
                {activity.userId === null ? "Unknown" : `#${activity.userId}`}
              </span>

              <span>{formatFailureReason(activity.failureReason)}</span>

              <span>{new Date(activity.createdAt).toLocaleString()}</span>
            </div>
          ))}
        </div>

        <AnalyticsPagination
          page={analytics.page}
          pageSize={analytics.pageSize}
          totalItems={analytics.totalItems}
          totalPages={analytics.totalPages}
          onPageChange={setPage}
          onPageSizeChange={(value) => {
            setPageSize(value);
            setPage(1);
          }}
        />
      </div>
    </>
  );
}
