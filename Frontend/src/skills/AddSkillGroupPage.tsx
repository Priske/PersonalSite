import { type FormEvent, useMemo, useState } from "react";
import {Link, useNavigate} from "react-router-dom";

import { useCreateSkillGroup } from "./useCreateSkillGroup";
import { useSkillGroups } from "./useSkillGroups";

export function AddSkillGroupPage()
{
    const navigate = useNavigate();

    const createSkillGroupMutation = useCreateSkillGroup();
    const groupsQuery = useSkillGroups();

    const highestDisplayOrder = useMemo(() => {
        const groups = groupsQuery.data?.items ?? [];

        return groups.length === 0
            ? 0
            : Math.max(
                ...groups.map(group => group.displayOrder)
            );
    }, [groupsQuery.data]);

    const [name, setName] = useState("");
    const [validationMessage, setValidationMessage] =
        useState<string | null>(null);

    const isSaving =
        createSkillGroupMutation.isPending;

    async function handleSubmit(
        event: FormEvent<HTMLFormElement>,
    ) {
        event.preventDefault();

        setValidationMessage(null);

        const cleanedName = name.trim();

        if (!cleanedName) {
            setValidationMessage(
                "The skill group name is required.",
            );

            return;
        }

        try {
            const group = await createSkillGroupMutation.mutateAsync({
                name: cleanedName,
                displayOrder: highestDisplayOrder + 1,
            });

            navigate(`/account/skills/${group.id}/edit`, {
                replace: true,
            });
        } catch {
                setValidationMessage("Could not create the skill group.");
        }
    }

    return (
        <section className="manage-skill-group-page">
            <header className="manage-skill-group-page__header">
                <div>
                    <p className="manage-skill-group-page__eyebrow">
                        Create skill group
                    </p>

                    <h2>
                        {name.trim() || "New skill group"}
                    </h2>

                    <p>
                        Create a new skill group. Skills can be
                        added after the group has been created.
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
                                autoFocus
                            />
                        </div>
                    </div>
                </section>

                {validationMessage && (
                    <p className="form-message form-message--error">
                        {validationMessage}
                    </p>
                )}

                {createSkillGroupMutation.isError && (
                    <p className="form-message form-message--error">
                        
                    </p>
                )}

                <div className="manage-skill-group-form__actions">
                    <button
                        className="button button--primary"
                        type="submit"
                        disabled={isSaving}
                    >
                        {isSaving
                            ? "Creating..."
                            : "Create skill group"}
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