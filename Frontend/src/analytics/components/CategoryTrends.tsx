import type { ContactLinkAnalyticsResponse, RedirectAnalyticsResponse, VideoAnalyticsResponse } from "../types";
import type { Period } from "../dashboardHelpers";
import { alignDailySeries } from "../dashboardHelpers";
import { TrendChart } from "./AnalyticsCharts";

export function ContactTrends({ data, period }: { data: ContactLinkAnalyticsResponse; period: Period }) {
  if (!Array.isArray(data.dailyByLink)) return <p role="status">Contact-link trends are temporarily unavailable.</p>;
  return <TrendChart
    title="Daily clicks by contact link"
    series={data.dailyByLink.map(link => link.label)}
    points={alignDailySeries(data.dailyByLink.map(link => link.daily.map(day => ({ date: day.date, value: day.count }))))}
    period={period}
  />;
}

export function RedirectTrends({ data, period }: { data: RedirectAnalyticsResponse; period: Period }) {
  if (!Array.isArray(data.dailyByLink)) return <p role="status">Redirect trends are temporarily unavailable.</p>;
  return <TrendChart
    title="Daily visits by redirect link"
    series={data.dailyByLink.map(link => link.slug)}
    points={alignDailySeries(data.dailyByLink.map(link => link.daily.map(day => ({ date: day.date, value: day.count }))))}
    period={period}
  />;
}

export function VideoTrends({ data, period }: { data: VideoAnalyticsResponse; period: Period }) {
  if (!Array.isArray(data.dailyByVideo)) return <p role="status">Video trends are temporarily unavailable.</p>;
  const nameCounts = new Map<string, number>();
  const occurrences = new Map<string, number>();
  data.dailyByVideo.forEach(video => nameCounts.set(video.fileName, (nameCounts.get(video.fileName) ?? 0) + 1));
  const labels = data.dailyByVideo.map(video => {
    const occurrence = (occurrences.get(video.fileName) ?? 0) + 1;
    occurrences.set(video.fileName, occurrence);
    return nameCounts.get(video.fileName)! > 1 ? `${video.fileName} (${occurrence})` : video.fileName;
  });
  const plays = alignDailySeries(data.dailyByVideo.map(video => video.daily.map(day => ({ date: day.date, value: day.plays }))));
  return <TrendChart title="Daily plays by video" series={labels} points={plays} period={period} />;
}
