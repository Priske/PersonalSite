import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { useState, type FormEvent } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { getTag, getTags, updateTag } from "./tagsApi";
import { DeleteTagButton } from "./DeleteTagButton";

const pageSize = 25;

function readPage(value: string | null) {
  const page = Number(value);

  return Number.isInteger(page) && page > 0 ? page : 1;
}

export function TagsPage() {
  const queryClient = useQueryClient();

  const [searchParams, setSearchParams] = useSearchParams();

  const [selectedTagId, setSelectedTagId] = useState<number | null>(null);

  const page = readPage(searchParams.get("page"));

  const search = searchParams.get("search")?.trim() ?? "";

  const tagsQuery = useQuery({
    queryKey: [
      "tags",
      {
        page,
        pageSize,
        search,
      },
    ],

    queryFn: () =>
      getTags({
        page,
        pageSize,
        search,
      }),

    placeholderData: keepPreviousData,
  });

  const selectedTagQuery = useQuery({
    queryKey: ["tags", "detail", selectedTagId],

    queryFn: () => {
      if (selectedTagId === null) {
        throw new Error("No tag selected");
      }

      return getTag(selectedTagId);
    },

    enabled: selectedTagId !== null,
    retry: false,
  });

  const updateTagMutation = useMutation({
    mutationFn: ({ id, name }: { id: number; name: string }) =>
      updateTag(id, {
        id,
        name,
      }),

    onSuccess: async (_, variables) => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: ["tags"],
        }),
        queryClient.invalidateQueries({
          queryKey: ["tags", "detail", variables.id],
        }),
      ]);
    },
  });

  function setPage(nextPage: number) {
    const next = new URLSearchParams(searchParams);

    if (nextPage === 1) {
      next.delete("page");
    } else {
      next.set("page", nextPage.toString());
    }

    setSelectedTagId(null);
    setSearchParams(next);
  }

  function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);

    const value = formData.get("search")?.toString().trim() ?? "";

    const next = new URLSearchParams();

    if (value) {
      next.set("search", value);
    }

    setSelectedTagId(null);
    setSearchParams(next);
  }

  function handleToggleTag(tagId: number) {
    setSelectedTagId((currentTagId) => (currentTagId === tagId ? null : tagId));

    updateTagMutation.reset();
  }

  function handleRename(event: FormEvent<HTMLFormElement>, tagId: number) {
    event.preventDefault();

    const formData = new FormData(event.currentTarget);

    const name = formData.get("name")?.toString().trim() ?? "";

    if (!name) {
      return;
    }

    updateTagMutation.mutate({
      id: tagId,
      name,
    });
  }

  function handleTagDeleted() {
    setSelectedTagId(null);
  }

  return (
    <main className="tags-list-page">
      <article className="tag-list-card">
        <header className="tag-list-card__header">
          <div>
            <p className="tag-list-card__eyebrow">Tags</p>

            <h1>Tag List</h1>
          </div>

          {!tagsQuery.isPending && !tagsQuery.isError && (
            <p className="tag-list-card__count">
              {tagsQuery.data.totalItems} total
            </p>
          )}
        </header>

        <div className="tag-list-card__body">
          <form className="tag-search" key={search} onSubmit={handleSearch}>
            <div className="tag-search__field">
              <label htmlFor="tag-search">Search by name</label>

              <input
                id="tag-search"
                type="search"
                name="search"
                defaultValue={search}
                placeholder="Enter a tag name"
              />
            </div>

            <button
              className="button"
              type="submit"
              disabled={tagsQuery.isFetching}
            >
              Search
            </button>
          </form>

          {search && (
            <div className="tag-list-card__filter">
              <span>
                Results for <strong>{search}</strong>
              </span>

              <Link
                className="tag-list-card__clear"
                to="/account/tags"
                onClick={() => setSelectedTagId(null)}
              >
                Clear search
              </Link>
            </div>
          )}

          {tagsQuery.isPending && (
            <p className="form-message" role="status">
              Loading tags...
            </p>
          )}

          {tagsQuery.isError && (
            <p className="form-message form-message--error" role="alert">
              Could not load the tags. The server may be unavailable.
            </p>
          )}

          {!tagsQuery.isPending && !tagsQuery.isError && (
            <>
              {tagsQuery.data.items.length === 0 ? (
                <div className="tag-list-empty">
                  <h2>No tags found</h2>

                  <p>Try another name.</p>
                </div>
              ) : (
                <ul className="tag-list">
                  {tagsQuery.data.items.map((tag) => {
                    const isSelected = selectedTagId === tag.id;

                    const details =
                      isSelected && selectedTagQuery.data?.id === tag.id
                        ? selectedTagQuery.data
                        : null;

                    const canDelete =
                      details !== null && details.projects.length === 0;

                    return (
                      <li
                        className={`tag-list__item${
                          isSelected ? " tag-list__item--expanded" : ""
                        }`}
                        key={tag.id}
                      >
                        <button
                          className="tag-list__toggle"
                          type="button"
                          onClick={() => handleToggleTag(tag.id)}
                          aria-expanded={isSelected}
                          aria-controls={`tag-details-${tag.id}`}
                        >
                          <span className="tag-list__identity">
                            <strong className="tag-list__name">
                              {tag.name}
                            </strong>

                            <span className="tag-list__id">ID {tag.id}</span>
                          </span>

                          <span className="tag-list__arrow" aria-hidden="true">
                            {isSelected ? "−" : "+"}
                          </span>
                        </button>

                        {isSelected && (
                          <div
                            className="tag-list__details"
                            id={`tag-details-${tag.id}`}
                          >
                            {selectedTagQuery.isPending && (
                              <p className="form-message" role="status">
                                Loading tag details...
                              </p>
                            )}

                            {selectedTagQuery.isError && (
                              <p
                                className="form-message form-message--error"
                                role="alert"
                              >
                                Could not load the tag details.
                              </p>
                            )}

                            {details && (
                              <>
                                <section className="tag-details__section">
                                  <div className="tag-details__heading">
                                    <p className="tag-details__eyebrow">
                                      Rename
                                    </p>

                                    <h2>Change tag name</h2>
                                  </div>

                                  <form
                                    className="tag-rename-form"
                                    key={details.name}
                                    onSubmit={(event) =>
                                      handleRename(event, details.id)
                                    }
                                  >
                                    <div className="tag-rename-form__field">
                                      <label htmlFor={`tag-name-${details.id}`}>
                                        Name
                                      </label>

                                      <input
                                        id={`tag-name-${details.id}`}
                                        name="name"
                                        type="text"
                                        defaultValue={details.name}
                                        maxLength={100}
                                        required
                                        disabled={updateTagMutation.isPending}
                                      />
                                    </div>

                                    <button
                                      className="button"
                                      type="submit"
                                      disabled={updateTagMutation.isPending}
                                    >
                                      {updateTagMutation.isPending
                                        ? "Saving..."
                                        : "Save name"}
                                    </button>
                                  </form>

                                  {updateTagMutation.isSuccess && (
                                    <p
                                      className="form-message form-message--success"
                                      role="status"
                                    >
                                      Tag name updated.
                                    </p>
                                  )}

                                  {updateTagMutation.isError && (
                                    <p
                                      className="form-message form-message--error"
                                      role="alert"
                                    >
                                      Could not update the tag.
                                    </p>
                                  )}
                                </section>

                                <section className="tag-details__section">
                                  <div className="tag-details__heading">
                                    <p className="tag-details__eyebrow">
                                      Usage
                                    </p>

                                    <h2>Used by projects</h2>
                                  </div>

                                  {details.projects.length === 0 ? (
                                    <div className="tag-details__empty">
                                      <p>
                                        This tag is not used by any projects.
                                      </p>
                                    </div>
                                  ) : (
                                    <ul className="tag-project-list">
                                      {details.projects.map((project) => (
                                        <li
                                          className="tag-project-list__item"
                                          key={project.id}
                                        >
                                          <span className="tag-project-list__title">
                                            {project.title}
                                          </span>

                                          <span className="tag-project-list__id">
                                            ID {project.id}
                                          </span>
                                        </li>
                                      ))}
                                    </ul>
                                  )}
                                </section>

                                <section className="tag-details__section tag-details__section--danger">
                                  <div className="tag-details__heading">
                                    <p className="tag-details__eyebrow">
                                      Danger zone
                                    </p>

                                    <h2>Delete tag</h2>
                                  </div>

                                  {canDelete ? (
                                    <p className="tag-details__description">
                                      Permanently delete this tag. This action
                                      cannot be undone.
                                    </p>
                                  ) : (
                                    <p className="tag-details__description">
                                      This tag cannot be deleted while it is
                                      used by a project.
                                    </p>
                                  )}

                                  <DeleteTagButton
                                    tagId={details.id}
                                    tagName={details.name}
                                    disabled={!canDelete}
                                    onDeleted={handleTagDeleted}
                                  />
                                </section>
                              </>
                            )}
                          </div>
                        )}
                      </li>
                    );
                  })}
                </ul>
              )}

              <footer className="tag-list-card__footer">
                <p className="tag-list-card__summary">
                  Page {tagsQuery.data.page} of{" "}
                  {Math.max(tagsQuery.data.totalPages, 1)}.{" "}
                  {tagsQuery.data.totalItems} tags found.
                </p>

                <div className="tag-list-pagination">
                  <button
                    className="button button--secondary"
                    type="button"
                    onClick={() => setPage(tagsQuery.data.page - 1)}
                    disabled={tagsQuery.data.page <= 1 || tagsQuery.isFetching}
                  >
                    Previous
                  </button>

                  <button
                    className="button"
                    type="button"
                    onClick={() => setPage(tagsQuery.data.page + 1)}
                    disabled={
                      tagsQuery.data.page >= tagsQuery.data.totalPages ||
                      tagsQuery.isFetching
                    }
                  >
                    Next
                  </button>
                </div>
              </footer>

              {tagsQuery.isFetching && (
                <p className="tag-list-card__updating" role="status">
                  Updating tags...
                </p>
              )}
            </>
          )}
        </div>
      </article>
    </main>
  );
}
