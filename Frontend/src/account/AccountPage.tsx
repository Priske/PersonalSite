import { Link } from "react-router-dom";
import { useCurrentUser } from "../auth/useCurrentUser";

export function AccountPage() {
  const currentUserQuery = useCurrentUser();

  if (!currentUserQuery.data) {
    return null;
  }

  const user = currentUserQuery.data;

  return (
    <article className="account-card">
      <header className="account-card__header">
        <p className="account-card__eyebrow">Profile information</p>

        <h2>{user.name}</h2>

        <p className="account-card__description">
          Review the personal information linked to your account.
        </p>
      </header>

      <dl className="account-details">
        <div className="account-details__row">
          <dt>Name</dt>
          <dd>{user.name}</dd>
        </div>

        <div className="account-details__row">
          <dt>Email</dt>
          <dd>{user.email}</dd>
        </div>

        <div className="account-details__row">
          <dt>Role</dt>
          <dd>{user.role}</dd>
        </div>
      </dl>

      <div className="account-card__actions">
        <Link className="button" to="/account/edit">
          Edit account
        </Link>
      </div>
    </article>
  );
}
