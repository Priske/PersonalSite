import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

import { useCurrentUser } from "../auth/useCurrentUser";
import { ProjectManagementItem } from "../projects/ProjectManagementitem";
import type { ProjectSummary } from "../projects/types";
import { useManageableProjects } from "../projects/useProjects";
import { useUpdateProjectsOrder } from "../projects/useUpdateProjectsOrder";

export function AccountProjectsPage() {
  const currentUserQuery = useCurrentUser();

  const isAdministrator = currentUserQuery.data?.role === "Administrator";

  const projectQuery = useManageableProjects(isAdministrator);

  const updateOrderMutation = useUpdateProjectsOrder();

  const [projects, setProjects] = useState<ProjectSummary[]>([]);

  useEffect(() => {
    if (!projectQuery.data) {
      return;
    }

    setProjects(
      [...projectQuery.data.items].sort(
        (a, b) => a.displayOrder - b.displayOrder,
      ),
    );
  }, [projectQuery.data]);

  async function moveProject(currentIndex: number, direction: -1 | 1) {
    const targetIndex = currentIndex + direction;

    if (targetIndex < 0 || targetIndex >= projects.length) {
      return;
    }

    const previousProjects = projects;

    const reorderedProjects = [...projects];

    [reorderedProjects[currentIndex], reorderedProjects[targetIndex]] = [
      reorderedProjects[targetIndex],
      reorderedProjects[currentIndex],
    ];

    setProjects(reorderedProjects);

    try {
      await updateOrderMutation.mutateAsync({
        projectIds: reorderedProjects.map((project) => project.id),
      });
    } catch {
      setProjects(previousProjects);
    }
  }

  return (
    <article className="account-card">
      <header className="account-card__header account-management__header">
        <div>
          <p className="account-card__eyebrow">
            {isAdministrator
              ? "Official website content"
              : "Demo website content"}
          </p>

          <h2>Projects</h2>

          <p className="account-management__description">
            {isAdministrator
              ? "Manage the official projects displayed on the public homepage."
              : "Manage the demo projects displayed in your personal preview."}
          </p>
        </div>

        <Link className="button" to="/account/projects/new">
          Add project
        </Link>
      </header>

      <div className="account-management__body">
        <div className="account-management__empty">
          {projectQuery.isPending && (
            <p className="account-management__status">Loading projects...</p>
          )}

          {projectQuery.isError && (
            <p className="form-message form-message--error">
              Could not load projects.
            </p>
          )}

          {updateOrderMutation.isError && (
            <p className="form-message form-message--error">
              Could not save the project order.
            </p>
          )}

          {projectQuery.isSuccess && projects.length === 0 && (
            <div className="account-management__empty">
              <p className="account-management__empty-title">No projects yet</p>

              <p>
                {isAdministrator
                  ? "Add a project to the official portfolio."
                  : "Add a project to your demo portfolio."}
              </p>
            </div>
          )}

          {projectQuery.isSuccess && projects.length > 0 && (
            <div className="project-management-list">
              {projects.map((project, index) => (
                <ProjectManagementItem
                  key={project.id}
                  project={project}
                  index={index}
                  projectCount={projects.length}
                  isSaving={updateOrderMutation.isPending}
                  onMove={moveProject}
                />
              ))}
            </div>
          )}
        </div>
      </div>
    </article>
  );
}
