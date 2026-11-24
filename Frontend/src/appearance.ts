export type Appearance = "light" | "dark";

export const APPEARANCE_STORAGE_KEY = "personalsite.appearance";

export function readAppearance(): Appearance | null {
  try {
    const value = localStorage.getItem(APPEARANCE_STORAGE_KEY);
    return value === "light" || value === "dark" ? value : null;
  } catch {
    return null;
  }
}

export function saveAppearance(appearance: Appearance): void {
  try {
    localStorage.setItem(APPEARANCE_STORAGE_KEY, appearance);
  } catch {
    // Switching still works for this page session when storage is unavailable.
  }
}
