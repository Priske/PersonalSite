import { getAccessToken } from "./auth/tokenStorage";

const apiUrl = import.meta.env.VITE_API_URL ?? "";

type ApiErrorResponse = {
  error?: unknown;
  detail?: unknown;
};

export class ApiError extends Error {
  status: number;

  constructor(status: number, message: string) {
    super(message);

    this.name = "ApiError";
    this.status = status;
  }
}

export function apiPath(path: string) {
  return `${apiUrl}${path}`;
}

async function sendRequest(path: string, options: RequestInit) {
  const headers = new Headers(options.headers);
  const token = getAccessToken();

  headers.set("Accept", "application/json");

  if (options.body && !(options.body instanceof FormData)) {
    headers.set("Content-Type", "application/json");
  }

  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  const response = await fetch(`${apiUrl}${path}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    let message = `Request failed with status ${response.status}`;

    try {
      const body = (await response.json()) as ApiErrorResponse;

      if (typeof body.error === "string") {
        message = body.error;
      } else if (typeof body.detail === "string") {
        message = body.detail;
      }
    } catch {
      // The response did not contain JSON.
    }

    throw new ApiError(response.status, message);
  }

  return response;
}

export async function apiRequest<T>(
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const response = await sendRequest(path, options);

  return response.json() as Promise<T>;
}

export async function apiRequestWithoutResponse(
  path: string,
  options: RequestInit = {},
): Promise<void> {
  await sendRequest(path, options);
}
