import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import type { FormEvent } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { getUsers, seedFakeUsers } from "./usersApi";

const pageSize = 10;

function readPage(value: string | null) {
  const page = Number(value);

  return Number.isInteger(page) && page > 0 ? page : 1;
}

export function UserListPage() {
  const [searchParams, setSearchParams] = useSearchParams();

  const page = readPage(searchParams.get("page"));
  const search = searchParams.get("search")?.trim() ?? "";

  const usersQuery = useQuery({
    queryKey: [
      "users",
      {
        page,
        pageSize,
        search,
      },
    ],
    queryFn: () =>
      getUsers({
        page,
        pageSize,
        search,
      }),
    placeholderData: keepPreviousData,
  });

  function setPage(nextPage: number) {
    const next = new URLSearchParams(searchParams);

    if (nextPage === 1) {
      next.delete("page");
    } else {
      next.set("page", nextPage.toString());
    }

    setSearchParams(next);
  }
  const queryClient = useQueryClient();

  const seedFakeUsersMutation = useMutation({
    mutationFn: seedFakeUsers,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["users"],
      });
    },
  });

  function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);

    const value = formData.get("search")?.toString().trim() ?? "";

    const next = new URLSearchParams();

    if (value) {
      next.set("search", value);
    }

    setSearchParams(next);
  }

  return (
    <main className="user-list-page">
      <section className="user-list-page__intro">
        <p className="section-banner">Administration</p>

        <h1>
          User
          <br />
          directory
        </h1>

        <p className="user-list-page__intro-text">
          Search, review and manage registered user accounts.
        </p>
      </section>

      <section className="user-list-page__content">
        <span className="user-list-page__connector" aria-hidden="true">
          <span className="user-list-page__connector-line" />
          <span className="user-list-page__connector-dot" />
        </span>

        <article className="user-list-card">
          <header className="user-list-card__header">
            <div>
              <p className="user-list-card__eyebrow">Users</p>

              <h2>User List</h2>
            </div>

            {!usersQuery.isPending && !usersQuery.isError && (
              <p className="user-list-card__count">
                {usersQuery.data.totalItems} total
              </p>
            )}
          </header>

          <div className="user-list-card__body">
            <form className="user-search" key={search} onSubmit={handleSearch}>
              <div className="user-search__field">
                <label htmlFor="user-search">Search by name or email</label>

                <input
                  id="user-search"
                  type="search"
                  name="search"
                  defaultValue={search}
                  placeholder="Enter a name or email"
                />
              </div>

              <button
                className="button"
                type="submit"
                disabled={usersQuery.isFetching}
              >
                Search
              </button>
            </form>

            {search && (
              <div className="user-list-card__filter">
                <span>
                  Results for <strong>{search}</strong>
                </span>

                <Link className="user-list-card__clear" to="/users">
                  Clear search
                </Link>
              </div>
            )}

            {usersQuery.isPending && (
              <p className="form-message" role="status">
                Loading users...
              </p>
            )}

            {usersQuery.isError && (
              <p className="form-message form-message--error" role="alert">
                Could not load the users. The server may be unavailable.
              </p>
            )}

            {!usersQuery.isPending && !usersQuery.isError && (
              <>
                {usersQuery.data.items.length === 0 ? (
                  <div className="user-list-empty">
                    {!search && (
                      <button
                        className="button"
                        type="button"
                        onClick={() => seedFakeUsersMutation.mutate()}
                        disabled={seedFakeUsersMutation.isPending}
                      >
                        {seedFakeUsersMutation.isPending
                          ? "Creating demo users..."
                          : "Create demo users"}
                      </button>
                    )}
                    {seedFakeUsersMutation.isError && (
                      <p
                        className="form-message form-message--error"
                        role="alert"
                      >
                        Could not create demo users.
                      </p>
                    )}
                    <h3>No users found</h3>
                    {search && <p>Try another name or email address.</p>}
                  </div>
                ) : (
                  <ul className="user-list">
                    {usersQuery.data.items.map((user) => (
                      <li className="user-list__item" key={user.id}>
                        <Link
                          className="user-list__link"
                          to={`/users/${user.id}/edit`}
                          state={{
                            fromUserList: true,
                          }}
                        >
                          <span className="user-list__identity">
                            <strong>{user.name}</strong>
                            <span>{user.email}</span>
                          </span>

                          <span className="user-list__meta">
                            <span>ID {user.id}</span>

                            <span
                              className="user-list__arrow"
                              aria-hidden="true"
                            >
                              →
                            </span>
                          </span>
                        </Link>
                      </li>
                    ))}
                  </ul>
                )}

                <footer className="user-list-card__footer">
                  <p className="user-list-card__summary">
                    Page {usersQuery.data.page} of{" "}
                    {Math.max(usersQuery.data.totalPages, 1)}.{" "}
                    {usersQuery.data.totalItems} users found.
                  </p>

                  <div className="user-list-pagination">
                    <button
                      className="button button--secondary"
                      type="button"
                      onClick={() => setPage(usersQuery.data.page - 1)}
                      disabled={
                        usersQuery.data.page <= 1 || usersQuery.isFetching
                      }
                    >
                      Previous
                    </button>

                    <button
                      className="button"
                      type="button"
                      onClick={() => setPage(usersQuery.data.page + 1)}
                      disabled={
                        usersQuery.data.page >= usersQuery.data.totalPages ||
                        usersQuery.isFetching
                      }
                    >
                      Next
                    </button>
                  </div>
                </footer>

                {usersQuery.isFetching && (
                  <p className="user-list-card__updating" role="status">
                    Updating users...
                  </p>
                )}
              </>
            )}
          </div>
        </article>
      </section>
    </main>
  );
}
