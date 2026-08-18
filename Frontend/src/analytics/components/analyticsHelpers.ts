export function toIsoDate(value: string, endOfDay = false) {
  if (!value) {
    return undefined;
  }

  const date = new Date(
    `${value}T${endOfDay ? "23:59:59.999" : "00:00:00.000"}`,
  );

  return date.toISOString();
}

export function formatFailureReason(reason: string | null) {
  switch (reason) {
    case "unknown_email":
      return "Unknown email";

    case "incorrect_password":
      return "Incorrect password";

    case "missing_credentials":
      return "Missing credentials";

    default:
      return "—";
  }
}

export function formatReferrer(referrer: string) {
  if (referrer === "Direct") {
    return "Direct";
  }

  try {
    return new URL(referrer).hostname;
  } catch {
    return referrer;
  }
}

export function formatDeleteFailureReason(reason: string | null) {
  switch (reason) {
    case "unknown_delete_user":
      return "Unknown user";

    default:
      return "—";
  }
}
