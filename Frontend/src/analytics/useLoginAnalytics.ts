import { useQuery } from "@tanstack/react-query";
import { getLoginAnalytics } from "./analyticsApi";
import type { GetLoginAnalyticsRequest } from "./types";

export function useLoginAnalytics(request: GetLoginAnalyticsRequest) {
  return useQuery({
    queryKey: ["analytics", "login", request],
    queryFn: () => getLoginAnalytics(request),
  });
}
