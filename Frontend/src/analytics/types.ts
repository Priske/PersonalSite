export type ActivityType =
  | "PageViewed"
  | "LinkClicked"
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
  summary: LoginAnalyticsSummary;
  items: LoginActivityResponse[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};

export type ReferrerActivityRequest = {
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
  totalPageViews: number;
  referrers: ReferrerAnalyticsItem[];
};

export type GetCreateUserAnalyticsRequest = {
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
  summary: CreateUserAnalyticsSummary;
  items: CreateUserActivityResponse[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};
export type GetDeleteUserAnalyticsRequest = {
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
  summary: DeleteUserAnalyticsSummary;
  items: DeleteUserActivityResponse[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};
