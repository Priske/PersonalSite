import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { getDeleteUserAnalytics } from "./analyticsApi";
import type { GetDeleteUserAnalyticsRequest } from "./types";

export function useDeleteUserAnalytics(request: GetDeleteUserAnalyticsRequest) {
  return useQuery({
    queryKey: ["analytics", "delete-user", request],
    queryFn: () => getDeleteUserAnalytics(request),
    placeholderData: keepPreviousData,
  });
}
