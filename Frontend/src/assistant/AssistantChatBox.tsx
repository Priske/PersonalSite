import { useMutation } from "@tanstack/react-query";
import {
  useEffect,
  useRef,
  useState,
  type FormEvent,
  type KeyboardEvent,
} from "react";
import { askQuestion } from "./assistantApi";

type ChatMessage = {
  id: number;
  role: "visitor" | "assistant";
  text: string;
};

const messagesStorageKey = "personal-site-assistant-messages";

const initialMessage: ChatMessage = {
  id: 1,
  role: "assistant",
  text: "Ask me about Ben’s projects, skills or how this portfolio was built.",
};

function loadStoredMessages(): ChatMessage[] {
  try {
    const storedMessages = window.sessionStorage.getItem(messagesStorageKey);

    if (!storedMessages) {
      return [initialMessage];
    }

    const parsedMessages: unknown = JSON.parse(storedMessages);

    if (!Array.isArray(parsedMessages)) {
      return [initialMessage];
    }

    const validMessages = parsedMessages.filter(
      (message): message is ChatMessage => {
        if (typeof message !== "object" || message === null) {
          return false;
        }

        const candidate = message as Partial<ChatMessage>;

        return (
          typeof candidate.id === "number" &&
          (candidate.role === "visitor" || candidate.role === "assistant") &&
          typeof candidate.text === "string"
        );
      },
    );

    return validMessages.length > 0 ? validMessages : [initialMessage];
  } catch {
    return [initialMessage];
  }
}

function getNextMessageId(messages: ChatMessage[]) {
  const highestId = messages.reduce(
    (currentHighest, message) => Math.max(currentHighest, message.id),
    0,
  );

  return highestId + 1;
}

export function AssistantChatBox() {
  const [isOpen, setIsOpen] = useState(false);
  const [question, setQuestion] = useState("");

  const [messages, setMessages] = useState<ChatMessage[]>(loadStoredMessages);

  const nextMessageId = useRef(getNextMessageId(messages));

  const inputRef = useRef<HTMLTextAreaElement>(null);

  const messagesEndRef = useRef<HTMLDivElement>(null);

  const formRef = useRef<HTMLFormElement>(null);

  const askMutation = useMutation({
    mutationFn: askQuestion,
    onSuccess: (response) => {
      addMessage("assistant", response.answer);
    },
  });

  useEffect(() => {
    try {
      window.sessionStorage.setItem(
        messagesStorageKey,
        JSON.stringify(messages),
      );
    } catch {
      // The chat continues to work even if browser
      // storage is unavailable.
    }
  }, [messages]);

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    inputRef.current?.focus();

    function closeWithEscape(event: globalThis.KeyboardEvent) {
      if (event.key === "Escape") {
        setIsOpen(false);
      }
    }

    document.addEventListener("keydown", closeWithEscape);

    return () => {
      document.removeEventListener("keydown", closeWithEscape);
    };
  }, [isOpen]);

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    messagesEndRef.current?.scrollIntoView({
      behavior: "smooth",
      block: "end",
    });
  }, [isOpen, messages, askMutation.isPending]);

  useEffect(() => {
    if (isOpen && !askMutation.isPending) {
      inputRef.current?.focus();
    }
  }, [isOpen, askMutation.isPending]);

  function addMessage(role: ChatMessage["role"], text: string) {
    const message: ChatMessage = {
      id: nextMessageId.current,
      role,
      text,
    };

    nextMessageId.current += 1;

    setMessages((currentMessages) => [...currentMessages, message]);
  }

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedQuestion = question.trim();

    if (!trimmedQuestion || askMutation.isPending) {
      return;
    }

    addMessage("visitor", trimmedQuestion);

    setQuestion("");

    askMutation.mutate({
      question: trimmedQuestion,
    });
  }

  function handleQuestionKeyDown(event: KeyboardEvent<HTMLTextAreaElement>) {
    if (
      event.key === "Enter" &&
      !event.shiftKey &&
      !event.nativeEvent.isComposing
    ) {
      event.preventDefault();

      formRef.current?.requestSubmit();
    }
  }

  if (!isOpen) {
    return (
      <div className="assistant-chat">
        <button
          type="button"
          className="assistant-chat__launcher"
          aria-expanded="false"
          aria-controls="assistant-chat-panel"
          onClick={() => setIsOpen(true)}
        >
          <span className="assistant-chat__launcher-icon" aria-hidden="true">
            ?
          </span>

          <span>Ask about Ben</span>
        </button>
      </div>
    );
  }

  return (
    <div className="assistant-chat">
      <section
        id="assistant-chat-panel"
        className="assistant-chat__panel"
        role="dialog"
        aria-labelledby="assistant-chat-title"
      >
        <header className="assistant-chat__header">
          <div>
            <h2 id="assistant-chat-title" className="assistant-chat__title">
              Ask about Ben
            </h2>

            <p className="assistant-chat__subtitle">Portfolio assistant</p>
          </div>

          <button
            type="button"
            className="assistant-chat__close"
            aria-label="Close portfolio assistant"
            onClick={() => setIsOpen(false)}
          >
            <span aria-hidden="true">×</span>
          </button>
        </header>

        <div
          className="assistant-chat__messages"
          aria-live="polite"
          aria-busy={askMutation.isPending}
        >
          {messages.map((message) => (
            <article
              key={message.id}
              className={
                message.role === "visitor"
                  ? "assistant-chat__message assistant-chat__message--visitor"
                  : "assistant-chat__message assistant-chat__message--assistant"
              }
            >
              <span className="assistant-chat__message-label">
                {message.role === "visitor" ? "You" : "Assistant"}
              </span>

              <p>{message.text}</p>
            </article>
          ))}

          {askMutation.isPending && (
            <article className="assistant-chat__message assistant-chat__message--assistant">
              <span className="assistant-chat__message-label">Assistant</span>

              <p>Thinking…</p>
            </article>
          )}

          {askMutation.isError && (
            <p className="assistant-chat__error" role="alert">
              {askMutation.error.message}
            </p>
          )}

          <div ref={messagesEndRef} />
        </div>

        <form
          ref={formRef}
          className="assistant-chat__form"
          onSubmit={handleSubmit}
        >
          <label className="sr-only" htmlFor="assistant-chat-question">
            Ask a question about Ben
          </label>

          <textarea
            ref={inputRef}
            id="assistant-chat-question"
            className="assistant-chat__input"
            value={question}
            rows={2}
            placeholder="Ask a question…"
            disabled={askMutation.isPending}
            onChange={(event) => setQuestion(event.target.value)}
            onKeyDown={handleQuestionKeyDown}
          />

          <button
            type="submit"
            className="assistant-chat__send"
            disabled={askMutation.isPending || question.trim().length === 0}
          >
            {askMutation.isPending ? "Waiting…" : "Send"}
          </button>
        </form>
      </section>
    </div>
  );
}
