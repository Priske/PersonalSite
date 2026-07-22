import {
  type FormEvent,
  useEffect,
  useState,
} from "react";
import {
  Link,
  Navigate,
  useNavigate,
  useParams,
} from "react-router-dom";
import { ApiError } from "../api";
import { useCreateSkill } from "./useCreateSkill";
import { useDeleteSkill } from "./useDeleteSkill";
import { useSkillGroup } from "./useSkillGroup";
import { useSkills } from "./useSkills";
import { useUpdateSkill } from "./useUpdateSkill";
import { useUpdateSkillGroup } from "./useUpdateSkillGroup";
import { useUpdateSkillOrder } from "./useUpdateSkillOrder";

type EditableSkill = {
  key: string;
  id: number | null;
  name: string;
};

export function ManageSkillGroupPage() {
  const navigate = useNavigate();
  const { groupId } = useParams();

  const parsedGroupId = Number(groupId);

  const hasValidGroupId =
    Number.isInteger(parsedGroupId) &&
    parsedGroupId > 0;

  const skillGroupQuery = useSkillGroup(
    parsedGroupId,
    hasValidGroupId,
  );

  const skillsQuery = useSkills(
    parsedGroupId,
    hasValidGroupId,
  );

  const updateSkillGroupMutation =
    useUpdateSkillGroup(parsedGroupId);

  const createSkillMutation =
    useCreateSkill(parsedGroupId);

  const updateSkillMutation =
    useUpdateSkill(parsedGroupId);

  const deleteSkillMutation =
    useDeleteSkill(parsedGroupId);

  const updateSkillOrderMutation =
    useUpdateSkillOrder(parsedGroupId);

  const [name, setName] = useState("");

  const [skills, setSkills] =
    useState<EditableSkill[]>([]);

  const [removedSkillIds, setRemovedSkillIds] =
    useState<number[]>([]);

  const [isInitialized, setIsInitialized] =
    useState(false);

  const [validationMessage, setValidationMessage] =
    useState<string | null>(null);

  useEffect(() => {
    if (
      isInitialized ||
      !skillGroupQuery.data ||
      !skillsQuery.data
    ) {
      return;
    }

    setName(skillGroupQuery.data.name);

    setSkills(
      [...skillsQuery.data.items]
        .sort(
          (left, right) =>
            left.displayOrder -
            right.displayOrder,
        )
        .map((skill) => ({
          key: `existing-${skill.id}`,
          id: skill.id,
          name: skill.name,
        })),
    );

    setIsInitialized(true);
  }, [
    isInitialized,
    skillGroupQuery.data,
    skillsQuery.data,
  ]);

  if (!hasValidGroupId) {
    return (
      <Navigate
        to="/account/skills"
        replace
      />
    );
  }

  const groupWasNotFound =
    skillGroupQuery.error instanceof ApiError &&
    skillGroupQuery.error.status === 404;

  if (groupWasNotFound) {
    return (
      <section className="manage-skill-group-page">
        <div className="manage-skill-group-page__message">
          <p className="form-message form-message--error">
            This skill group does not exist.
          </p>

          <Link
            className="button button--secondary"
            to="/account/skills"
          >
            Back to skills
          </Link>
        </div>
      </section>
    );
  }

  if (
    skillGroupQuery.isPending ||
    skillsQuery.isPending ||
    !isInitialized
  ) {
    return (
      <section className="manage-skill-group-page">
        <p className="account-management__status">
          Loading skill group...
        </p>
      </section>
    );
  }

  if (
    skillGroupQuery.isError ||
    skillsQuery.isError
  ) {
    return (
      <section className="manage-skill-group-page">
        <div className="manage-skill-group-page__message">
          <p className="form-message form-message--error">
            Could not load the skill group.
          </p>

          <Link
            className="button button--secondary"
            to="/account/skills"
          >
            Back to skills
          </Link>
        </div>
      </section>
    );
  }

  const isSaving =
    updateSkillGroupMutation.isPending ||
    createSkillMutation.isPending ||
    updateSkillMutation.isPending ||
    deleteSkillMutation.isPending ||
    updateSkillOrderMutation.isPending;

  const hasMutationError =
    updateSkillGroupMutation.isError ||
    createSkillMutation.isError ||
    updateSkillMutation.isError ||
    deleteSkillMutation.isError ||
    updateSkillOrderMutation.isError;

  const updateSkillName = (
    skillKey: string,
    newName: string,
  ) => {
    setSkills((currentSkills) =>
      currentSkills.map((skill) =>
        skill.key === skillKey
          ? {
              ...skill,
              name: newName,
            }
          : skill,
      ),
    );
  };

  const moveSkill = (
    currentIndex: number,
    direction: -1 | 1,
  ) => {
    setSkills((currentSkills) => {
      const targetIndex =
        currentIndex + direction;

      if (
        targetIndex < 0 ||
        targetIndex >= currentSkills.length
      ) {
        return currentSkills;
      }

      const reorderedSkills = [
        ...currentSkills,
      ];

      const currentSkill =
        reorderedSkills[currentIndex];

      reorderedSkills[currentIndex] =
        reorderedSkills[targetIndex];

      reorderedSkills[targetIndex] =
        currentSkill;

      return reorderedSkills;
    });
  };

  const addSkill = () => {
    setSkills((currentSkills) => [
      ...currentSkills,
      {
        key: `new-${crypto.randomUUID()}`,
        id: null,
        name: "",
      },
    ]);
  };

  const removeSkill = (
    skillToRemove: EditableSkill,
  ) => {
    setSkills((currentSkills) =>
      currentSkills.filter(
        (skill) =>
          skill.key !== skillToRemove.key,
      ),
    );

    if (skillToRemove.id !== null) {
      const removedId = skillToRemove.id;

      setRemovedSkillIds((currentIds) => [
        ...currentIds,
        removedId,
      ]);
    }
  };

  const validateForm = () => {
    if (!name.trim()) {
      return "The skill group name is required.";
    }

    for (const skill of skills) {
      if (!skill.name.trim()) {
        return "Every skill must have a name.";
      }
    }

    const normalizedNames = skills.map(
      (skill) =>
        skill.name.trim().toLowerCase(),
    );

    if (
      new Set(normalizedNames).size !==
      normalizedNames.length
    ) {
      return "Skill names must be unique within the group.";
    }

    return null;
  };

  const handleSubmit = async (
    event: FormEvent<HTMLFormElement>,
  ) => {
    event.preventDefault();
    setValidationMessage(null);

    const validationError = validateForm();

    if (validationError) {
      setValidationMessage(validationError);
      return;
    }

    try {
      await updateSkillGroupMutation.mutateAsync({
        name: name.trim(),
        displayOrder:
          skillGroupQuery.data.displayOrder,
      });

      for (const removedSkillId of removedSkillIds) {
        await deleteSkillMutation.mutateAsync(
          removedSkillId,
        );
      }

      const createdSkillIds =
        new Map<string, number>();

      for (const skill of skills) {
        if (skill.id !== null) {
          continue;
        }

        const createdSkill =
          await createSkillMutation.mutateAsync({
            name: skill.name.trim(),
            displayOrder:
              10_000 +
              createdSkillIds.size,
          });

        createdSkillIds.set(
          skill.key,
          createdSkill.id,
        );
      }

      const orderedSkills = skills.map(
        (skill, index) => {
          const id =
            skill.id ??
            createdSkillIds.get(skill.key);

          if (id === undefined) {
            throw new Error(
              `Could not determine the ID for "${skill.name}".`,
            );
          }

          return {
            id,
            name: skill.name.trim(),
            displayOrder: index + 1,
          };
        },
      );

      await updateSkillOrderMutation.mutateAsync({
        skillIds: orderedSkills.map(
          (skill) => skill.id,
        ),
      });

      for (const skill of orderedSkills) {
        await updateSkillMutation.mutateAsync({
          skillId: skill.id,
          name: skill.name,
          displayOrder: skill.displayOrder,
        });
      }

      navigate("/account/skills");
    } catch {
      // Mutation errors are rendered below.
    }
  };

  return (
    <section className="manage-skill-group-page">
      <header className="manage-skill-group-page__header">
        <div>
          <p className="manage-skill-group-page__eyebrow">
            Manage skill group
          </p>

          <h2>{name}</h2>

          <p>
            Edit the group and reorder all
            attached skills on this page.
          </p>
        </div>

        <Link
          className="button button--secondary"
          to="/account/skills"
        >
          Back to skills
        </Link>
      </header>

      <form
        className="manage-skill-group-form"
        onSubmit={handleSubmit}
      >
        <section className="manage-skill-group-section">
          <header className="manage-skill-group-section__header">
            <div>
              <p className="manage-skill-group-page__eyebrow">
                Group
              </p>

              <h3>Group details</h3>
            </div>
          </header>

          <div className="manage-skill-group-form__fields">
            <div className="form-field">
              <label htmlFor="skill-group-name">
                Group name
              </label>

              <input
                id="skill-group-name"
                name="name"
                type="text"
                value={name}
                onChange={(event) =>
                  setName(event.target.value)
                }
                disabled={isSaving}
              />
            </div>
          </div>
        </section>

        <section className="manage-skill-group-section">
          <header className="manage-skill-group-section__header">
            <div>
              <p className="manage-skill-group-page__eyebrow">
                Skills
              </p>

              <h3>Attached skills</h3>
            </div>

            <button
              className="button button--secondary"
              type="button"
              onClick={addSkill}
              disabled={isSaving}
            >
              Add skill
            </button>
          </header>

          {skills.length === 0 && (
            <p className="manage-skill-group-skills__empty">
              No skills are attached to this
              group yet.
            </p>
          )}

          {skills.length > 0 && (
            <div className="manage-skill-group-skills-editor">
              {skills.map((skill, index) => (
                <div
                  className="manage-skill-group-skill-editor"
                  key={skill.key}
                >
                  <span className="manage-skill-group-skill-editor__number">
                    {String(index + 1).padStart(
                      2,
                      "0",
                    )}
                  </span>

                  <div className="form-field">
                    <label
                      className="visually-hidden"
                      htmlFor={`skill-name-${skill.key}`}
                    >
                      Skill name
                    </label>

                    <input
                      id={`skill-name-${skill.key}`}
                      type="text"
                      value={skill.name}
                      onChange={(event) =>
                        updateSkillName(
                          skill.key,
                          event.target.value,
                        )
                      }
                      disabled={isSaving}
                    />
                  </div>

                  <div className="manage-skill-group-skill-editor__order-actions">
                    <button
                      className="button button--secondary"
                      type="button"
                      aria-label={`Move ${skill.name || "skill"} up`}
                      title="Move up"
                      onClick={() =>
                        moveSkill(index, -1)
                      }
                      disabled={
                        isSaving ||
                        index === 0
                      }
                    >
                      ↑
                    </button>

                    <button
                      className="button button--secondary"
                      type="button"
                      aria-label={`Move ${skill.name || "skill"} down`}
                      title="Move down"
                      onClick={() =>
                        moveSkill(index, 1)
                      }
                      disabled={
                        isSaving ||
                        index ===
                          skills.length - 1
                      }
                    >
                      ↓
                    </button>
                  </div>

                  <button
                    className="button button--secondary"
                    type="button"
                    onClick={() =>
                      removeSkill(skill)
                    }
                    disabled={isSaving}
                  >
                    Remove
                  </button>
                </div>
              ))}
            </div>
          )}
        </section>

        {validationMessage && (
          <p className="form-message form-message--error">
            {validationMessage}
          </p>
        )}

        {hasMutationError && (
          <p className="form-message form-message--error">
            Could not save all skill group changes.
          </p>
        )}

        <div className="manage-skill-group-form__actions">
          <button
            className="button button--primary"
            type="submit"
            disabled={isSaving}
          >
            {isSaving
              ? "Saving..."
              : "Save all changes"}
          </button>

          <Link
            className="button button--secondary"
            to="/account/skills"
          >
            Cancel
          </Link>
        </div>
      </form>
    </section>
  );
}