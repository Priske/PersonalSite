import { useQuery } from "@tanstack/react-query";
import { getFeaturedContent } from "./featuredContentApi";

export function useFeaturedContent() {
  return useQuery({
    queryKey: ["featured-content", "official"],
    queryFn: getFeaturedContent,
  });
}
