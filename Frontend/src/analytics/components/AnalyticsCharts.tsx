import { useId, useState } from "react";
import type { ChartPoint, Period } from "../dashboardHelpers";
import { fillDays } from "../dashboardHelpers";

export type BarValue = { label: string; values: number[] };

function linePattern(index: number): string {
  if (index === 0) return "none";
  if (index === 1) return "7 5";
  return `${2 + index * 3} 3 2 3`;
}

function seriesColor(index: number): string {
  return `var(--analytics-series-${index % 8})`;
}

export function BreakdownChart({ title, series, rows, format = String }: {
  title: string; series: string[]; rows: BarValue[]; format?: (value: number) => string;
}) {
  const max = rows.reduce((largest, row) => row.values.reduce((value, next) => Math.max(value, next), largest), 1);
  return <figure className="analytics-chart">
    <figcaption>{title}</figcaption>
    <div className="analytics-chart__categories" role="region" aria-label={`${title} categories`} tabIndex={0}>
      {rows.length === 0 ? <p>No matching activity.</p> : rows.map((row, rowIndex) => <div className="analytics-chart__category" key={`${row.label}-${rowIndex}`}>
        <strong>{row.label}</strong>
        {series.map((label, index) => <div className="analytics-chart__bar-row" key={label}>
          <span>{label}: {format(row.values[index] ?? 0)}</span>
          <div className={`analytics-chart__bar analytics-chart__series--${index}`} style={{ width: `${(row.values[index] ?? 0) / max * 100}%`, color: seriesColor(index) }} aria-hidden="true" />
        </div>)}
      </div>)}
    </div>
  </figure>;
}

export function TrendChart({ title, series, points, period, maxValue, format = String }: {
  title: string; series: string[]; points: ChartPoint[]; period: Period; maxValue?: number; format?: (value: number) => string;
}) {
  const id = useId();
  const [hiddenSources, setHiddenSources] = useState<Set<string>>(() => new Set());
  if (points.length === 0) return <figure className="analytics-chart"><figcaption>{title}</figcaption><p>No matching activity.</p></figure>;
  const visibleSeries = series.map((label, index) => ({ label, index })).filter(item => !hiddenSources.has(item.label));
  const days = fillDays(points, period);
  const max = days.reduce((largest, day) => visibleSeries.reduce((value, item) => Math.max(value, day.values[item.index] ?? 0), largest),
    visibleSeries.length === series.length ? Math.max(1, maxValue ?? 1) : 1);
  const activeDays = days.filter(day => visibleSeries.some(item => (day.values[item.index] ?? 0) !== 0));
  const x = (index: number) => days.length === 1 ? 330 : 60 + index / (days.length - 1) * 540;
  const y = (value: number) => 190 - value / max * 160;
  const ticks = new Set(Array.from({ length: Math.min(5, days.length) }, (_, i) =>
    Math.round(i / Math.max(1, Math.min(5, days.length) - 1) * (days.length - 1))));
  return <figure className="analytics-chart">
    <figcaption>{title}</figcaption>
    <div className="analytics-chart__legend" role="group" aria-label={`${title} visible sources`}>{series.map((label, i) => <button
      key={label}
      type="button"
      aria-pressed={!hiddenSources.has(label)}
      title={`${hiddenSources.has(label) ? "Show" : "Hide"} ${label}`}
      onClick={() => setHiddenSources(current => {
        const next = new Set(current);
        if (next.has(label)) next.delete(label);
        else next.add(label);
        return next;
      })}
    >
      <svg width="24" height="12" viewBox="0 0 24 12" style={{ color: seriesColor(i) }} aria-hidden="true" focusable="false">
        <path d="M0 6H24" pathLength={i === 0 ? 24 : linePattern(i).split(" ").reduce((length, value) => length + Number(value), 0)} stroke="currentColor" strokeWidth="2" style={{ strokeDasharray: linePattern(i) }} />
      </svg>
      <span>{label}</span>
    </button>)}</div>
    {visibleSeries.length === 0 ? <p role="status">Select a source to display</p> : <>
    <div className="analytics-chart__plot-scroll" role="region" aria-label={`${title} chart`} tabIndex={0}>
    <svg className="analytics-chart__plot" viewBox="0 0 660 245" role="img" aria-labelledby={`${id}-title ${id}-desc`}>
      <title id={`${id}-title`}>{title}</title>
      <desc id={`${id}-desc`}>Daily values from {days[0]?.date} to {days.at(-1)?.date}, including zero-activity days. Nonzero values are available in the data table below.</desc>
      <path d="M60 25V190H600" className="analytics-chart__axis" />
      <text x="52" y="35" textAnchor="end">{format(max)}</text><text x="52" y="194" textAnchor="end">0</text>
      {[...ticks].map(i => <text key={i} x={x(i)} y="220" textAnchor="middle">{days[i].date}</text>)}
      {visibleSeries.map(({ label, index }) => <g key={label} style={{ color: seriesColor(index) }}>
        <path className={`analytics-chart__line analytics-chart__series--${index}`} style={{ strokeDasharray: linePattern(index) }} d={days.map((day, i) => `${i === 0 ? "M" : "L"}${x(i)},${y(day.values[index] ?? 0)}`).join(" ")} />
        {days.length === 1 && <circle cx={x(0)} cy={y(days[0].values[index] ?? 0)} r="4" fill="currentColor" />}
      </g>)}
    </svg>
    </div>
    <details className="analytics-chart__data"><summary>Days with recorded activity</summary>
      {activeDays.length === 0 ? <p>No nonzero daily values in this period.</p> : <div className="analytics-chart__table-scroll" role="region" aria-label={`${title} days with recorded activity`} tabIndex={0}><table><caption>{title} — days with recorded activity</caption>
        <thead><tr><th scope="col">Date</th>{visibleSeries.map(({ label }) => <th scope="col" key={label}>{label}</th>)}</tr></thead>
        <tbody>{activeDays.map(day => <tr key={day.date}><th scope="row">{day.date}</th>{visibleSeries.map(({ label, index }) => <td key={label}>{format(day.values[index] ?? 0)}</td>)}</tr>)}</tbody>
      </table></div>}
    </details>
    </>}
  </figure>;
}
