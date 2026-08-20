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

type VideoActivity = {
  featuredContentId: number;
  fileId: number;
  fileName: string;
  positionSeconds: number;
  durationSeconds: number;
};

export function trackVideoStarted(video: VideoActivity) {
  return trackActivity({
    type: "VideoStarted",
    metadata: [{ key: "Video", value: video }],
  });
}

export function trackVideoWatched(
  video: VideoActivity,
  watchedSeconds: number,
  reason: string,
) {
  return trackActivity({
    type: "VideoWatched",
    metadata: [
      {
        key: "Video",
        value: {
          ...video,
          watchedSeconds,
          reason,
        },
      },
    ],
  });
}

export function trackVideoCompleted(video: VideoActivity) {
  return trackActivity({
    type: "VideoCompleted",
    metadata: [{ key: "Video", value: video }],
  });
}
