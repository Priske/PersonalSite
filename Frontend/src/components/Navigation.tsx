import { useQuery } from "@tanstack/react-query";
import { Link, NavLink } from "react-router-dom";
import { getCurrentUser } from "../auth/authApi";
import { LogoutButton } from "../auth/LogoutButton";
import { useAccessToken } from "../auth/tokenStorage";

export function Navigation() {
  const accessToken = useAccessToken();
  const hasAccessToken = accessToken !== null;

  const currentUserQuery = useQuery({
    queryKey: ["current-user"],
    queryFn: getCurrentUser,
    enabled: hasAccessToken,
    retry: false,
  });

  const navLinkClass = ({
    isActive,
  }: {
    isActive: boolean;
  }) =>
    isActive
      ? "site-nav__link site-nav__link--active"
      : "site-nav__link";

  const accountLinkClass = ({
    isActive,
  }: {
    isActive: boolean;
  }) =>
    isActive
      ? "site-nav__account site-nav__account--active"
      : "site-nav__account";

  const loggedIn =
    hasAccessToken && currentUserQuery.data !== undefined;

  return (
    <header className="site-header">
      <nav
        className="site-nav container"
        aria-label="Main navigation"
      >
        <Link className="site-nav__brand" to="/">
          <span className="site-nav__brand-name">
            Ben Eeckman
          </span>

          <span className="site-nav__brand-role">
            Junior Software Developer
          </span>
        </Link>

        <ul className="site-nav__links">
          <li>
            <a className="site-nav__link" href="/#about">
              About
            </a>
          </li>

          <li>
            <a className="site-nav__link" href="/#skills">
              Skills
            </a>
          </li>

          <li>
            <a className="site-nav__link" href="/#projects">
              Projects
            </a>
          </li>

          <li>
            <a className="site-nav__link" href="/#contact">
              Contact
            </a>
          </li>

          {loggedIn ? (
            <>
                <li>
                    <NavLink className={navLinkClass} to="/users">
                        Users
                    </NavLink>
                </li>

                <li>
                    <LogoutButton className="site-nav__link site-nav__logout" />
                </li>

                <li>
                    <NavLink className={accountLinkClass} to="/account">
                        Account
                    </NavLink>
                </li>
            </>
            ) : (
            <>
                <li>
                    <NavLink className={navLinkClass} to="/login">
                        Log in
                    </NavLink>
                </li>

                <li>
                    <NavLink
                        className={accountLinkClass} to="/register">
                            Register
                    </NavLink>
                </li>
            </>
            )}
        </ul>
      </nav>
    </header>
  );
}