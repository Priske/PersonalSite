import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState, type FormEvent } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";

import { ApiError } from "../api";
import { deleteProject, getProject, updateProject } from "./projectsApi";
import type { UpdateProjectRequest } from "./types";

function readProjectId(value: string | undefined) {
  const projectId = Number(value);
  return Number.isInteger(projectId) && projectId > 0 ? projectId : null;
}

export function ManageProjectPage() {
  const { projectId: projectIdParameter } = useParams();
  const projectId = readProjectId(projectIdParameter);
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [formError, setFormError] = useState<string | null>(null);

  const projectQuery = useQuery({
    queryKey: ["projects", "detail", projectId],
    queryFn: () => {
      if (projectId === null) throw new Error("Invalid project ID.");
      return getProject(projectId);
    },
    enabled: projectId !== null,
    retry: false,
  });

  const updateMutation = useMutation({
    mutationFn: (request: UpdateProjectRequest) => {
      if (projectId === null) throw new Error("Invalid project ID.");
      return updateProject(projectId, request);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["projects"] });
      navigate("/account/projects");
    },
  });

  const deleteMutation = useMutation({
    mutationFn: () => {
      if (projectId === null) throw new Error("Invalid project ID.");
      return deleteProject(projectId);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["projects"] });
      navigate("/account/projects");
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const formData = new FormData(event.currentTarget);
    const title = formData.get("title")?.toString().trim() ?? "";
    const description = formData.get("description")?.toString().trim() ?? "";
    const repositoryUrl = formData.get("repositoryUrl")?.toString().trim() ?? "";
    const liveUrl = formData.get("liveUrl")?.toString().trim() ?? "";
    const displayOrder = Number(formData.get("displayOrder"));
    const isFeatured = formData.get("isFeatured") === "on";

    if (!title || !description || !repositoryUrl) {
      setFormError("Title, description and repository URL are required.");
      return;
    }

    if (!Number.isInteger(displayOrder) || displayOrder < 0) {
      setFormError("Display order must be a valid number.");
      return;
    }

    updateMutation.mutate({
      title,
      description,
      repositoryUrl,
      liveUrl: liveUrl || undefined,
      isFeatured,
      displayOrder,
    });
  }

  if (projectId === null) {
    return (
      <section className="manage-skill-group-page">
        <div className="manage-skill-group-page__message">
          <p className="form-message form-message--error">Invalid project ID.</p>
          <Link className="button button--secondary" to="/account/projects">Back to projects</Link>
        </div>
      </section>
    );
  }

  if (projectQuery.isPending) {
    return (
      <section className="manage-skill-group-page">
        <div className="manage-skill-group-page__message">
          <p>Loading project...</p>
        </div>
      </section>
    );
  }

  const notFound = projectQuery.error instanceof ApiError && projectQuery.error.status === 404;

  if (notFound) {
    return (
      <section className="manage-skill-group-page">
        <div className="manage-skill-group-page__message">
          <p className="form-message form-message--error">Project not found.</p>
          <Link className="button button--secondary" to="/account/projects">Back to projects</Link>
        </div>
      </section>
    );
  }

  if (projectQuery.isError) {
    return (
      <section className="manage-skill-group-page">
        <div className="manage-skill-group-page__message">
          <p className="form-message form-message--error">Could not load the project.</p>
          <Link className="button button--secondary" to="/account/projects">Back to projects</Link>
        </div>
      </section>
    );
  }

  const project = projectQuery.data;
  const isSaving = updateMutation.isPending || deleteMutation.isPending;

  return (
    <section className="manage-skill-group-page">
      <header className="manage-skill-group-page__header">
        <div>
          <p className="manage-skill-group-page__eyebrow">Manage project</p>
          <h2>{project.title}</h2>
          <p>Update the project displayed on your portfolio.</p>
        </div>

        <Link className="button button--secondary" to="/account/projects">Back to projects</Link>
      </header>

      <form className="manage-skill-group-form" key={project.id} onSubmit={handleSubmit}>
        <section className="manage-skill-group-section">
          <header className="manage-skill-group-section__header">
            <div>
              <p className="manage-skill-group-page__eyebrow">Project</p>
              <h3>Project details</h3>
            </div>
          </header>

          <div className="manage-skill-group-form__fields">
            <div className="form-field">
              <label htmlFor="project-title">Title</label>
              <input id="project-title" name="title" defaultValue={project.title} disabled={isSaving} required />
            </div>

            <div className="form-field">
              <label htmlFor="project-description">Description</label>
              <textarea id="project-description" name="description" defaultValue={project.description} disabled={isSaving} required />
            </div>

            <div className="form-field">
              <label htmlFor="project-repository-url">Repository URL</label>
              <input id="project-repository-url" name="repositoryUrl" type="url" defaultValue={project.repositoryUrl} disabled={isSaving} required />
            </div>

            <div className="form-field">
              <label htmlFor="project-live-url">Live URL</label>
              <input id="project-live-url" name="liveUrl" type="url" defaultValue={project.liveUrl ?? ""} disabled={isSaving} />
            </div>

            <div className="form-field">
              <label htmlFor="project-display-order">Display order</label>
              <input id="project-display-order" name="displayOrder" type="number" min="0" defaultValue={project.displayOrder} disabled={isSaving} required />
            </div>

            <div className="form-field">
              <label>
                <input name="isFeatured" type="checkbox" defaultChecked={project.isFeatured} disabled={isSaving} />
                Featured project
              </label>
            </div>
          </div>
        </section>

        {formError && <p className="form-message form-message--error">{formError}</p>}
        {updateMutation.isError && <p className="form-message form-message--error">Could not update the project.</p>}
        {deleteMutation.isError && <p className="form-message form-message--error">Could not delete the project.</p>}

        <div className="manage-skill-group-form__actions">
          <button className="button" type="submit" disabled={isSaving}>
            {updateMutation.isPending ? "Saving..." : "Save project"}
          </button>

          <button
            className="button button--danger"
            type="button"
            disabled={isSaving}
            onClick={() => {
              if (window.confirm(`Delete "${project.title}"?`)) deleteMutation.mutate();
            }}
          >
            {deleteMutation.isPending ? "Deleting..." : "Delete project"}
          </button>
        </div>
      </form>
    </section>
  );
}