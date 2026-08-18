import { useLoginAnalytics } from "../analytics/useLoginAnalytics";
import { useReferrerAnalytics } from "../analytics/useReferrerAnalytics";

export function AnalyticsAdminPage() {
  const loginAnalyticsQuery = useLoginAnalytics({
    page: 1,
    pageSize: 20,
  });

  const referrerAnalyticsQuery = useReferrerAnalytics({
    sortBy: "count",
    descending: true,
  });

  if (loginAnalyticsQuery.isPending || referrerAnalyticsQuery.isPending) {
    return (
      <section className="account-card">
        <div className="account-management__body">
          <p>Loading analytics...</p>
        </div>
      </section>
    );
  }

  if (loginAnalyticsQuery.isError || referrerAnalyticsQuery.isError) {
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

  return (
    <section className="account-card analytics">
      <header className="account-card__header">
        <p className="account-card__eyebrow">Analytics</p>

        <h2>Site Activity</h2>

        <p className="account-management__description">
          Monitor authentication activity, user registrations and website usage.
        </p>
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
      </div>

      <div className="analytics-section analytics-section--referrers">
        <div className="analytics-section__header">
          <div>
            <p className="account-card__eyebrow">Traffic</p>
            <h3>Referrers</h3>
          </div>

          <span>{referrerAnalytics.totalPageViews} page views</span>
        </div>

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
    </section>
  );
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
