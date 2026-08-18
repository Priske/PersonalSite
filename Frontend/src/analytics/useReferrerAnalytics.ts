import { useQuery } from "@tanstack/react-query";
import { getReferrerActivity } from "./analyticsApi";
import type { ReferrerActivityRequest } from "./types";

export function useReferrerAnalytics(request: ReferrerActivityRequest) {
  return useQuery({
    queryKey: ["analytics", "referrer", request],
    queryFn: () => getReferrerActivity(request),
  });
}
