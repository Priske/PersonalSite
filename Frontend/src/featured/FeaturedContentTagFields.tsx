import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { createTag, getTags } from "../tags/tagsApi";
import type { FeaturedContentTag } from "./types";

type FeaturedContentTagFieldsProps = {
  tags: FeaturedContentTag[];
  disabled: boolean;
  onChange: (tags: FeaturedContentTag[]) => void;
};

export function FeaturedContentTagFields({
  tags,
  disabled,
  onChange,
}: FeaturedContentTagFieldsProps) {
  const queryClient = useQueryClient();
  const [selectedTagId, setSelectedTagId] = useState("");
  const [newTagName, setNewTagName] = useState("");

  const availableTagsQuery = useQuery({
    queryKey: ["tags", { page: 1, pageSize: 100, search: "" }],
    queryFn: () => getTags({ page: 1, pageSize: 100 }),
  });

  const createTagMutation = useMutation({
    mutationFn: createTag,
    onSuccess: async (createdTag) => {
      if (!tags.some((tag) => tag.id === createdTag.id)) {
        onChange(
          [...tags, createdTag].sort((first, second) =>
            first.name.localeCompare(second.name),
          ),
        );
      }

      setNewTagName("");
      await queryClient.invalidateQueries({ queryKey: ["tags"] });
    },
  });

  const availableTags = availableTagsQuery.data?.items ?? [];
  const unattachedTags = availableTags.filter(
    (availableTag) => !tags.some((tag) => tag.id === availableTag.id),
  );
  const isDisabled = disabled || createTagMutation.isPending;

  function addExistingTag() {
    const tagId = Number(selectedTagId);
    const tag = availableTags.find((candidate) => candidate.id === tagId);

    if (!tag) {
      return;
    }

    onChange(
      [...tags, tag].sort((first, second) =>
        first.name.localeCompare(second.name),
      ),
    );
    setSelectedTagId("");
  }

  function createNewTag() {
    const name = newTagName.trim();

    if (!name) {
      return;
    }

    const existingTag = availableTags.find(
      (tag) => tag.name.toLowerCase() === name.toLowerCase(),
    );

    if (existingTag) {
      if (!tags.some((tag) => tag.id === existingTag.id)) {
        onChange(
          [...tags, existingTag].sort((first, second) =>
            first.name.localeCompare(second.name),
          ),
        );
      }

      setNewTagName("");
      return;
    }

    createTagMutation.mutate({ name });
  }

  function removeTag(tagId: number) {
    onChange(tags.filter((tag) => tag.id !== tagId));
  }

  return (
    <div className="form-field project-tags-field">
      <div className="project-tags-field__header">
        <div>
          <p className="manage-skill-group-page__eyebrow">
            Featured content tags
          </p>
          <h4>Tags</h4>
        </div>

        <p>Attach existing tags or create a new one.</p>
      </div>

      <div className="project-tag-list">
        {tags.length > 0 ? (
          tags.map((tag) => (
            <span className="project-tag-pill" key={tag.id}>
              <span>{tag.name}</span>

              <button
                type="button"
                className="project-tag-remove"
                aria-label={`Remove ${tag.name}`}
                title={`Remove ${tag.name}`}
                disabled={isDisabled}
                onClick={() => removeTag(tag.id)}
              >
                ×
              </button>
            </span>
          ))
        ) : (
          <p className="project-tags-empty">
            No tags are attached to this content.
          </p>
        )}
      </div>

      <div className="project-tag-controls">
        <div className="project-tag-control">
          <label htmlFor="featured-tag-select">Add existing tag</label>

          <div className="project-tag-control__row">
            <select
              id="featured-tag-select"
              value={selectedTagId}
              disabled={
                isDisabled ||
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
              disabled={!selectedTagId || isDisabled}
              onClick={addExistingTag}
            >
              Add tag
            </button>
          </div>
        </div>

        <div className="project-tag-control">
          <label htmlFor="new-featured-tag">Create new tag</label>

          <div className="project-tag-control__row">
            <input
              id="new-featured-tag"
              type="text"
              value={newTagName}
              placeholder="Enter a tag name"
              disabled={isDisabled}
              onChange={(event) => setNewTagName(event.target.value)}
            />

            <button
              type="button"
              className="button button--secondary"
              disabled={!newTagName.trim() || isDisabled}
              onClick={createNewTag}
            >
              {createTagMutation.isPending ? "Creating..." : "Create tag"}
            </button>
          </div>
        </div>
      </div>

      {availableTagsQuery.isError && (
        <p className="form-message form-message--error">
          Could not load the available tags.
        </p>
      )}

      {createTagMutation.isError && (
        <p className="form-message form-message--error">
          Could not create the tag.
        </p>
      )}
    </div>
  );
}
