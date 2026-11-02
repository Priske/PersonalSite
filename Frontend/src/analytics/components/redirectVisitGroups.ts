import type { RedirectAnalyticsItem } from "../types";

export type RedirectVisitGroup = {
  slug: string;
  visits: number;
  details: RedirectAnalyticsItem[];
};

export function groupRedirectVisits(
  items: readonly RedirectAnalyticsItem[],
  sortBy: string,
  descending: boolean,
): RedirectVisitGroup[] {
  const groups = new Map<string, RedirectVisitGroup>();

  for (const item of items) {
    let group = groups.get(item.slug);
    if (!group) {
      group = { slug: item.slug, visits: 0, details: [] };
      groups.set(item.slug, group);
    }
    group.visits += item.visits;
    group.details.push(item);
  }

  for (const group of groups.values()) {
    group.details.sort((left, right) =>
      right.visits - left.visits ||
      (left.campaign ?? "").localeCompare(right.campaign ?? "") ||
      left.referrer.localeCompare(right.referrer),
    );
  }

  // The API sorts individual breakdown rows; sort again by the displayed totals.
  return [...groups.values()].sort((left, right) => {
    const order = sortBy === "slug"
      ? left.slug.localeCompare(right.slug)
      : left.visits - right.visits;
    return (descending ? -order : order) || left.slug.localeCompare(right.slug);
  });
}
