export type ActivityType =
  | "PageViewed"
  | "LinkClicked"
  | "VideoStarted"
  | "VideoWatched"
  | "VideoCompleted"
  | "User_Registered"
  | "Login"
  | "Logout"
  | "DemoHomepageUpdated"
  | "DemoProjectCreated"
  | "DemoProjectUpdated"
  | "DemoProjectDeleted";

export type MetadataValue =
  | string
  | number
  | boolean
  | {
      [key: string]: MetadataValue;
    };

export type ActivityMetadataRequest = {
  key: string;
  value: MetadataValue;
};

export type TrackActivityRequest = {
  type: ActivityType;
  metadata: ActivityMetadataRequest[];
};

export type GetLoginAnalyticsRequest = {
  timeZone?: string;
  userId?: number;
  search?: string;
  successful?: boolean;
  from?: string;
  to?: string;
  sortBy?: string;
  descending?: boolean;
  page?: number;
  pageSize?: number;
};

export type LoginActivityResponse = {
  id: number;
  userId: number | null;
  createdAt: string;
  successful: boolean;
  failureReason: string | null;
};

export type LoginAnalyticsSummary = {
  totalAttempts: number;
  successfulLogins: number;
  failedLogins: number;
  unknownEmailAttempts: number;
  incorrectPasswordAttempts: number;
};

export type LoginAnalyticsResponse = {
  daily: DailyOutcome[];
  summary: LoginAnalyticsSummary;
  items: LoginActivityResponse[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};

export type ReferrerActivityRequest = {
  timeZone?: string;
  search?: string;
  from?: string;
  to?: string;
  sortBy?: string;
  descending?: boolean;
};
export type ReferrerAnalyticsItem = {
  referrer: string;
  count: number;
};

export type ReferrerAnalyticsResponse = {
  dailyByReferrer: { referrer: string; daily: DailyCount[] }[];
  matchingPageViews: number;
  daily: DailyCount[];
  totalPageViews: number;
  referrers: ReferrerAnalyticsItem[];
};

export type ContactLinkAnalyticsRequest = ReferrerActivityRequest;

export type RedirectAnalyticsItem = {
  slug: string;
  campaign: string | null;
  referrer: string;
  visits: number;
};

export type RedirectAnalyticsResponse = {
  dailyByLink: { slug: string; daily: DailyCount[] }[];
  daily: DailyCount[];
  totalVisits: number;
  links: RedirectAnalyticsItem[];
};

export type ContactLinkAnalyticsItem = {
  label: string;
  clicks: number;
};

export type ContactLinkAnalyticsResponse = {
  dailyByLink: { label: string; daily: DailyCount[] }[];
  daily: DailyCount[];
  totalClicks: number;
  links: ContactLinkAnalyticsItem[];
};

export type VideoAnalyticsRequest = ReferrerActivityRequest;

export type VideoAnalyticsItem = {
  featuredContentId: number;
  fileId: number;
  fileName: string;
  plays: number;
  completions: number;
  watchedSeconds: number;
};

export type VideoAnalyticsResponse = {
  dailyByVideo: { featuredContentId: number; fileId: number; fileName: string; daily: DailyVideo[] }[];
  daily: DailyVideo[];
  totalPlays: number;
  totalCompletions: number;
  totalWatchedSeconds: number;
  videos: VideoAnalyticsItem[];
};

export type GetCreateUserAnalyticsRequest = {
  timeZone?: string;
  search?: string;
  from?: string;
  to?: string;
  sortBy?: string;
  descending?: boolean;
  page?: number;
  pageSize?: number;
};

export type CreateUserActivityResponse = {
  id: number;
  userId: number | null;
  name: string | null;
  email: string | null;
  createdAt: string;
};

export type CreateUserAnalyticsSummary = {
  totalCreatedUsers: number;
};

export type CreateUserAnalyticsResponse = {
  daily: DailyCount[];
  summary: CreateUserAnalyticsSummary;
  items: CreateUserActivityResponse[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};
export type GetDeleteUserAnalyticsRequest = {
  timeZone?: string;
  userId?: number;
  search?: string;
  successful?: boolean;
  from?: string;
  to?: string;
  sortBy?: string;
  descending?: boolean;
  page?: number;
  pageSize?: number;
};

export type DeleteUserActivityResponse = {
  id: number;
  userId: number | null;
  targetUserId: number | null;
  createdAt: string;
  successful: boolean;
  failureReason: string | null;
};

export type DeleteUserAnalyticsSummary = {
  totalAttempts: number;
  successfulDeletes: number;
  failedDeletes: number;
};

export type DeleteUserAnalyticsResponse = {
  daily: DailyOutcome[];
  summary: DeleteUserAnalyticsSummary;
  items: DeleteUserActivityResponse[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};

export type DailyCount = { date: string; count: number };
export type DailyOutcome = { date: string; successful: number; failed: number };
export type DailyVideo = { date: string; plays: number; completions: number; watchedSeconds: number };
