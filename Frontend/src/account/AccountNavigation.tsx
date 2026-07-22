import { NavLink } from "react-router-dom";
import type { CurrentUser } from "../auth/types";

type AccountNavigationProps = {
  user: CurrentUser;
};

export function AccountNavigation({
  user,
}: AccountNavigationProps) {
  const isAdministrator =
    user.role === "Administrator";

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
          Manage your profile and the content displayed on your
          personal website.
        </p>
      </div>

      <nav
        className="account-navigation__links"
        aria-label="Account navigation"
      >
        <NavLink
          className={({ isActive }) =>
            isActive
              ? "account-navigation__link account-navigation__link--active"
              : "account-navigation__link"
          }
          end
          to="/account"
        >
          <span className="account-navigation__number">01</span>
          <span>Profile</span>
        </NavLink>

        {isAdministrator && (
          <>
            <NavLink
              className={({ isActive }) =>
                isActive
                  ? "account-navigation__link account-navigation__link--active"
                  : "account-navigation__link"
              }
              to="/account/skills"
            >
              <span className="account-navigation__number">02</span>
              <span>Skills</span>
            </NavLink>

            <NavLink
              className={({ isActive }) =>
                isActive
                  ? "account-navigation__link account-navigation__link--active"
                  : "account-navigation__link"
              }
              to="/account/projects"
            >
              <span className="account-navigation__number">03</span>
              <span>Projects</span>
            </NavLink>
          </>
        )}
      </nav>

      <div className="account-navigation__user">
        <span>Signed in as</span>
        <strong>{user.name}</strong>
        <span>{user.role}</span>
      </div>
    </aside>
  );
}