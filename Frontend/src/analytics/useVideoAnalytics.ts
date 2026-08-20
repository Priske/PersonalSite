import { useQuery } from "@tanstack/react-query";
import { getVideoAnalytics } from "./analyticsApi";
import type { VideoAnalyticsRequest } from "./types";

export function useVideoAnalytics(request: VideoAnalyticsRequest) {
  return useQuery({
    queryKey: ["analytics", "videos", request],
    queryFn: () => getVideoAnalytics(request),
  });
}
