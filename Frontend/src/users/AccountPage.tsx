import { Link, Navigate } from "react-router-dom";
import { ApiError } from "../api";
import { useAccessToken } from "../auth/tokenStorage";
import { useCurrentUser } from "../auth/useCurrentUser";

export function AccountPage() {
  const accessToken = useAccessToken();
  const currentUserQuery = useCurrentUser();

  if (accessToken === null) {
    return <Navigate to="/login" replace />;
  }

  if (currentUserQuery.isPending) {
    return (
      <main className="account-status-page">
        <p className="account-status-page__message">
          Loading account...
        </p>
      </main>
    );
  }

  const unauthorized =
    currentUserQuery.error instanceof ApiError &&
    currentUserQuery.error.status === 401;

  if (unauthorized) {
    return <Navigate to="/login" replace />;
  }

  if (currentUserQuery.isError) {
    return (
      <main className="account-status-page">
        <p className="form-message form-message--error">
          Could not load the account.
        </p>
      </main>
    );
  }

  const member = currentUserQuery.data;

  return (
    <main className="account-page">
      <section className="account-page__intro">
        <p className="section-banner">Account</p>

        <h1>
          Your
          <br />
          profile
        </h1>

        <p className="account-page__intro-text">
          View and manage the personal information linked to your
          account.
        </p>
      </section>

      <section className="account-page__content">
        <span className="account-page__connector" aria-hidden="true">
          <span className="account-page__connector-line" />
          <span className="account-page__connector-dot" />
        </span>

        <article className="account-card">
          <header className="account-card__header">
            <p className="account-card__eyebrow">
              {member.role}
            </p>

            <h2>{member.name}</h2>
          </header>

          <dl className="account-details">
            <div className="account-details__row">
              <dt>Name</dt>
              <dd>{member.name}</dd>
            </div>

            <div className="account-details__row">
              <dt>Email</dt>
              <dd>{member.email}</dd>
            </div>

            <div className="account-details__row">
              <dt>Role</dt>
              <dd>{member.role}</dd>
            </div>
          </dl>

          <div className="account-card__actions">
            <Link className="button" to="/account/edit">
              Edit account
            </Link>
          </div>
        </article>
      </section>
    </main>
  );
}