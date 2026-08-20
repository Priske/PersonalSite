import { useQuery } from "@tanstack/react-query";
import { getContactLinkAnalytics } from "./analyticsApi";
import type { ContactLinkAnalyticsRequest } from "./types";

export function useContactLinkAnalytics(
  request: ContactLinkAnalyticsRequest,
) {
  return useQuery({
    queryKey: ["analytics", "contact-links", request],
    queryFn: () => getContactLinkAnalytics(request),
  });
}
