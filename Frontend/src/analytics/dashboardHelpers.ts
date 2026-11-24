import { toIsoDate } from "./components/analyticsHelpers";
import type { ReferrerAnalyticsResponse } from "./types";

export type Period = { mode: "week" | "custom" | "all"; from: string; to: string };
export type AnalyticsView = "details" | "breakdown" | "trend";
export type SectionFilters = {
  search: string;
  sortBy: string;
  descending: boolean;
  successful?: boolean;
  page: number;
  pageSize: number;
};

export function effectiveFilters(filters: SectionFilters, view: AnalyticsView) {
  return view === "trend"
    ? { ...filters, search: undefined, sortBy: undefined, descending: undefined, page: 1 }
    : { ...filters, search: filters.search || undefined };
}

export function dateInput(date: Date): string {
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}`;
}

export function lastWeek(now = new Date()): Period {
  const first = new Date(now.getFullYear(), now.getMonth(), now.getDate() - 6);
  return { mode: "week", from: dateInput(first), to: dateInput(now) };
}

export function validPeriod(period: Period): boolean {
  if (period.mode === "all") return true;
  const validDate = (value: string) => /^\d{4}-\d{2}-\d{2}$/.test(value) &&
    !Number.isNaN(new Date(`${value}T12:00:00`).getTime()) && dateInput(new Date(`${value}T12:00:00`)) === value;
  return validDate(period.from) && validDate(period.to) && period.from <= period.to;
}

export function periodRequest(period: Period) {
  return {
    from: period.mode === "all" ? undefined : toIsoDate(period.from),
    to: period.mode === "all" ? undefined : toIsoDate(period.to, true),
    timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
  };
}

export type ChartPoint = { date: string; values: number[] };

export function alignDailySeries(series: { date: string; value: number }[][]): ChartPoint[] {
  const valuesByDate = new Map<string, number[]>();
  series.forEach((days, index) => {
    days.forEach(day => {
      const values = valuesByDate.get(day.date) ?? Array<number>(series.length).fill(0);
      values[index] += day.value;
      valuesByDate.set(day.date, values);
    });
  });
  return [...valuesByDate.entries()]
    .sort(([left], [right]) => left.localeCompare(right))
    .map(([date, values]) => ({ date, values }));
}

export function requireDailyData<T extends { daily: unknown[] }>(data: T): T {
  // During a frontend/API version mismatch, show the section's load failure
  // instead of crashing the page or presenting missing aggregates as zero.
  if (!Array.isArray(data.daily)) throw new Error("Daily analytics data is unavailable.");
  return data;
}

export function requireReferrerDailyData(data: ReferrerAnalyticsResponse): ReferrerAnalyticsResponse {
  requireDailyData(data);
  if (!Array.isArray(data.dailyByReferrer)) throw new Error("Referrer trend data is unavailable.");
  return data;
}

export function requireCategoryDailyData<T extends { daily: unknown[] }>(data: T, categories: unknown): T {
  requireDailyData(data);
  if (!Array.isArray(categories)) throw new Error("Category trend data is unavailable.");
  return data;
}

// Called only for successful, complete aggregates. Advance calendar dates, not
// 24-hour durations, so daylight-saving transitions cannot skip or repeat a day.
export function fillDays(points: ChartPoint[], period: Period): ChartPoint[] {
  const start = period.mode === "all" ? points[0]?.date : period.from;
  if (!start) return [];
  const end = period.mode === "all" ? dateInput(new Date()) : period.to;
  const byDate = new Map(points.map(point => [point.date, point.values]));
  const width = points[0]?.values.length ?? 1;
  const result: ChartPoint[] = [];
  const cursor = new Date(`${start}T12:00:00`);
  while (dateInput(cursor) <= end) {
    const date = dateInput(cursor);
    result.push({ date, values: byDate.get(date) ?? Array<number>(width).fill(0) });
    cursor.setDate(cursor.getDate() + 1);
  }
  return result;
}

export function watchedTime(seconds: number): string {
  return `${seconds.toLocaleString(undefined, { maximumFractionDigits: 20 })} s`;
}
