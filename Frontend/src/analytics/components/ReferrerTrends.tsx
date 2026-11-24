import type { ReferrerAnalyticsResponse } from "../types";
import type { Period } from "../dashboardHelpers";
import { alignDailySeries } from "../dashboardHelpers";
import { TrendChart } from "./AnalyticsCharts";

export function ReferrerTrends({ data, period }: { data: ReferrerAnalyticsResponse; period: Period }) {
  if (!Array.isArray(data.dailyByReferrer)) return <p role="status">Referrer trends are temporarily unavailable.</p>;
  if (data.dailyByReferrer.length === 0) return <p>No matching page views.</p>;
  const points = alignDailySeries(data.dailyByReferrer.map(referrer =>
    referrer.daily.map(day => ({ date: day.date, value: day.count }))));

  return <TrendChart
    title="Daily page views by referrer"
    series={data.dailyByReferrer.map(referrer => referrer.referrer === "Direct" ? "Referrer unavailable" : referrer.referrer)}
    points={points}
    period={period}
  />;
}
