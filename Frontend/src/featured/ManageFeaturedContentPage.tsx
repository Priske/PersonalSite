import { useEffect, useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useParams } from "react-router-dom";
import { ApiError, apiPath } from "../api";
import {
  getFeaturedContentDetails,
  removeFeaturedContentFile,
  updateFeaturedContent,
  uploadFeaturedContentFile,
} from "./featuredContentApi";
import { FeaturedContentTagFields } from "./FeaturedContentTagFields";
import type {
  FeaturedContentTag,
  UpdateFeaturedContentRequest,
} from "./types";

function readId(value: string | undefined) {
  const id = Number(value);
  return Number.isInteger(id) && id > 0 ? id : null;
}

function formatFileSize(bytes: number) {
  return `${(bytes / 1024 / 1024).toFixed(1)} MB`;
}

export function ManageFeaturedContentPage() {
  const { featuredContentId: parameter } = useParams();
  const id = readId(parameter);
  const queryClient = useQueryClient();
  const [tags, setTags] = useState<FeaturedContentTag[]>([]);
  const [formError, setFormError] = useState<string | null>(null);

  const contentQuery = useQuery({
    queryKey: ["featured-content", "detail", id],
    queryFn: () => {
      if (id === null) throw new Error("Invalid featured-content ID.");
      return getFeaturedContentDetails(id);
    },
    enabled: id !== null,
    retry: false,
  });

  useEffect(() => {
    if (contentQuery.data) {
      setTags(contentQuery.data.tags);
    }
  }, [contentQuery.data]);

  async function refreshContent() {
    await Promise.all([
      queryClient.invalidateQueries({ queryKey: ["featured-content"] }),
      queryClient.invalidateQueries({ queryKey: ["featured-content", "detail", id] }),
    ]);
  }

  const updateMutation = useMutation({
    mutationFn: (request: UpdateFeaturedContentRequest) => {
      if (id === null) throw new Error("Invalid featured-content ID.");
      return updateFeaturedContent(id, request);
    },
    onSuccess: refreshContent,
  });

  const uploadMutation = useMutation({
    mutationFn: async (files: File[]) => {
      if (id === null) throw new Error("Invalid featured-content ID.");
      for (const file of files) {
        await uploadFeaturedContentFile(id, file);
      }
    },
    onSuccess: refreshContent,
  });

  const removeMutation = useMutation({
    mutationFn: (fileId: number) => {
      if (id === null) throw new Error("Invalid featured-content ID.");
      return removeFeaturedContentFile(id, fileId);
    },
    onSuccess: refreshContent,
  });

  function handleUpdate(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);
    const data = new FormData(event.currentTarget);
    const title = data.get("title")?.toString().trim() ?? "";
    const description = data.get("description")?.toString().trim() ?? "";
    if (!title || !description) {
      setFormError("Title and description are required.");
      return;
    }
    updateMutation.mutate({
      title,
      description,
      tagIds: tags.map((tag) => tag.id),
    });
  }

  function handleUpload(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const input = event.currentTarget.elements.namedItem("files") as HTMLInputElement;
    const files = Array.from(input.files ?? []);
    if (files.length === 0) {
      setFormError("Select at least one file.");
      return;
    }
    setFormError(null);
    uploadMutation.mutate(files);
  }

  if (id === null) {
    return <p className="form-message form-message--error">Invalid featured-content ID.</p>;
  }

  if (contentQuery.isPending) {
    return <p className="account-management__status">Loading featured content...</p>;
  }

  const notFound = contentQuery.error instanceof ApiError && contentQuery.error.status === 404;

  if (notFound || contentQuery.isError) {
    return (
      <section className="manage-skill-group-page__message">
        <p className="form-message form-message--error">
          {notFound ? "Featured content not found." : "Could not load featured content."}
        </p>
        <Link className="button button--secondary" to="/account/featured-content">Back to content</Link>
      </section>
    );
  }

  return (
    <section className="manage-skill-group-page">
      <header className="manage-skill-group-page__header">
        <div>
          <p className="manage-skill-group-page__eyebrow">Featured content</p>
          <h2>Edit content</h2>
          <p>Update its text, tags, and attached files.</p>
        </div>
        <Link className="button button--secondary" to="/account/featured-content">Back to content</Link>
      </header>

      <form className="manage-skill-group-form featured-admin-form" onSubmit={handleUpdate}>
        <div className="featured-admin-form__fields">
          <div className="form-field">
            <label htmlFor="featured-title">Title</label>
            <input id="featured-title" name="title" defaultValue={contentQuery.data.title} disabled={updateMutation.isPending} required />
          </div>
          <div className="form-field">
            <label htmlFor="featured-description">Description</label>
            <textarea id="featured-description" name="description" defaultValue={contentQuery.data.description} disabled={updateMutation.isPending} required />
          </div>
        </div>

        <FeaturedContentTagFields
          tags={tags}
          disabled={updateMutation.isPending}
          onChange={setTags}
        />

        {formError && <p className="form-message form-message--error">{formError}</p>}
        {updateMutation.isError && <p className="form-message form-message--error">{updateMutation.error.message}</p>}
        {updateMutation.isSuccess && <p className="form-message">Changes saved.</p>}

        <div className="manage-skill-group-form__actions">
          <button className="button" type="submit" disabled={updateMutation.isPending}>
            {updateMutation.isPending ? "Saving..." : "Save changes"}
          </button>
        </div>
      </form>

      <section className="featured-admin-section featured-admin-files">
        <header>
          <div>
            <p className="manage-skill-group-page__eyebrow">Attachments</p>
            <h3>Files</h3>
          </div>
        </header>

        <form className="featured-admin-upload" onSubmit={handleUpload}>
          <input name="files" type="file" accept="video/mp4,video/webm,image/jpeg,image/png,image/webp,application/pdf" multiple disabled={uploadMutation.isPending} />
          <button className="button" type="submit" disabled={uploadMutation.isPending}>
            {uploadMutation.isPending ? "Uploading..." : "Upload files"}
          </button>
        </form>

        {uploadMutation.isError && <p className="form-message form-message--error">{uploadMutation.error.message}</p>}
        {removeMutation.isError && <p className="form-message form-message--error">{removeMutation.error.message}</p>}

        {contentQuery.data.files.length === 0 ? (
          <p className="featured-admin-files__empty">No files attached.</p>
        ) : (
          <ul className="featured-admin-file-list">
            {contentQuery.data.files.map((file) => (
              <li key={file.id}>
                <div>
                  <strong>{file.originalFileName}</strong>
                  <span>{file.contentType} · {formatFileSize(file.sizeInBytes)}</span>
                </div>
                <div className="featured-admin-file-list__actions">
                  <a className="button button--secondary" href={apiPath(`/files/${file.id}`)} target="_blank" rel="noreferrer">View</a>
                  <button className="button button--secondary" type="button" disabled={removeMutation.isPending} onClick={() => removeMutation.mutate(file.id)}>Remove</button>
                </div>
              </li>
            ))}
          </ul>
        )}
      </section>
    </section>
  );
}
