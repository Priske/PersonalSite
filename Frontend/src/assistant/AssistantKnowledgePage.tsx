import { useMutation } from "@tanstack/react-query";
import { useRef, useState, type FormEvent } from "react";
import { uploadAssistantKnowledge } from "./assistantKnowledgeApi";

export function AssistantKnowledgePage() {
  const formRef = useRef<HTMLFormElement>(null);
  const [file, setFile] = useState<File | null>(null);

  const uploadMutation = useMutation({
    mutationFn: uploadAssistantKnowledge,
    onSuccess: () => {
      setFile(null);
      formRef.current?.reset();
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (file) {
      uploadMutation.mutate(file);
    }
  }

  return (
    <section className="manage-skill-group-page">
      <header className="manage-skill-group-page__header">
        <div>
          <p className="manage-skill-group-page__eyebrow">Assistant</p>

          <h2>Knowledge files</h2>

          <p>Upload Markdown documents that the portfolio assistant can use.</p>
        </div>
      </header>

      <form
        ref={formRef}
        className="manage-skill-group-form"
        onSubmit={handleSubmit}
      >
        <div className="form-field">
          <label htmlFor="assistant-knowledge-file">Markdown file</label>

          <input
            id="assistant-knowledge-file"
            type="file"
            accept=".md,text/markdown,text/plain"
            disabled={uploadMutation.isPending}
            onChange={(event) => setFile(event.target.files?.[0] ?? null)}
            required
          />
        </div>

        {uploadMutation.isSuccess && (
          <p className="form-message">Knowledge file uploaded successfully.</p>
        )}

        {uploadMutation.isError && (
          <p className="form-message form-message--error">
            {uploadMutation.error.message}
          </p>
        )}

        <div className="manage-skill-group-form__actions">
          <button
            className="button"
            type="submit"
            disabled={!file || uploadMutation.isPending}
          >
            {uploadMutation.isPending ? "Uploading..." : "Upload file"}
          </button>
        </div>
      </form>
    </section>
  );
}
