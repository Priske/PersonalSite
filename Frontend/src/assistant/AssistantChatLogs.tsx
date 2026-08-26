import { useState } from "react";
import { AnalyticsFilters } from "../analytics/components/AnalyticsFilters";
import { AnalyticsPagination } from "../analytics/components/AnalyticsPagination";
import { toIsoDate } from "../analytics/components/analyticsHelpers";
import { useAssistantChatLogs } from "./useAssistantChatLogs";

export function AssistantChatLogs() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [search, setSearch] = useState("");
  const [userId, setUserId] = useState("");
  const [sortBy, setSortBy] = useState("createdAt");
  const [descending, setDescending] = useState(true);
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");

  const parsedUserId =
    userId.trim() === "" ? undefined : Number.parseInt(userId, 10);

  const analyticsQuery = useAssistantChatLogs({
    userId:
      parsedUserId !== undefined && !Number.isNaN(parsedUserId)
        ? parsedUserId
        : undefined,
    search: search.trim() || undefined,
    from: toIsoDate(from),
    to: toIsoDate(to, true),
    sortBy,
    descending,
    page,
    pageSize,
  });

  if (analyticsQuery.isPending) {
    return (
      <section className="analytics-section assistant-chat-logs">
        <p>Loading assistant chat logs...</p>
      </section>
    );
  }

  if (analyticsQuery.isError) {
    return (
      <section className="analytics-section assistant-chat-logs">
        <p className="form-message form-message--error">
          Could not load assistant chat logs: {analyticsQuery.error.message}
        </p>
      </section>
    );
  }

  const analytics = analyticsQuery.data;

  return (
    <section className="assistant-chat-logs">
      <div className="analytics-summary assistant-chat-logs__summary">
        <div className="analytics-summary__item">
          <span>Total chats</span>
          <strong>{analytics.summary.totalChats}</strong>
        </div>

        <div className="analytics-summary__item">
          <span>Authenticated</span>
          <strong>{analytics.summary.authenticatedChats}</strong>
        </div>

        <div className="analytics-summary__item">
          <span>Anonymous</span>
          <strong>{analytics.summary.anonymousChats}</strong>
        </div>
      </div>

      <div className="analytics-section assistant-chat-logs__content">
        <div className="analytics-section__header">
          <div>
            <p className="account-card__eyebrow">Assistant</p>
            <h3>Chat logs</h3>
          </div>

          <span>{analytics.totalItems} conversations</span>
        </div>

        <AnalyticsFilters
          search={search}
          searchPlaceholder="Search questions and answers..."
          sortBy={sortBy}
          sortOptions={[
            {
              value: "createdAt",
              label: "Date",
            },
            {
              value: "userId",
              label: "User",
            },
          ]}
          descending={descending}
          from={from}
          to={to}
          onSearchChange={(value) => {
            setSearch(value);
            setPage(1);
          }}
          onSortByChange={(value) => {
            setSortBy(value);
            setPage(1);
          }}
          onDescendingChange={(value) => {
            setDescending(value);
            setPage(1);
          }}
          onFromChange={(value) => {
            setFrom(value);
            setPage(1);
          }}
          onToChange={(value) => {
            setTo(value);
            setPage(1);
          }}
        >
          <label className="analytics-filter">
            <span>User ID</span>

            <input
              type="number"
              min="1"
              step="1"
              value={userId}
              placeholder="All users"
              onChange={(event) => {
                setUserId(event.target.value);
                setPage(1);
              }}
            />
          </label>
        </AnalyticsFilters>

        {analytics.items.length === 0 ? (
          <div className="assistant-chat-logs__empty">
            <strong>No chat logs found</strong>

            <p>There are no conversations matching the current filters.</p>
          </div>
        ) : (
          <div className="assistant-chat-logs__list">
            {analytics.items.map((activity) => (
              <article className="assistant-chat-log" key={activity.id}>
                <header className="assistant-chat-log__header">
                  <div>
                    <span className="assistant-chat-log__identity">
                      {activity.userId === null
                        ? "Anonymous visitor"
                        : `User #${activity.userId}`}
                    </span>

                    <span className="assistant-chat-log__id">
                      Chat #{activity.id}
                    </span>
                  </div>

                  <time dateTime={activity.createdAt}>
                    {new Date(activity.createdAt).toLocaleString()}
                  </time>
                </header>

                <div className="assistant-chat-log__body">
                  <section className="assistant-chat-log__message">
                    <span className="assistant-chat-log__label">Question</span>

                    <p>{activity.question}</p>
                  </section>

                  <section className="assistant-chat-log__message assistant-chat-log__message--answer">
                    <span className="assistant-chat-log__label">Answer</span>

                    <p>{activity.answer}</p>
                  </section>
                </div>
              </article>
            ))}
          </div>
        )}

        <AnalyticsPagination
          page={analytics.page}
          pageSize={analytics.pageSize}
          totalItems={analytics.totalItems}
          totalPages={analytics.totalPages}
          onPageChange={setPage}
          onPageSizeChange={(value) => {
            setPageSize(value);
            setPage(1);
          }}
        />
      </div>
    </section>
  );
}
