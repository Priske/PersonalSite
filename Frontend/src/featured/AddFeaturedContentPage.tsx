import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { createFeaturedContent } from "./featuredContentApi";
import { FeaturedContentTagFields } from "./FeaturedContentTagFields";
import type {
  CreateFeaturedContentRequest,
  FeaturedContentTag,
} from "./types";

export function AddFeaturedContentPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [tags, setTags] = useState<FeaturedContentTag[]>([]);
  const [formError, setFormError] = useState<string | null>(null);

  const createMutation = useMutation({
    mutationFn: (request: CreateFeaturedContentRequest) =>
      createFeaturedContent(request),
    onSuccess: async (created) => {
      await queryClient.invalidateQueries({ queryKey: ["featured-content"] });
      navigate(`/account/featured-content/${created.id}/edit`);
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const data = new FormData(event.currentTarget);
    const title = data.get("title")?.toString().trim() ?? "";
    const description = data.get("description")?.toString().trim() ?? "";

    if (!title || !description) {
      setFormError("Title and description are required.");
      return;
    }

    createMutation.mutate({
      title,
      description,
      tagIds: tags.map((tag) => tag.id),
    });
  }

  return (
    <section className="manage-skill-group-page">
      <header className="manage-skill-group-page__header">
        <div>
          <p className="manage-skill-group-page__eyebrow">Featured content</p>
          <h2>Add content</h2>
          <p>Create the content record, then attach media on the edit page.</p>
        </div>
        <Link className="button button--secondary" to="/account/featured-content">
          Back to content
        </Link>
      </header>

      <form className="manage-skill-group-form featured-admin-form" onSubmit={handleSubmit}>
        <div className="featured-admin-form__fields">
          <div className="form-field">
            <label htmlFor="featured-title">Title</label>
            <input id="featured-title" name="title" disabled={createMutation.isPending} required />
          </div>
          <div className="form-field">
            <label htmlFor="featured-description">Description</label>
            <textarea id="featured-description" name="description" disabled={createMutation.isPending} required />
          </div>
        </div>

        <FeaturedContentTagFields
          tags={tags}
          disabled={createMutation.isPending}
          onChange={setTags}
        />

        {formError && <p className="form-message form-message--error">{formError}</p>}
        {createMutation.isError && (
          <p className="form-message form-message--error">{createMutation.error.message}</p>
        )}

        <div className="manage-skill-group-form__actions">
          <button className="button" type="submit" disabled={createMutation.isPending}>
            {createMutation.isPending ? "Creating..." : "Create content"}
          </button>
          <Link className="button button--secondary" to="/account/featured-content">Cancel</Link>
        </div>
      </form>
    </section>
  );
}
