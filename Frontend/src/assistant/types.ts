export type AskQuestionRequest = {
  question: string;
};

export type AskQuestionResponse = {
  answer: string;
};

export type GetAssistantChatAnalyticsRequest = {
  userId?: number;
  search?: string;
  from?: string;
  to?: string;
  sortBy?: string;
  descending?: boolean;
  page?: number;
  pageSize?: number;
};

export type AssistantChatActivity = {
  id: number;
  userId: number | null;
  question: string;
  answer: string;
  createdAt: string;
};

export type AssistantChatAnalyticsSummary = {
  totalChats: number;
  authenticatedChats: number;
  anonymousChats: number;
};

export type AssistantChatAnalyticsResponse = {
  summary: AssistantChatAnalyticsSummary;
  items: AssistantChatActivity[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};
