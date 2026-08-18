import { useEffect, useState } from "react";
import { useDeleteUserAnalytics } from "../useDeleteUserAnalytics";
import { AnalyticsFilters } from "./AnalyticsFilters";
import { AnalyticsPagination } from "./AnalyticsPagination";
import { formatDeleteFailureReason, toIsoDate } from "./analyticsHelpers";

type DeleteUserAnalyticsSectionProps = {
  resetKey: number;
};

export function DeleteUserAnalyticsSection({
  resetKey,
}: DeleteUserAnalyticsSectionProps) {
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

  const analyticsQuery = useDeleteUserAnalytics({
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
        <p>Loading deleted-user analytics...</p>
      </div>
    );
  }

  if (analyticsQuery.isError) {
    return (
      <div className="analytics-section">
        <p>Could not load deleted-user analytics.</p>
      </div>
    );
  }

  const analytics = analyticsQuery.data;

  return (
    <div className="analytics-section">
      <div className="analytics-section__header">
        <div>
          <p className="account-card__eyebrow">Users</p>
          <h3>Deleted Users</h3>
        </div>

        <span>{analytics.summary.totalAttempts} attempts</span>
      </div>

      <AnalyticsFilters
        search={search}
        searchPlaceholder="Search actor, target or reason..."
        sortBy={sortBy}
        sortOptions={[
          { value: "createdAt", label: "Date" },
          { value: "userId", label: "Actor" },
          { value: "targetUserId", label: "Target" },
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

      <div className="analytics-delete-summary">
        <div>
          <span>Successful</span>
          <strong>{analytics.summary.successfulDeletes}</strong>
        </div>

        <div>
          <span>Failed</span>
          <strong>{analytics.summary.failedDeletes}</strong>
        </div>
      </div>

      <div className="analytics-delete-users">
        <div className="analytics-delete-users__header">
          <span>Status</span>
          <span>Actor</span>
          <span>Target</span>
          <span>Reason</span>
          <span>Date</span>
        </div>

        {analytics.items.map((activity) => (
          <div className="analytics-delete-users__row" key={activity.id}>
            <span>
              <strong
                className={
                  activity.successful
                    ? "analytics-status analytics-status--success"
                    : "analytics-status analytics-status--failed"
                }
              >
                {activity.successful ? "Deleted" : "Failed"}
              </strong>
            </span>

            <span>
              {activity.userId === null ? "Unknown" : `#${activity.userId}`}
            </span>

            <span>
              {activity.targetUserId === null
                ? "Unknown"
                : `#${activity.targetUserId}`}
            </span>

            <span>{formatDeleteFailureReason(activity.failureReason)}</span>

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
  );
}
