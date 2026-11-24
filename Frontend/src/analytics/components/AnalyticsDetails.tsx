import type { ReactNode } from "react";
import type { ContactLinkAnalyticsResponse, CreateUserAnalyticsResponse, DeleteUserAnalyticsResponse, LoginAnalyticsResponse, RedirectAnalyticsResponse, ReferrerAnalyticsResponse, VideoAnalyticsResponse } from "../types";
import { formatDeleteFailureReason, formatFailureReason } from "./analyticsHelpers";
import { groupRedirectVisits } from "./redirectVisitGroups";
import { watchedTime } from "../dashboardHelpers";

function DetailTable({ caption, columns, rows }: { caption: string; columns: string[]; rows: ReactNode[][] }) {
  if (rows.length === 0) return <p>No matching activity.</p>;
  return <div className="analytics-chart__table-scroll" role="region" aria-label={caption} tabIndex={0}><table className="analytics-detail-table">
    <caption>{caption}</caption>
    <thead><tr>{columns.map(column => <th scope="col" key={column}>{column}</th>)}</tr></thead>
    <tbody>{rows.map((row, index) => <tr key={index}>{row.map((value, i) => <td key={columns[i]}>{value}</td>)}</tr>)}</tbody>
  </table></div>;
}

const userLabel = (id: number | null) => id === null ? "Unknown" : `#${id}`;

export function LoginDetails({ data }: { data: LoginAnalyticsResponse }) {
  return <>
    <p>{data.summary.successfulLogins} successful; {data.summary.failedLogins} failed; {data.summary.unknownEmailAttempts} unknown email; {data.summary.incorrectPasswordAttempts} incorrect password.</p>
    <DetailTable caption="Login activity" columns={["Status", "User", "Reason", "Date"]} rows={data.items.map(item => [item.successful ? "Successful" : "Failed", userLabel(item.userId), formatFailureReason(item.failureReason), new Date(item.createdAt).toLocaleString()])} />
  </>;
}

export function ReferrerDetails({ data }: { data: ReferrerAnalyticsResponse }) {
  return <DetailTable caption="Page views by referrer" columns={["Referrer", "Views"]} rows={data.referrers.map(item => [item.referrer === "Direct" ? "Referrer unavailable" : item.referrer, item.count])} />;
}

export function ContactDetails({ data }: { data: ContactLinkAnalyticsResponse }) {
  return <DetailTable caption="Contact-link clicks" columns={["Contact link", "Clicks"]} rows={data.links.map(item => [item.label, item.clicks])} />;
}

export function VideoDetails({ data }: { data: VideoAnalyticsResponse }) {
  return <>
    <p>{data.totalPlays} plays; {data.totalCompletions} completions; {watchedTime(data.totalWatchedSeconds)} watched.</p>
    <DetailTable caption="Featured video activity" columns={["Video", "Plays", "Completed", "Watched"]} rows={data.videos.map(item => [item.fileName, item.plays, item.completions, watchedTime(item.watchedSeconds)])} />
  </>;
}

export function RegistrationDetails({ data }: { data: CreateUserAnalyticsResponse }) {
  return <DetailTable caption="Registrations" columns={["User", "Name", "Email", "Created"]} rows={data.items.map(item => [userLabel(item.userId), item.name ?? "—", item.email ?? "—", new Date(item.createdAt).toLocaleString()])} />;
}

export function DeletionDetails({ data }: { data: DeleteUserAnalyticsResponse }) {
  return <>
    <p>{data.summary.successfulDeletes} successful; {data.summary.failedDeletes} failed.</p>
    <DetailTable caption="User deletion attempts" columns={["Status", "Actor", "Target", "Reason", "Date"]} rows={data.items.map(item => [item.successful ? "Deleted" : "Failed", userLabel(item.userId), userLabel(item.targetUserId), formatDeleteFailureReason(item.failureReason), new Date(item.createdAt).toLocaleString()])} />
  </>;
}

function website(value: string): string | null {
  try {
    const url = new URL(value);
    return ["http:", "https:"].includes(url.protocol) && url.hostname ? value : null;
  } catch { return null; }
}

export function RedirectDetails({ data, sortBy, descending }: { data: RedirectAnalyticsResponse; sortBy: string; descending: boolean }) {
  const groups = groupRedirectVisits(data.links, sortBy, descending);
  return <>
    <p>Visits may include bots and link previews; they are not verified QR scans, unique people or email opens.</p>
    {groups.length === 0 ? <p>No redirect visits match these filters.</p> : <div className="redirect-visits-list">
      <div className="redirect-visits-columns" aria-hidden="true"><span>Link</span><span>Visits</span><span /></div>
      {groups.map(group => <details className="redirect-visits-link" key={group.slug}>
        <summary aria-label={`${group.slug}: ${group.visits} visits. Show campaign and referrer details.`}>
          <span className="redirect-visits-name"><strong>{group.slug}</strong></span>
          <span className="redirect-visits-count"><strong>{group.visits}</strong></span>
          <span className="redirect-visits-toggle" aria-hidden="true" />
        </summary>
        <div className="redirect-visits-breakdown"><h4>Entry link: <code>/r/{group.slug}</code></h4>
          <p className="redirect-visits-detail-help">The campaign label is the optional label you assigned to this link.</p>
          <ul>{group.details.map(detail => <li key={JSON.stringify([detail.campaign, detail.referrer])}><dl>
            <div><dt>Campaign label</dt><dd>{detail.campaign ?? "Not set"}</dd></div>
            {website(detail.referrer) && <div><dt>Referring website</dt><dd>{detail.referrer}</dd></div>}
            <div className="redirect-visits-detail-count"><dt>Recorded visits</dt><dd>{detail.visits}</dd></div>
          </dl></li>)}</ul>
        </div>
      </details>)}
    </div>}
  </>;
}
