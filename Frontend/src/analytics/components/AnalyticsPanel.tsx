import type { ReactNode } from "react";
import type { UseQueryResult } from "@tanstack/react-query";
import type { AnalyticsView, SectionFilters } from "../dashboardHelpers";
import { AnalyticsFilters } from "./AnalyticsFilters";
import type { SortOption } from "./AnalyticsFilters";
import { AnalyticsPagination } from "./AnalyticsPagination";

export function AnalyticsPanel({ id, title, metric, query, filters, defaultSort, sortOptions, status = false, view, onViewChange, onChange, details, breakdown, trend, paging }: {
  id: string; title: string; metric?: number;
  query: Pick<UseQueryResult, "isPending" | "isError" | "isFetching">;
  filters: SectionFilters; defaultSort: string; sortOptions: SortOption[]; status?: boolean;
  view: AnalyticsView; onViewChange: (view: AnalyticsView) => void;
  onChange: (changes: Partial<SectionFilters>) => void;
  details: ReactNode; breakdown: ReactNode; trend: ReactNode;
  paging?: { page: number; pageSize: number; totalItems: number; totalPages: number };
}) {
  const active = filters.successful !== undefined || (view !== "trend" && Boolean(filters.search || filters.sortBy !== defaultSort || !filters.descending));
  const statusControl = status && <label className="analytics-filter"><span>Status</span><select value={filters.successful === undefined ? "all" : filters.successful ? "successful" : "failed"} onChange={event => onChange({ successful: event.target.value === "all" ? undefined : event.target.value === "successful" })}>
    <option value="all">All</option><option value="successful">Successful</option><option value="failed">Failed</option>
  </select></label>;
  return <section id={id} className="analytics-panel" aria-labelledby={`${id}-heading`}>
    <header className="analytics-section__header">
      <h3 id={`${id}-heading`}>{title}</h3>
      <span>{query.isError ? "Unavailable" : query.isPending ? "Loading…" : `${metric ?? 0} matching`}</span>
    </header>
    <div className="analytics-view-control" role="group" aria-label={`${title} view`}>
      {(["details", "breakdown", "trend"] as const).map(value => <button type="button" key={value} aria-pressed={view === value} onClick={() => onViewChange(value)}>{value === "details" ? "Details" : value === "breakdown" ? "Breakdown" : "Trend"}</button>)}
    </div>
    {(view !== "trend" || status) && <details className="analytics-filter-disclosure"><summary>Filters{active ? " · Active" : ""}</summary>
      {view === "trend" ? <div className="analytics-filters">{statusControl}</div> : <AnalyticsFilters search={filters.search} searchPlaceholder={`Search ${title.toLowerCase()}…`} sortBy={filters.sortBy} sortOptions={sortOptions} descending={filters.descending}
        from="" to="" showDates={false} onFromChange={() => {}} onToChange={() => {}}
        onSearchChange={search => onChange({ search })} onSortByChange={sortBy => onChange({ sortBy })} onDescendingChange={descending => onChange({ descending })}>
        {statusControl}
      </AnalyticsFilters>}
    </details>}
    <div aria-busy={query.isFetching}>
      {query.isError ? <p role="status">Could not load {title.toLowerCase()}. This does not mean there was no activity.</p> : query.isPending ? <p role="status">Loading {title.toLowerCase()}…</p> : <>
        {view === "details" ? details : view === "breakdown" ? breakdown : trend}
        {view === "details" && paging && <AnalyticsPagination {...paging} onPageChange={page => onChange({ page })} onPageSizeChange={pageSize => onChange({ pageSize })} />}
      </>}
    </div>
  </section>;
}
