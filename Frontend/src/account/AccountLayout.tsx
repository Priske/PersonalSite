import { Navigate, Outlet } from "react-router-dom";
import { ApiError } from "../api";
import { useAccessToken } from "../auth/tokenStorage";
import { useCurrentUser } from "../auth/useCurrentUser";
import { AccountNavigation } from "./AccountNavigation";

export function AccountLayout() {
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

  return (
    <main className="account-page">
      <AccountNavigation user={currentUserQuery.data} />

      <section className="account-page__content">
        <span
          className="account-page__connector"
          aria-hidden="true"
        >
          <span className="account-page__connector-line" />
          <span className="account-page__connector-dot" />
        </span>

        <div className="account-page__outlet">
          <Outlet />
        </div>
      </section>
    </main>
  );
}