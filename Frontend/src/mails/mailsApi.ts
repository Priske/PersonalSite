import { apiRequestWithoutResponse } from "../api";
import type { MakeContactRequest } from "./types";

export function makeContact(request: MakeContactRequest): Promise<void> {
  return apiRequestWithoutResponse("/contact", {
    method: "POST",
    body: JSON.stringify(request),
  });
}
