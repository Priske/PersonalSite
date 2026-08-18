import { useState } from "react";
import { useCreateUserAnalytics } from "../analytics/useCreateUserAnalytics";
import { useDeleteUserAnalytics } from "../analytics/useDeleteUserAnalytics";
import { useLoginAnalytics } from "../analytics/useLoginAnalytics";
import { useReferrerAnalytics } from "../analytics/useReferrerAnalytics";

type SortOption = {
  value: string;
  label: string;
};

type AnalyticsFiltersProps = {
  search: string;
  searchPlaceholder: string;
  sortBy: string;
  sortOptions: SortOption[];
  descending: boolean;
  from: string;
  to: string;
  onSearchChange: (value: string) => void;
  onSortByChange: (value: string) => void;
  onDescendingChange: (value: boolean) => void;
  onFromChange: (value: string) => void;
  onToChange: (value: string) => void;
};

type AnalyticsPaginationProps = {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
};

export function AnalyticsAdminPage() {
  const [loginPage, setLoginPage] = useState(1);
  const [loginPageSize, setLoginPageSize] = useState(20);
  const [loginSearch, setLoginSearch] = useState("");
  const [loginSuccessful, setLoginSuccessful] = useState<boolean | undefined>();
  const [loginSortBy, setLoginSortBy] = useState("createdAt");
  const [loginDescending, setLoginDescending] = useState(true);
  const [loginFrom, setLoginFrom] = useState("");
  const [loginTo, setLoginTo] = useState("");

  const [referrerSearch, setReferrerSearch] = useState("");
  const [referrerSortBy, setReferrerSortBy] = useState("count");
  const [referrerDescending, setReferrerDescending] = useState(true);
  const [referrerFrom, setReferrerFrom] = useState("");
  const [referrerTo, setReferrerTo] = useState("");

  const [registrationPage, setRegistrationPage] = useState(1);
  const [registrationPageSize, setRegistrationPageSize] = useState(20);
  const [registrationSearch, setRegistrationSearch] = useState("");
  const [registrationSortBy, setRegistrationSortBy] = useState("createdAt");
  const [registrationDescending, setRegistrationDescending] = useState(true);
  const [registrationFrom, setRegistrationFrom] = useState("");
  const [registrationTo, setRegistrationTo] = useState("");

  const [deletePage, setDeletePage] = useState(1);
  const [deletePageSize, setDeletePageSize] = useState(20);
  const [deleteSearch, setDeleteSearch] = useState("");
  const [deleteSuccessful, setDeleteSuccessful] = useState<
    boolean | undefined
  >();
  const [deleteSortBy, setDeleteSortBy] = useState("createdAt");
  const [deleteDescending, setDeleteDescending] = useState(true);
  const [deleteFrom, setDeleteFrom] = useState("");
  const [deleteTo, setDeleteTo] = useState("");

  function clearAllFilters() {
    setLoginPage(1);
    setLoginPageSize(20);
    setLoginSearch("");
    setLoginSuccessful(undefined);
    setLoginSortBy("createdAt");
    setLoginDescending(true);
    setLoginFrom("");
    setLoginTo("");

    setReferrerSearch("");
    setReferrerSortBy("count");
    setReferrerDescending(true);
    setReferrerFrom("");
    setReferrerTo("");

    setRegistrationPage(1);
    setRegistrationPageSize(20);
    setRegistrationSearch("");
    setRegistrationSortBy("createdAt");
    setRegistrationDescending(true);
    setRegistrationFrom("");
    setRegistrationTo("");

    setDeletePage(1);
    setDeletePageSize(20);
    setDeleteSearch("");
    setDeleteSuccessful(undefined);
    setDeleteSortBy("createdAt");
    setDeleteDescending(true);
    setDeleteFrom("");
    setDeleteTo("");
  }
  const loginAnalyticsQuery = useLoginAnalytics({
    search: loginSearch || undefined,
    successful: loginSuccessful,
    from: toIsoDate(loginFrom),
    to: toIsoDate(loginTo, true),
    sortBy: loginSortBy,
    descending: loginDescending,
    page: loginPage,
    pageSize: loginPageSize,
  });

  const referrerAnalyticsQuery = useReferrerAnalytics({
    search: referrerSearch || undefined,
    from: toIsoDate(referrerFrom),
    to: toIsoDate(referrerTo, true),
    sortBy: referrerSortBy,
    descending: referrerDescending,
  });

  const createUserAnalyticsQuery = useCreateUserAnalytics({
    search: registrationSearch || undefined,
    from: toIsoDate(registrationFrom),
    to: toIsoDate(registrationTo, true),
    sortBy: registrationSortBy,
    descending: registrationDescending,
    page: registrationPage,
    pageSize: registrationPageSize,
  });

  const deleteUserAnalyticsQuery = useDeleteUserAnalytics({
    search: deleteSearch || undefined,
    successful: deleteSuccessful,
    from: toIsoDate(deleteFrom),
    to: toIsoDate(deleteTo, true),
    sortBy: deleteSortBy,
    descending: deleteDescending,
    page: deletePage,
    pageSize: deletePageSize,
  });

  if (
    loginAnalyticsQuery.isPending ||
    referrerAnalyticsQuery.isPending ||
    createUserAnalyticsQuery.isPending ||
    deleteUserAnalyticsQuery.isPending
  ) {
    return (
      <section className="account-card">
        <div className="account-management__body">
          <p>Loading analytics...</p>
        </div>
      </section>
    );
  }

  if (
    loginAnalyticsQuery.isError ||
    referrerAnalyticsQuery.isError ||
    createUserAnalyticsQuery.isError ||
    deleteUserAnalyticsQuery.isError
  ) {
    return (
      <section className="account-card">
        <div className="account-management__body">
          <p>Could not load analytics.</p>
        </div>
      </section>
    );
  }

  const loginAnalytics = loginAnalyticsQuery.data;
  const referrerAnalytics = referrerAnalyticsQuery.data;
  const createUserAnalytics = createUserAnalyticsQuery.data;
  const deleteUserAnalytics = deleteUserAnalyticsQuery.data;

  return (
    <section className="account-card analytics">
      <header className="account-card__header analytics-page-header">
        <div>
          <p className="account-card__eyebrow">Analytics</p>

          <h2>Site Activity</h2>

          <p className="account-management__description">
            Monitor authentication activity, user registrations and website
            usage.
          </p>
        </div>

        <button
          type="button"
          className="analytics-clear-button"
          onClick={clearAllFilters}
        >
          Clear all filters
        </button>
      </header>

      <div className="analytics-summary">
        <div className="analytics-summary__item">
          <span>Total attempts</span>
          <strong>{loginAnalytics.summary.totalAttempts}</strong>
        </div>

        <div className="analytics-summary__item">
          <span>Successful logins</span>
          <strong>{loginAnalytics.summary.successfulLogins}</strong>
        </div>

        <div className="analytics-summary__item">
          <span>Failed logins</span>
          <strong>{loginAnalytics.summary.failedLogins}</strong>
        </div>

        <div className="analytics-summary__item">
          <span>Unknown email</span>
          <strong>{loginAnalytics.summary.unknownEmailAttempts}</strong>
        </div>

        <div className="analytics-summary__item">
          <span>Incorrect password</span>
          <strong>{loginAnalytics.summary.incorrectPasswordAttempts}</strong>
        </div>
      </div>

      <div className="analytics-section">
        <div className="analytics-section__header">
          <div>
            <p className="account-card__eyebrow">Authentication</p>
            <h3>Login Activity</h3>
          </div>

          <span>{loginAnalytics.totalItems} events</span>
        </div>

        <AnalyticsFilters
          search={loginSearch}
          searchPlaceholder="Search user or reason..."
          sortBy={loginSortBy}
          sortOptions={[
            { value: "createdAt", label: "Date" },
            { value: "userId", label: "User" },
          ]}
          descending={loginDescending}
          from={loginFrom}
          to={loginTo}
          onSearchChange={(value) => {
            setLoginSearch(value);
            setLoginPage(1);
          }}
          onSortByChange={(value) => {
            setLoginSortBy(value);
            setLoginPage(1);
          }}
          onDescendingChange={(value) => {
            setLoginDescending(value);
            setLoginPage(1);
          }}
          onFromChange={(value) => {
            setLoginFrom(value);
            setLoginPage(1);
          }}
          onToChange={(value) => {
            setLoginTo(value);
            setLoginPage(1);
          }}
        >
          <label className="analytics-filter">
            <span>Status</span>

            <select
              value={
                loginSuccessful === undefined
                  ? "all"
                  : loginSuccessful
                    ? "successful"
                    : "failed"
              }
              onChange={(event) => {
                const value = event.target.value;

                setLoginSuccessful(
                  value === "all" ? undefined : value === "successful",
                );

                setLoginPage(1);
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

          {loginAnalytics.items.map((activity) => (
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
          page={loginAnalytics.page}
          pageSize={loginAnalytics.pageSize}
          totalItems={loginAnalytics.totalItems}
          totalPages={loginAnalytics.totalPages}
          onPageChange={setLoginPage}
          onPageSizeChange={(pageSize) => {
            setLoginPageSize(pageSize);
            setLoginPage(1);
          }}
        />
      </div>

      <div className="analytics-section analytics-section--referrers">
        <div className="analytics-section__header">
          <div>
            <p className="account-card__eyebrow">Traffic</p>
            <h3>Referrers</h3>
          </div>

          <span>{referrerAnalytics.totalPageViews} page views</span>
        </div>

        <AnalyticsFilters
          search={referrerSearch}
          searchPlaceholder="Search referrer..."
          sortBy={referrerSortBy}
          sortOptions={[
            { value: "count", label: "Views" },
            { value: "referrer", label: "Referrer" },
          ]}
          descending={referrerDescending}
          from={referrerFrom}
          to={referrerTo}
          onSearchChange={setReferrerSearch}
          onSortByChange={setReferrerSortBy}
          onDescendingChange={setReferrerDescending}
          onFromChange={setReferrerFrom}
          onToChange={setReferrerTo}
        />

        <div className="analytics-referrers">
          <div className="analytics-referrers__header">
            <span>Referrer</span>
            <span>Views</span>
          </div>

          {referrerAnalytics.referrers.map((item) => (
            <div className="analytics-referrers__row" key={item.referrer}>
              <span>{formatReferrer(item.referrer)}</span>
              <strong>{item.count}</strong>
            </div>
          ))}
        </div>
      </div>

      <div className="analytics-section analytics-section--registrations">
        <div className="analytics-section__header">
          <div>
            <p className="account-card__eyebrow">Users</p>
            <h3>Registrations</h3>
          </div>

          <span>
            {createUserAnalytics.summary.totalCreatedUsers} registered
          </span>
        </div>

        <AnalyticsFilters
          search={registrationSearch}
          searchPlaceholder="Search name or email..."
          sortBy={registrationSortBy}
          sortOptions={[
            { value: "createdAt", label: "Created" },
            { value: "name", label: "Name" },
            { value: "email", label: "Email" },
          ]}
          descending={registrationDescending}
          from={registrationFrom}
          to={registrationTo}
          onSearchChange={(value) => {
            setRegistrationSearch(value);
            setRegistrationPage(1);
          }}
          onSortByChange={(value) => {
            setRegistrationSortBy(value);
            setRegistrationPage(1);
          }}
          onDescendingChange={(value) => {
            setRegistrationDescending(value);
            setRegistrationPage(1);
          }}
          onFromChange={(value) => {
            setRegistrationFrom(value);
            setRegistrationPage(1);
          }}
          onToChange={(value) => {
            setRegistrationTo(value);
            setRegistrationPage(1);
          }}
        />

        <div className="analytics-registrations">
          <div className="analytics-registrations__header">
            <span>User</span>
            <span>Name</span>
            <span>Email</span>
            <span>Created</span>
          </div>

          {createUserAnalytics.items.map((activity) => (
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
          page={createUserAnalytics.page}
          pageSize={createUserAnalytics.pageSize}
          totalItems={createUserAnalytics.totalItems}
          totalPages={createUserAnalytics.totalPages}
          onPageChange={setRegistrationPage}
          onPageSizeChange={(pageSize) => {
            setRegistrationPageSize(pageSize);
            setRegistrationPage(1);
          }}
        />
      </div>

      <div className="analytics-section">
        <div className="analytics-section__header">
          <div>
            <p className="account-card__eyebrow">Users</p>
            <h3>Deleted Users</h3>
          </div>

          <span>{deleteUserAnalytics.summary.totalAttempts} attempts</span>
        </div>

        <AnalyticsFilters
          search={deleteSearch}
          searchPlaceholder="Search actor, target or reason..."
          sortBy={deleteSortBy}
          sortOptions={[
            { value: "createdAt", label: "Date" },
            { value: "userId", label: "Actor" },
            { value: "targetUserId", label: "Target" },
          ]}
          descending={deleteDescending}
          from={deleteFrom}
          to={deleteTo}
          onSearchChange={(value) => {
            setDeleteSearch(value);
            setDeletePage(1);
          }}
          onSortByChange={(value) => {
            setDeleteSortBy(value);
            setDeletePage(1);
          }}
          onDescendingChange={(value) => {
            setDeleteDescending(value);
            setDeletePage(1);
          }}
          onFromChange={(value) => {
            setDeleteFrom(value);
            setDeletePage(1);
          }}
          onToChange={(value) => {
            setDeleteTo(value);
            setDeletePage(1);
          }}
        >
          <label className="analytics-filter">
            <span>Status</span>

            <select
              value={
                deleteSuccessful === undefined
                  ? "all"
                  : deleteSuccessful
                    ? "successful"
                    : "failed"
              }
              onChange={(event) => {
                const value = event.target.value;

                setDeleteSuccessful(
                  value === "all" ? undefined : value === "successful",
                );

                setDeletePage(1);
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
            <strong>{deleteUserAnalytics.summary.successfulDeletes}</strong>
          </div>

          <div>
            <span>Failed</span>
            <strong>{deleteUserAnalytics.summary.failedDeletes}</strong>
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

          {deleteUserAnalytics.items.map((activity) => (
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
          page={deleteUserAnalytics.page}
          pageSize={deleteUserAnalytics.pageSize}
          totalItems={deleteUserAnalytics.totalItems}
          totalPages={deleteUserAnalytics.totalPages}
          onPageChange={setDeletePage}
          onPageSizeChange={(pageSize) => {
            setDeletePageSize(pageSize);
            setDeletePage(1);
          }}
        />
      </div>
    </section>
  );
}

function AnalyticsFilters({
  search,
  searchPlaceholder,
  sortBy,
  sortOptions,
  descending,
  from,
  to,
  onSearchChange,
  onSortByChange,
  onDescendingChange,
  onFromChange,
  onToChange,
  children,
}: AnalyticsFiltersProps & {
  children?: React.ReactNode;
}) {
  return (
    <div className="analytics-filters">
      <label className="analytics-filter analytics-filter--search">
        <span>Search</span>

        <input
          type="search"
          value={search}
          placeholder={searchPlaceholder}
          onChange={(event) => onSearchChange(event.target.value)}
        />
      </label>

      {children}

      <label className="analytics-filter">
        <span>From</span>

        <input
          type="date"
          value={from}
          onChange={(event) => onFromChange(event.target.value)}
        />
      </label>

      <label className="analytics-filter">
        <span>To</span>

        <input
          type="date"
          value={to}
          onChange={(event) => onToChange(event.target.value)}
        />
      </label>

      <label className="analytics-filter">
        <span>Sort by</span>

        <select
          value={sortBy}
          onChange={(event) => onSortByChange(event.target.value)}
        >
          {sortOptions.map((option) => (
            <option value={option.value} key={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </label>

      <label className="analytics-filter">
        <span>Order</span>

        <select
          value={descending ? "descending" : "ascending"}
          onChange={(event) =>
            onDescendingChange(event.target.value === "descending")
          }
        >
          <option value="descending">Descending</option>
          <option value="ascending">Ascending</option>
        </select>
      </label>
    </div>
  );
}

function AnalyticsPagination({
  page,
  pageSize,
  totalItems,
  totalPages,
  onPageChange,
  onPageSizeChange,
}: AnalyticsPaginationProps) {
  return (
    <div className="analytics-pagination">
      <div className="analytics-pagination__summary">
        <span>
          Page {page} of {Math.max(totalPages, 1)}
        </span>

        <span>{totalItems} total</span>
      </div>

      <div className="analytics-pagination__controls">
        <label className="analytics-pagination__size">
          <span>Rows</span>

          <select
            value={pageSize}
            onChange={(event) => onPageSizeChange(Number(event.target.value))}
          >
            <option value={10}>10</option>
            <option value={20}>20</option>
            <option value={50}>50</option>
          </select>
        </label>

        <button
          type="button"
          className="analytics-pagination__button"
          disabled={page <= 1}
          onClick={() => onPageChange(page - 1)}
        >
          Previous
        </button>

        <button
          type="button"
          className="analytics-pagination__button"
          disabled={totalPages === 0 || page >= totalPages}
          onClick={() => onPageChange(page + 1)}
        >
          Next
        </button>
      </div>
    </div>
  );
}

function toIsoDate(value: string, endOfDay = false) {
  if (!value) {
    return undefined;
  }

  const date = new Date(
    `${value}T${endOfDay ? "23:59:59.999" : "00:00:00.000"}`,
  );

  return date.toISOString();
}

function formatFailureReason(reason: string | null) {
  switch (reason) {
    case "unknown_email":
      return "Unknown email";

    case "incorrect_password":
      return "Incorrect password";

    case "missing_credentials":
      return "Missing credentials";

    default:
      return "—";
  }
}

function formatReferrer(referrer: string) {
  if (referrer === "Direct") {
    return "Direct";
  }

  try {
    return new URL(referrer).hostname;
  } catch {
    return referrer;
  }
}

function formatDeleteFailureReason(reason: string | null) {
  switch (reason) {
    case "unknown_delete_user":
      return "Unknown user";

    default:
      return "—";
  }
}
