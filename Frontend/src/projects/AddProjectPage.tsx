import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";

import { createProject } from "./projectsApi";
import type { CreateProjectRequest } from "./types";
import { normalizeUrl } from "./helper";

export function AddProjectPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [formError, setFormError] = useState<string | null>(null);

  const createMutation = useMutation({
    mutationFn: (request: CreateProjectRequest) => createProject(request),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["projects"],
      });

      navigate("/account/projects");
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setFormError(null);

    const formData = new FormData(event.currentTarget);

    const title = formData.get("title")?.toString().trim() ?? "";

    const description = formData.get("description")?.toString().trim() ?? "";

    const repositoryUrl = normalizeUrl(
      formData.get("repositoryUrl")?.toString() ?? "",
    );

    const liveUrl = normalizeUrl(formData.get("liveUrl")?.toString() ?? "");

    const isFeatured = formData.get("isFeatured") === "on";

    if (!title || !description || !repositoryUrl) {
      setFormError("Title, description and repository URL are required.");

      return;
    }

    createMutation.mutate({
      title,
      description,
      repositoryUrl,
      liveUrl: liveUrl || undefined,
      isFeatured,
      tagIds: [],
    });
  }

  return (
    <section className="manage-skill-group-page">
      <header className="manage-skill-group-page__header">
        <div>
          <p className="manage-skill-group-page__eyebrow">Projects</p>

          <h2>Add project</h2>

          <p>Add a new project to your portfolio.</p>
        </div>

        <Link className="button button--secondary" to="/account/projects">
          Back to projects
        </Link>
      </header>

      <form className="manage-skill-group-form" onSubmit={handleSubmit}>
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

              <input
                id="project-title"
                name="title"
                disabled={createMutation.isPending}
                required
              />
            </div>

            <div className="form-field">
              <label htmlFor="project-description">Description</label>

              <textarea
                id="project-description"
                name="description"
                disabled={createMutation.isPending}
                required
              />
            </div>

            <div className="form-field">
              <label htmlFor="project-repository-url">Repository URL</label>

              <input
                id="project-repository-url"
                name="repositoryUrl"
                type="text"
                placeholder="github.com/username/project"
                disabled={createMutation.isPending}
                required
              />
            </div>

            <div className="form-field">
              <label htmlFor="project-live-url">Live URL</label>

              <input
                id="project-live-url"
                name="liveUrl"
                type="text"
                placeholder="example.com/project"
                disabled={createMutation.isPending}
              />
            </div>

            <div className="form-field">
              <label>
                <input
                  name="isFeatured"
                  type="checkbox"
                  disabled={createMutation.isPending}
                />
                Featured project
              </label>
            </div>
          </div>
        </section>

        {formError && (
          <p className="form-message form-message--error">{formError}</p>
        )}

        {createMutation.isError && (
          <p className="form-message form-message--error">
            Could not create the project.
          </p>
        )}

        <div className="manage-skill-group-form__actions">
          <button
            className="button"
            type="submit"
            disabled={createMutation.isPending}
          >
            {createMutation.isPending ? "Creating..." : "Create project"}
          </button>

          <Link className="button button--secondary" to="/account/projects">
            Cancel
          </Link>
        </div>
      </form>
    </section>
  );
}
