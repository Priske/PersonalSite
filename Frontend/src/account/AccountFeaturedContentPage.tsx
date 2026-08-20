import { Link } from "react-router-dom";
import { useFeaturedContent } from "../featured/useFeaturedContent";

export function AccountFeaturedContentPage() {
  const contentQuery = useFeaturedContent();

  return (
    <article className="account-card">
      <header className="account-card__header account-management__header">
        <div>
          <p className="account-card__eyebrow">Official website content</p>
          <h2>Featured content</h2>
          <p className="account-management__description">
            Manage the highlighted content and its attached media.
          </p>
        </div>

        <Link className="button" to="/account/featured-content/new">
          Add content
        </Link>
      </header>

      <div className="account-management__body">
        {contentQuery.isPending && (
          <p className="account-management__status">Loading content...</p>
        )}

        {contentQuery.isError && (
          <p className="form-message form-message--error">
            Could not load featured content.
          </p>
        )}

        {contentQuery.isSuccess && contentQuery.data.items.length === 0 && (
          <div className="account-management__empty">
            <p className="account-management__empty-title">No content yet</p>
            <p>Create the first item before uploading its media.</p>
          </div>
        )}

        {contentQuery.isSuccess && contentQuery.data.items.length > 0 && (
          <div className="featured-management-list">
            {contentQuery.data.items.map((item) => (
              <section className="featured-management-item" key={item.id}>
                <header className="featured-management-item__header">
                  <div>
                    <p>Featured content {item.id}</p>
                    <h3>{item.title}</h3>
                  </div>

                  <Link
                    className="button button--secondary"
                    to={`/account/featured-content/${item.id}/edit`}
                  >
                    Edit content
                  </Link>
                </header>

                <p className="featured-management-item__description">
                  {item.description}
                </p>

                <dl className="featured-management-item__details">
                  <div>
                    <dt>Files</dt>
                    <dd>{item.files.length}</dd>
                  </div>
                  <div>
                    <dt>Tags</dt>
                    <dd>{item.tags.length > 0 ? item.tags.join(", ") : "None"}</dd>
                  </div>
                </dl>
              </section>
            ))}
          </div>
        )}
      </div>
    </article>
  );
}
