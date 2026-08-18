import type { ReactNode } from "react";

export type SortOption = {
  value: string;
  label: string;
};

type AnalyticsFiltersProps = {
  search: string;
  searchPlaceholder: string;
  sortBy: string;
  sortOptions: SortOption[];
  descending: boolean;
  from: string;
  to: string;
  onSearchChange: (value: string) => void;
  onSortByChange: (value: string) => void;
  onDescendingChange: (value: boolean) => void;
  onFromChange: (value: string) => void;
  onToChange: (value: string) => void;
  children?: ReactNode;
};

export function AnalyticsFilters({
  search,
  searchPlaceholder,
  sortBy,
  sortOptions,
  descending,
  from,
  to,
  onSearchChange,
  onSortByChange,
  onDescendingChange,
  onFromChange,
  onToChange,
  children,
}: AnalyticsFiltersProps) {
  return (
    <div className="analytics-filters">
      <label className="analytics-filter analytics-filter--search">
        <span>Search</span>

        <input
          type="search"
          value={search}
          placeholder={searchPlaceholder}
          onChange={(event) => onSearchChange(event.target.value)}
        />
      </label>

      {children}

      <label className="analytics-filter">
        <span>From</span>

        <input
          type="date"
          value={from}
          onChange={(event) => onFromChange(event.target.value)}
        />
      </label>

      <label className="analytics-filter">
        <span>To</span>

        <input
          type="date"
          value={to}
          onChange={(event) => onToChange(event.target.value)}
        />
      </label>

      <label className="analytics-filter">
        <span>Sort by</span>

        <select
          value={sortBy}
          onChange={(event) => onSortByChange(event.target.value)}
        >
          {sortOptions.map((option) => (
            <option value={option.value} key={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </label>

      <label className="analytics-filter">
        <span>Order</span>

        <select
          value={descending ? "descending" : "ascending"}
          onChange={(event) =>
            onDescendingChange(event.target.value === "descending")
          }
        >
          <option value="descending">Descending</option>
          <option value="ascending">Ascending</option>
        </select>
      </label>
    </div>
  );
}
