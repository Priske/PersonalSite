import { Link } from "react-router-dom";
import type { ProjectSummary } from "./types";

type ProjectManagementItemProps = {
  project: ProjectSummary;
  index: number;
  projectCount: number;
  isSaving: boolean;
  onMove: (currentIndex: number, direction: -1 | 1) => Promise<void>;
};

export function ProjectManagementItem({
  project,
  index,
  projectCount,
  isSaving,
  onMove,
}: ProjectManagementItemProps) {
  return (
    <section className="project-management-item">
      <header className="project-management-item__header">
        <div>
          <p className="project-management-item__order">
            Project {String(index + 1).padStart(2, "0")}
          </p>

          <h3>{project.title}</h3>
        </div>

        <div className="project-management-item__actions">
          <button
            className="button button--secondary"
            type="button"
            aria-label={`Move ${project.title} up`}
            title="Move up"
            onClick={() => void onMove(index, -1)}
            disabled={isSaving || index === 0}
          >
            ↑
          </button>

          <button
            className="button button--secondary"
            type="button"
            aria-label={`Move ${project.title} down`}
            title="Move down"
            onClick={() => void onMove(index, 1)}
            disabled={isSaving || index === projectCount - 1}
          >
            ↓
          </button>

          <Link
            className="button button--secondary"
            to={`/account/projects/${project.id}/edit`}
          >
            Edit project
          </Link>
        </div>
      </header>

      <div className="project-management-item__content">
        <p>{project.description}</p>

        <dl className="project-management-item__details">
          <div>
            <dt>Featured</dt>
            <dd>{project.isFeatured ? "Yes" : "No"}</dd>
          </div>

          <div>
            <dt>Display order</dt>
            <dd>{project.displayOrder}</dd>
          </div>

          <div>
            <dt>Repository</dt>
            <dd>
              {project.repositoryUrl ? (
                <a
                  href={project.repositoryUrl}
                  target="_blank"
                  rel="noreferrer"
                >
                  View repository
                </a>
              ) : (
                "Not provided"
              )}
            </dd>
          </div>

          <div>
            <dt>Live site</dt>
            <dd>
              {project.liveUrl ? (
                <a href={project.liveUrl} target="_blank" rel="noreferrer">
                  View project
                </a>
              ) : (
                "Not provided"
              )}
            </dd>
          </div>
          <div>
            <dt>Tags</dt>
            <dd>
              {project.tags.length > 0 && (
                <ul className="project-management-item__tags">
                  {project.tags.map((tag) => (
                    <li key={tag}>{tag}</li>
                  ))}
                </ul>
              )}
            </dd>
          </div>
        </dl>
      </div>
    </section>
  );
}
