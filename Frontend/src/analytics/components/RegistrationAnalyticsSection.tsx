import { useEffect, useState } from "react";
import { useCreateUserAnalytics } from "../useCreateUserAnalytics";
import { AnalyticsFilters } from "./AnalyticsFilters";
import { AnalyticsPagination } from "./AnalyticsPagination";
import { toIsoDate } from "./analyticsHelpers";

type RegistrationAnalyticsSectionProps = {
  resetKey: number;
};

export function RegistrationAnalyticsSection({
  resetKey,
}: RegistrationAnalyticsSectionProps) {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState("");
  const [sortBy, setSortBy] = useState("createdAt");
  const [descending, setDescending] = useState(true);
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");

  useEffect(() => {
    setPage(1);
    setPageSize(20);
    setSearch("");
    setSortBy("createdAt");
    setDescending(true);
    setFrom("");
    setTo("");
  }, [resetKey]);

  const analyticsQuery = useCreateUserAnalytics({
    search: search || undefined,
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
        <p>Loading registration analytics...</p>
      </div>
    );
  }

  if (analyticsQuery.isError) {
    return (
      <div className="analytics-section">
        <p>Could not load registration analytics.</p>
      </div>
    );
  }

  const analytics = analyticsQuery.data;

  return (
    <div className="analytics-section analytics-section--registrations">
      <div className="analytics-section__header">
        <div>
          <p className="account-card__eyebrow">Users</p>
          <h3>Registrations</h3>
        </div>

        <span>{analytics.summary.totalCreatedUsers} registered</span>
      </div>

      <AnalyticsFilters
        search={search}
        searchPlaceholder="Search name or email..."
        sortBy={sortBy}
        sortOptions={[
          { value: "createdAt", label: "Created" },
          { value: "name", label: "Name" },
          { value: "email", label: "Email" },
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
      />

      <div className="analytics-registrations">
        <div className="analytics-registrations__header">
          <span>User</span>
          <span>Name</span>
          <span>Email</span>
          <span>Created</span>
        </div>

        {analytics.items.map((activity) => (
          <div className="analytics-registrations__row" key={activity.id}>
            <span>
              {activity.userId === null ? "Unknown" : `#${activity.userId}`}
            </span>

            <span>{activity.name ?? "—"}</span>
            <span>{activity.email ?? "—"}</span>

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
