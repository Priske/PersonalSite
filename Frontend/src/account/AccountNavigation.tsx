import { NavLink } from "react-router-dom";
import type { CurrentUser } from "../auth/types";

type AccountNavigationProps = {
  user: CurrentUser;
};

function navigationClass({ isActive }: { isActive: boolean }) {
  return isActive
    ? "account-navigation__link account-navigation__link--active"
    : "account-navigation__link";
}

export function AccountNavigation({ user }: AccountNavigationProps) {
  const isAdministrator = user.role === "Administrator";

  return (
    <aside className="account-navigation">
      <div className="account-navigation__heading">
        <p className="section-banner">Account</p>

        <h1>
          Manage
          <br />
          content
        </h1>

        <p className="account-navigation__intro">
          Manage your profile and the content displayed on your personal
          website.
        </p>
      </div>

      <nav
        className="account-navigation__links"
        aria-label="Account navigation"
      >
        <NavLink className={navigationClass} end to="/account">
          <span className="account-navigation__number">01</span>

          <span>Profile</span>
        </NavLink>

        {isAdministrator && (
          <NavLink className={navigationClass} to="/account/skills">
            <span className="account-navigation__number">02</span>

            <span>Skills</span>
          </NavLink>
        )}

        <NavLink className={navigationClass} to="/account/projects">
          <span className="account-navigation__number">
            {isAdministrator ? "03" : "02"}
          </span>

          <span>Projects</span>
        </NavLink>

        <NavLink className={navigationClass} to="/account/tags">
          <span className="account-navigation__number">
            {isAdministrator ? "04" : "03"}
          </span>

          <span>Tags</span>
        </NavLink>

        <NavLink
          className={navigationClass}
          to={
            isAdministrator
              ? "/account/homePage"
              : "/account/demo-home-page-edit"
          }
        >
          <span className="account-navigation__number">
            {isAdministrator ? "05" : "04"}
          </span>

          <span>Home Page</span>
        </NavLink>
      </nav>

      <div className="account-navigation__user">
        <span>Signed in as</span>

        <strong>{user.name}</strong>

        <span>{user.role}</span>
      </div>
    </aside>
  );
}
