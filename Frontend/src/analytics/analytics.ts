import { trackActivity } from "./analyticsApi";

export function trackPageView(path: string) {
  return trackActivity({
    type: "PageViewed",
    metadata: [
      {
        key: "Page",
        value: {
          path,
          referrer: document.referrer,
        },
      },
    ],
  });
}

export function trackLinkClick(
  label: string,
  destination: string,
  section: string,
) {
  return trackActivity({
    type: "LinkClicked",
    metadata: [
      {
        key: "Link",
        value: {
          label,
          destination,
          section,
          page: location.pathname,
        },
      },
    ],
  });
}
