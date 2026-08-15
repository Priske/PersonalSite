import { apiRequestWithoutResponse } from "../api";

import type { TrackActivityRequest } from "./types";

export function trackActivity(request: TrackActivityRequest) {
  return apiRequestWithoutResponse("/analytics", {
    method: "POST",
    body: JSON.stringify(request),
  });
}
