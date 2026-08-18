import { useEffect, useState, type FormEvent } from "react";
import { ContactSection } from "../home/ContactSection";
import { HeroSection } from "../home/HeroSection";
import type {
  GetHomePageConfigDetailsResponse,
  UpdateHomePageConfigRequest,
} from "./types";
import { uploadCv } from "../Files/FilesApi";

type HomePageEditorProps = {
  config: GetHomePageConfigDetailsResponse | undefined;
  isLoading: boolean;
  isLoadError: boolean;
  isSaving: boolean;
  saveError: Error | null;
  isSaveSuccess: boolean;
  canUploadCv: boolean;
  onSave: (request: UpdateHomePageConfigRequest) => void;
};

export function HomePageEditor({
  config,
  isLoading,
  isLoadError,
  isSaving,
  saveError,
  isSaveSuccess,
  canUploadCv,
  onSave,
}: HomePageEditorProps) {
  const [isPreviewOpen, setIsPreviewOpen] = useState(false);
  const [cvFile, setCvFile] = useState<File | null>(null);

  const [form, setForm] = useState<GetHomePageConfigDetailsResponse | null>(
    null,
  );

  useEffect(() => {
    if (config) {
      setForm(config);
    }
  }, [config]);

  function updateField(
    field: keyof UpdateHomePageConfigRequest,
    value: string,
  ) {
    setForm((current) => {
      if (!current) {
        return current;
      }

      return {
        ...current,
        [field]: value,
      };
    });
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!form) {
      return;
    }

    onSave(form);

    if (canUploadCv && cvFile) {
      await uploadCv(cvFile);
    }
  }

  if (isLoading || !form) {
    return <p>Loading homepage...</p>;
  }

  if (isLoadError) {
    return (
      <p className="form-message form-message--error">
        Could not load the homepage.
      </p>
    );
  }

  return (
    <div className="home-editor">
      <header className="home-editor__header">
        <div>
          <p className="home-editor__eyebrow">Homepage content</p>
          <h2>Edit hero</h2>
        </div>

        <div className="home-editor__header-actions">
          <button
            className="button button--secondary"
            type="button"
            onClick={() => setIsPreviewOpen(true)}
          >
            Preview
          </button>

          <button
            className="button"
            type="submit"
            form="hero-editor-form"
            disabled={isSaving}
          >
            {isSaving ? "Saving..." : "Save changes"}
          </button>
        </div>
      </header>

      <form
        id="hero-editor-form"
        className="home-editor__form"
        onSubmit={handleSubmit}
      >
        <div>
          {saveError && (
            <p className="form-message form-message--error">
              {saveError.message}
            </p>
          )}

          {isSaveSuccess && (
            <p className="form-message form-message--success">
              Homepage saved.
            </p>
          )}
        </div>

        <div className="home-editor__field-group">
          <h3>Identity</h3>

          <label className="form-field">
            <span>Banner</span>
            <input
              value={form.heroBanner}
              onChange={(event) =>
                updateField("heroBanner", event.target.value)
              }
              required
            />
          </label>

          <div className="home-editor__field-row">
            <label className="form-field">
              <span>First name</span>
              <input
                value={form.heroFirstName}
                onChange={(event) =>
                  updateField("heroFirstName", event.target.value)
                }
                required
              />
            </label>

            <label className="form-field">
              <span>Last name</span>
              <input
                value={form.heroLastName}
                onChange={(event) =>
                  updateField("heroLastName", event.target.value)
                }
                required
              />
            </label>
          </div>

          <label className="form-field">
            <span>Role</span>
            <input
              value={form.heroRole}
              onChange={(event) => updateField("heroRole", event.target.value)}
              required
            />
          </label>
        </div>

        <div className="home-editor__field-group">
          <h3>Introduction</h3>

          <label className="form-field">
            <span>Eyebrow</span>
            <input
              value={form.heroEyebrow}
              onChange={(event) =>
                updateField("heroEyebrow", event.target.value)
              }
              required
            />
          </label>

          <label className="form-field">
            <span>Heading</span>
            <textarea
              rows={5}
              value={form.heroHeading}
              onChange={(event) =>
                updateField("heroHeading", event.target.value)
              }
              required
            />
          </label>

          <label className="form-field">
            <span>Summary</span>
            <textarea
              rows={7}
              value={form.heroSummary}
              onChange={(event) =>
                updateField("heroSummary", event.target.value)
              }
              required
            />
          </label>
        </div>

        <div className="home-editor__field-group">
          <h3>Buttons</h3>

          <label className="form-field">
            <span>Primary button</span>
            <input
              value={form.heroPrimaryActionLabel}
              onChange={(event) =>
                updateField("heroPrimaryActionLabel", event.target.value)
              }
              required
            />
          </label>

          <label className="form-field">
            <span>Secondary button</span>
            <input
              value={form.heroSecondaryActionLabel}
              onChange={(event) =>
                updateField("heroSecondaryActionLabel", event.target.value)
              }
              required
            />
          </label>
        </div>

        <div className="home-editor__section-heading">
          <div>
            <p className="home-editor__eyebrow">Contact content</p>
            <h3>Contact section</h3>
          </div>
        </div>

        <div className="home-editor__field-group">
          <h3>Section heading</h3>

          <label className="form-field">
            <span>Section number</span>
            <input
              value={form.contactSectionNumber}
              onChange={(event) =>
                updateField("contactSectionNumber", event.target.value)
              }
              disabled
            />
          </label>

          <label className="form-field">
            <span>Section eyebrow</span>
            <input
              value={form.contactSectionEyebrow}
              onChange={(event) =>
                updateField("contactSectionEyebrow", event.target.value)
              }
              required
            />
          </label>

          <label className="form-field">
            <span>Section heading</span>
            <input
              value={form.contactSectionHeading}
              onChange={(event) =>
                updateField("contactSectionHeading", event.target.value)
              }
              required
            />
          </label>
        </div>

        <div className="home-editor__field-group">
          <h3>Contact message</h3>

          <label className="form-field">
            <span>Eyebrow</span>
            <input
              value={form.contactEyebrow}
              onChange={(event) =>
                updateField("contactEyebrow", event.target.value)
              }
              required
            />
          </label>

          <label className="form-field">
            <span>Heading</span>
            <input
              value={form.contactHeading}
              onChange={(event) =>
                updateField("contactHeading", event.target.value)
              }
              required
            />
          </label>

          <label className="form-field">
            <span>Description</span>
            <textarea
              rows={7}
              value={form.contactDescription}
              onChange={(event) =>
                updateField("contactDescription", event.target.value)
              }
              required
            />
          </label>
        </div>

        <div className="home-editor__field-group">
          <h3>Contact actions</h3>

          <label className="form-field">
            <span>Email button</span>
            <input
              value={form.contactEmailActionLabel}
              onChange={(event) =>
                updateField("contactEmailActionLabel", event.target.value)
              }
              required
            />
          </label>

          <label className="form-field">
            <span>Login button</span>
            <input
              value={form.contactLoginActionLabel}
              onChange={(event) =>
                updateField("contactLoginActionLabel", event.target.value)
              }
              required
            />
          </label>
        </div>

        <div className="home-editor__field-group">
          <h3>Contact details</h3>

          <label className="form-field">
            <span>Email</span>
            <input
              type="email"
              value={form.email}
              onChange={(event) => updateField("email", event.target.value)}
              required
            />
          </label>

          <label className="form-field">
            <span>Phone number</span>
            <input
              type="tel"
              value={form.phoneNumber ?? ""}
              onChange={(event) =>
                updateField("phoneNumber", event.target.value)
              }
              required
            />
          </label>

          <label className="form-field">
            <span>LinkedIn URL</span>
            <input
              type="url"
              value={form.linkedInUrl ?? ""}
              onChange={(event) =>
                updateField("linkedInUrl", event.target.value)
              }
            />
          </label>

          <label className="form-field">
            <span>GitHub URL</span>
            <input
              type="url"
              value={form.gitHubUrl ?? ""}
              onChange={(event) => updateField("gitHubUrl", event.target.value)}
            />
          </label>

          {canUploadCv ? (
            <label className="form-field">
              <span>CV</span>

              <input
                type="file"
                accept=".pdf,application/pdf"
                onChange={(event) => {
                  setCvFile(event.target.files?.[0] ?? null);
                }}
              />
            </label>
          ) : (
            <div className="form-field">
              <span>CV</span>

              <a
                className="button button--secondary"
                href="/files/cv"
                target="_blank"
                rel="noreferrer"
              >
                View CV
              </a>
            </div>
          )}
        </div>
      </form>

      {isPreviewOpen && (
        <div
          className="home-preview-modal"
          role="dialog"
          aria-modal="true"
          aria-labelledby="home-preview-title"
          onMouseDown={() => setIsPreviewOpen(false)}
        >
          <div
            className="home-preview-modal__window"
            onMouseDown={(event) => event.stopPropagation()}
          >
            <header className="home-preview-modal__header">
              <div>
                <p className="home-editor__eyebrow">Unsaved changes included</p>
                <h2 id="home-preview-title">Hero preview</h2>
              </div>

              <button
                className="button button--secondary"
                type="button"
                onClick={() => setIsPreviewOpen(false)}
              >
                Close
              </button>
            </header>

            <div className="home-preview-modal__viewport">
              <div
                className="home-preview-modal__page"
                onClick={(event) => event.preventDefault()}
              >
                <HeroSection config={form} />
                <ContactSection config={form} />
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
