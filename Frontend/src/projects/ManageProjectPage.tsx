import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
} from "@tanstack/react-query";
import { useEffect, useState, type FormEvent } from "react";
import {
  Link,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";

import { ApiError } from "../api";
import { createTag, getTags } from "../tags/tagsApi";
import type { TagSummary } from "../tags/types";
import { deleteProject, getProject, updateProject } from "./projectsApi";
import type { UpdateProjectRequest } from "./types";

function readProjectId(value: string | undefined) {
  const projectId = Number(value);

  return Number.isInteger(projectId) && projectId > 0 ? projectId : null;
}

const pageSize = 100;

function readPage(value: string | null) {
  const page = Number(value);

  return Number.isInteger(page) && page > 0 ? page : 1;
}

export function ManageProjectPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const { projectId: projectIdParameter } = useParams();

  const projectId = readProjectId(projectIdParameter);
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const page = readPage(searchParams.get("page"));
  const search = searchParams.get("search")?.trim() ?? "";

  const [formError, setFormError] = useState<string | null>(null);
  const [selectedTagId, setSelectedTagId] = useState("");
  const [projectTags, setProjectTags] = useState<TagSummary[]>([]);
  const [newTagName, setNewTagName] = useState("");

  const projectQuery = useQuery({
    queryKey: ["projects", "detail", projectId],
    queryFn: () => {
      if (projectId === null) {
        throw new Error("Invalid project ID.");
      }

      return getProject(projectId);
    },
    enabled: projectId !== null,
    retry: false,
  });

  const availableTagsQuery = useQuery({
    queryKey: ["tags", { page, pageSize, search }],
    queryFn: () =>
      getTags({
        page,
        pageSize,
        search,
      }),
    placeholderData: keepPreviousData,
  });

  useEffect(() => {
    if (!projectQuery.data) {
      return;
    }

    setProjectTags(projectQuery.data.tags);
  }, [projectQuery.data]);

  const updateMutation = useMutation({
    mutationFn: (request: UpdateProjectRequest) => {
      if (projectId === null) {
        throw new Error("Invalid project ID.");
      }

      return updateProject(projectId, request);
    },

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["projects"],
      });

      navigate("/account/projects");
    },
  });

  const deleteMutation = useMutation({
    mutationFn: () => {
      if (projectId === null) {
        throw new Error("Invalid project ID.");
      }

      return deleteProject(projectId);
    },

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["projects"],
      });

      navigate("/account/projects");
    },
  });

  const createTagMutation = useMutation({
    mutationFn: createTag,

    onSuccess: async (createdTag) => {
      setProjectTags((currentTags) => {
        if (currentTags.some((tag) => tag.id === createdTag.id)) {
          return currentTags;
        }

        return [...currentTags, createdTag].sort((first, second) =>
          first.name.localeCompare(second.name),
        );
      });

      setNewTagName("");

      await queryClient.invalidateQueries({
        queryKey: ["tags"],
      });
    },

    onError: () => {
      setFormError("Could not create the tag.");
    },
  });

  function handleAddTag() {
    setFormError(null);

    if (!selectedTagId || !availableTagsQuery.data) {
      return;
    }

    const tagId = Number(selectedTagId);

    const selectedTag = availableTagsQuery.data.items.find(
      (tag) => tag.id === tagId,
    );

    if (!selectedTag) {
      setFormError("The selected tag could not be found.");
      return;
    }

    setProjectTags((currentTags) => {
      if (currentTags.some((tag) => tag.id === selectedTag.id)) {
        return currentTags;
      }

      return [...currentTags, selectedTag].sort((first, second) =>
        first.name.localeCompare(second.name),
      );
    });

    setSelectedTagId("");
  }

  function handleRemoveTag(tagId: number) {
    console.log("Removing tag ID:", tagId);
    console.log("Current tags:", projectTags);
    setProjectTags((currentTags) =>
      currentTags.filter((tag) => tag.id !== tagId),
    );
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const formData = new FormData(event.currentTarget);

    const title = formData.get("title")?.toString().trim() ?? "";

    const description = formData.get("description")?.toString().trim() ?? "";

    const repositoryUrl =
      formData.get("repositoryUrl")?.toString().trim() ?? "";

    const liveUrl = formData.get("liveUrl")?.toString().trim() ?? "";

    const isFeatured = formData.get("isFeatured") === "on";

    if (!title || !description || !repositoryUrl) {
      setFormError("Title, description and repository URL are required.");
      return;
    }

    updateMutation.mutate({
      title,
      description,
      repositoryUrl,
      liveUrl: liveUrl || undefined,
      isFeatured,
      tagIds: projectTags.map((tag) => tag.id),
    });
  }

  function handleCreateTag() {
    setFormError(null);

    const name = newTagName.trim();

    if (!name) {
      setFormError("Tag name is required.");
      return;
    }

    const existingTag = availableTags.find(
      (tag) => tag.name.toLowerCase() === name.toLowerCase(),
    );

    if (existingTag) {
      setProjectTags((currentTags) => {
        if (currentTags.some((tag) => tag.id === existingTag.id)) {
          return currentTags;
        }

        return [...currentTags, existingTag].sort((first, second) =>
          first.name.localeCompare(second.name),
        );
      });

      setNewTagName("");
      return;
    }

    createTagMutation.mutate({ name });
  }

  if (projectId === null) {
    return (
      <section className="manage-skill-group-page">
        <div className="manage-skill-group-page__message">
          <p className="form-message form-message--error">
            Invalid project ID.
          </p>

          <Link className="button button--secondary" to="/account/projects">
            Back to projects
          </Link>
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

  const notFound =
    projectQuery.error instanceof ApiError && projectQuery.error.status === 404;

  if (notFound) {
    return (
      <section className="manage-skill-group-page">
        <div className="manage-skill-group-page__message">
          <p className="form-message form-message--error">Project not found.</p>

          <Link className="button button--secondary" to="/account/projects">
            Back to projects
          </Link>
        </div>
      </section>
    );
  }

  if (projectQuery.isError) {
    return (
      <section className="manage-skill-group-page">
        <div className="manage-skill-group-page__message">
          <p className="form-message form-message--error">
            Could not load the project.
          </p>

          <Link className="button button--secondary" to="/account/projects">
            Back to projects
          </Link>
        </div>
      </section>
    );
  }

  const project = projectQuery.data;

  const availableTags = availableTagsQuery.data?.items ?? [];

  const unattachedTags = availableTags.filter(
    (availableTag) =>
      !projectTags.some((projectTag) => projectTag.id === availableTag.id),
  );

  const isSaving =
    updateMutation.isPending ||
    deleteMutation.isPending ||
    createTagMutation.isPending;

  return (
    <section className="manage-skill-group-page">
      <header className="manage-skill-group-page__header">
        <div>
          <p className="manage-skill-group-page__eyebrow">Manage project</p>

          <h2>{project.title}</h2>

          <p>Update the project displayed on your portfolio.</p>
        </div>

        <Link className="button button--secondary" to="/account/projects">
          Back to projects
        </Link>
      </header>

      <form
        className="manage-skill-group-form"
        key={project.id}
        onSubmit={handleSubmit}
      >
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
                defaultValue={project.title}
                disabled={isSaving}
                required
              />
            </div>

            <div className="form-field">
              <label htmlFor="project-description">Description</label>

              <textarea
                id="project-description"
                name="description"
                defaultValue={project.description}
                disabled={isSaving}
                required
              />
            </div>

            <div className="form-field">
              <label htmlFor="project-repository-url">Repository URL</label>

              <input
                id="project-repository-url"
                name="repositoryUrl"
                type="url"
                defaultValue={project.repositoryUrl}
                disabled={isSaving}
                required
              />
            </div>

            <div className="form-field">
              <label htmlFor="project-live-url">Live URL</label>

              <input
                id="project-live-url"
                name="liveUrl"
                type="url"
                defaultValue={project.liveUrl ?? ""}
                disabled={isSaving}
              />
            </div>

            <div className="form-field project-featured-field">
              <label>
                <input
                  name="isFeatured"
                  type="checkbox"
                  defaultChecked={project.isFeatured}
                  disabled={isSaving}
                />
                Featured project
              </label>
            </div>

            <div className="form-field project-tags-field">
              <div className="project-tags-field__header">
                <div>
                  <p className="manage-skill-group-page__eyebrow">
                    Project tags
                  </p>
                  <h4>Tags</h4>
                </div>

                <p>Attach existing tags or create a new one.</p>
              </div>

              <div className="project-tag-list">
                {projectTags.length > 0 ? (
                  projectTags.map((tag) => (
                    <span className="project-tag-pill" key={tag.id}>
                      <span>{tag.name}</span>

                      <button
                        type="button"
                        className="project-tag-remove"
                        aria-label={`Remove ${tag.name}`}
                        title={`Remove ${tag.name}`}
                        disabled={isSaving}
                        onClick={() => handleRemoveTag(tag.id)}
                      >
                        ×
                      </button>
                    </span>
                  ))
                ) : (
                  <p className="project-tags-empty">
                    No tags are attached to this project.
                  </p>
                )}
              </div>

              <div className="project-tag-controls">
                <div className="project-tag-control">
                  <label htmlFor="project-tag-select">Add existing tag</label>

                  <div className="project-tag-control__row">
                    <select
                      id="project-tag-select"
                      value={selectedTagId}
                      disabled={
                        isSaving ||
                        availableTagsQuery.isPending ||
                        unattachedTags.length === 0
                      }
                      onChange={(event) => setSelectedTagId(event.target.value)}
                    >
                      <option value="">
                        {availableTagsQuery.isPending
                          ? "Loading tags..."
                          : unattachedTags.length === 0
                            ? "No more tags available"
                            : "Select a tag"}
                      </option>

                      {unattachedTags.map((tag) => (
                        <option key={tag.id} value={tag.id}>
                          {tag.name}
                        </option>
                      ))}
                    </select>

                    <button
                      type="button"
                      className="button button--secondary"
                      disabled={!selectedTagId || isSaving}
                      onClick={handleAddTag}
                    >
                      Add tag
                    </button>
                  </div>
                </div>

                <div className="project-tag-control">
                  <label htmlFor="new-project-tag">Create new tag</label>

                  <div className="project-tag-control__row">
                    <input
                      id="new-project-tag"
                      type="text"
                      value={newTagName}
                      placeholder="Enter a tag name"
                      disabled={isSaving || createTagMutation.isPending}
                      onChange={(event) => setNewTagName(event.target.value)}
                    />

                    <button
                      type="button"
                      className="button button--secondary"
                      disabled={
                        !newTagName.trim() ||
                        isSaving ||
                        createTagMutation.isPending
                      }
                      onClick={handleCreateTag}
                    >
                      {createTagMutation.isPending
                        ? "Creating..."
                        : "Create tag"}
                    </button>
                  </div>
                </div>
              </div>

              {availableTagsQuery.isError && (
                <p className="form-message form-message--error">
                  Could not load the available tags.
                </p>
              )}
            </div>
          </div>
        </section>

        {formError && (
          <p className="form-message form-message--error">{formError}</p>
        )}

        {updateMutation.isError && (
          <p className="form-message form-message--error">
            Could not update the project.
          </p>
        )}

        {deleteMutation.isError && (
          <p className="form-message form-message--error">
            Could not delete the project.
          </p>
        )}

        <div className="manage-skill-group-form__actions">
          <button className="button" type="submit" disabled={isSaving}>
            {updateMutation.isPending ? "Saving..." : "Save project"}
          </button>

          <button
            className="button button--danger"
            type="button"
            disabled={isSaving}
            onClick={() => {
              if (window.confirm(`Delete "${project.title}"?`)) {
                deleteMutation.mutate();
              }
            }}
          >
            {deleteMutation.isPending ? "Deleting..." : "Delete project"}
          </button>
        </div>
      </form>
    </section>
  );
}
