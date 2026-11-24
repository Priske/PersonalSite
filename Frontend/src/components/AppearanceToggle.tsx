import type { Appearance } from "../appearance";

export function AppearanceToggle({
  appearance,
  onToggle,
}: {
  appearance: Appearance;
  onToggle: () => void;
}) {
  const label = appearance === "dark" ? "Switch to light mode" : "Switch to dark mode";

  return (
    <button
      className="appearance-toggle"
      type="button"
      aria-label={label}
      title={label}
      onClick={onToggle}
    >
      <svg
        width="22"
        height="22"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        strokeWidth="1.75"
        strokeLinecap="round"
        strokeLinejoin="round"
        aria-hidden="true"
        focusable="false"
      >
        {appearance === "dark" ? (
          <>
            <circle cx="12" cy="12" r="4" />
            <path d="M12 2v2m0 16v2M2 12h2m16 0h2M4.93 4.93l1.42 1.42m11.3 11.3 1.42 1.42M4.93 19.07l1.42-1.42m11.3-11.3 1.42-1.42" />
          </>
        ) : (
          <path d="M20.9 13.3A9 9 0 0 1 10.7 3.1a9 9 0 1 0 10.2 10.2Z" />
        )}
      </svg>
    </button>
  );
}
