import { useEffect, useState } from "react";
import { useVideoAnalytics } from "../useVideoAnalytics";
import { AnalyticsFilters } from "./AnalyticsFilters";
import { toIsoDate } from "./analyticsHelpers";

type VideoAnalyticsSectionProps = {
  resetKey: number;
};

function formatWatchedTime(seconds: number) {
  const roundedSeconds = Math.round(seconds);
  const minutes = Math.floor(roundedSeconds / 60);
  const remainingSeconds = roundedSeconds % 60;

  return `${minutes}m ${remainingSeconds}s`;
}

export function VideoAnalyticsSection({
  resetKey,
}: VideoAnalyticsSectionProps) {
  const [search, setSearch] = useState("");
  const [sortBy, setSortBy] = useState("watched");
  const [descending, setDescending] = useState(true);
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");

  useEffect(() => {
    setSearch("");
    setSortBy("watched");
    setDescending(true);
    setFrom("");
    setTo("");
  }, [resetKey]);

  const analyticsQuery = useVideoAnalytics({
    search: search || undefined,
    from: toIsoDate(from),
    to: toIsoDate(to, true),
    sortBy,
    descending,
  });

  if (analyticsQuery.isPending) {
    return <div className="analytics-section">Loading video analytics...</div>;
  }

  if (analyticsQuery.isError) {
    return <div className="analytics-section">Could not load video analytics.</div>;
  }

  const analytics = analyticsQuery.data;

  return (
    <div className="analytics-section analytics-section--referrers">
      <div className="analytics-section__header">
        <div>
          <p className="account-card__eyebrow">Engagement</p>
          <h3>Featured videos</h3>
        </div>
      </div>

      <div className="analytics-summary analytics-summary--videos">
        <div className="analytics-summary__item">
          <span>Plays</span>
          <strong>{analytics.totalPlays}</strong>
        </div>
        <div className="analytics-summary__item">
          <span>Completed</span>
          <strong>{analytics.totalCompletions}</strong>
        </div>
        <div className="analytics-summary__item">
          <span>Watched</span>
          <strong>{formatWatchedTime(analytics.totalWatchedSeconds)}</strong>
        </div>
      </div>

      <AnalyticsFilters
        search={search}
        searchPlaceholder="Search video..."
        sortBy={sortBy}
        sortOptions={[
          { value: "watched", label: "Watched time" },
          { value: "plays", label: "Plays" },
          { value: "completions", label: "Completions" },
          { value: "name", label: "File" },
        ]}
        descending={descending}
        from={from}
        to={to}
        onSearchChange={setSearch}
        onSortByChange={setSortBy}
        onDescendingChange={setDescending}
        onFromChange={setFrom}
        onToChange={setTo}
      />

      <div className="analytics-videos">
        <div className="analytics-videos__header">
          <span>Video</span>
          <span>Plays</span>
          <span>Completed</span>
          <span>Watched</span>
        </div>

        {analytics.videos.map((video) => (
          <div className="analytics-videos__row" key={video.fileId}>
            <span title={video.fileName}>{video.fileName}</span>
            <strong>{video.plays}</strong>
            <strong>{video.completions}</strong>
            <strong>{formatWatchedTime(video.watchedSeconds)}</strong>
          </div>
        ))}
      </div>
    </div>
  );
}
