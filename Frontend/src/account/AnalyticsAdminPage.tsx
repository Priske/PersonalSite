import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { getContactLinkAnalytics, getCreateUserAnalytics, getDeleteUserAnalytics, getLoginAnalytics, getRedirectAnalytics, getReferrerActivity, getVideoAnalytics } from "../analytics/analyticsApi";
import { AnalyticsPanel } from "../analytics/components/AnalyticsPanel";
import { ReferrerTrends } from "../analytics/components/ReferrerTrends";
import { ContactTrends, RedirectTrends, VideoTrends } from "../analytics/components/CategoryTrends";
import { BreakdownChart, TrendChart } from "../analytics/components/AnalyticsCharts";
import { ContactDetails, DeletionDetails, LoginDetails, RedirectDetails, ReferrerDetails, RegistrationDetails, VideoDetails } from "../analytics/components/AnalyticsDetails";
import { groupRedirectVisits } from "../analytics/components/redirectVisitGroups";
import { effectiveFilters, lastWeek, periodRequest, requireCategoryDailyData, requireDailyData, requireReferrerDailyData, validPeriod, watchedTime } from "../analytics/dashboardHelpers";
import type { AnalyticsView, Period, SectionFilters } from "../analytics/dashboardHelpers";
import type { DailyCount, DailyOutcome } from "../analytics/types";
import "../styles/pages/analytics-dashboard.css";

const sectionDefaults = {
  logins: "createdAt", referrers: "count", contacts: "clicks", redirects: "visits",
  videos: "watched", registrations: "createdAt", deletions: "createdAt",
} as const;
type Section = keyof typeof sectionDefaults;
type Filters = Record<Section, SectionFilters>;
function initialFilters(): Filters {
  const section = (sortBy: string): SectionFilters => ({ search: "", sortBy, descending: true, page: 1, pageSize: 5 });
  return {
    logins: section(sectionDefaults.logins), referrers: section(sectionDefaults.referrers),
    contacts: section(sectionDefaults.contacts), redirects: section(sectionDefaults.redirects),
    videos: section(sectionDefaults.videos), registrations: section(sectionDefaults.registrations),
    deletions: section(sectionDefaults.deletions),
  };
}
const counts = (daily: DailyCount[]) => daily.map(day => ({ date: day.date, values: [day.count] }));
const outcomes = (daily: DailyOutcome[]) => daily.map(day => ({ date: day.date, values: [day.successful, day.failed] }));

export function AnalyticsAdminPage() {
  const [period, setPeriod] = useState<Period>(lastWeek);
  const [filters, setFilters] = useState<Filters>(initialFilters);
  const [views, setViews] = useState<Record<Section, AnalyticsView>>({
    logins: "trend", referrers: "trend", contacts: "trend", redirects: "trend",
    videos: "trend", registrations: "trend", deletions: "trend",
  });
  const valid = validPeriod(period);
  const dates = valid ? periodRequest(period) : {};
  const request = (key: Section) => ({ ...dates, ...effectiveFilters(filters[key], views[key]) });
  const loginRequest = request("logins");
  const referrerRequest = request("referrers");
  const contactRequest = request("contacts");
  const redirectRequest = request("redirects");
  const videoRequest = request("videos");
  const registrationRequest = request("registrations");
  const deletionRequest = request("deletions");
  const logins = useQuery({ queryKey: ["analytics", "login", loginRequest], queryFn: () => getLoginAnalytics(loginRequest).then(requireDailyData), enabled: valid });
  const referrers = useQuery({ queryKey: ["analytics", "referrers", referrerRequest], queryFn: () => getReferrerActivity(referrerRequest).then(requireReferrerDailyData), enabled: valid });
  const contacts = useQuery({ queryKey: ["analytics", "contact-links", contactRequest], queryFn: () => getContactLinkAnalytics(contactRequest).then(data => requireCategoryDailyData(data, data.dailyByLink)), enabled: valid });
  const redirects = useQuery({ queryKey: ["analytics", "redirect-links", redirectRequest], queryFn: () => getRedirectAnalytics(redirectRequest).then(data => requireCategoryDailyData(data, data.dailyByLink)), enabled: valid });
  const videos = useQuery({ queryKey: ["analytics", "videos", videoRequest], queryFn: () => getVideoAnalytics(videoRequest).then(data => requireCategoryDailyData(data, data.dailyByVideo)), enabled: valid });
  const registrations = useQuery({ queryKey: ["analytics", "create-users", registrationRequest], queryFn: () => getCreateUserAnalytics(registrationRequest).then(requireDailyData), enabled: valid });
  const deletions = useQuery({ queryKey: ["analytics", "delete-users", deletionRequest], queryFn: () => getDeleteUserAnalytics(deletionRequest).then(requireDailyData), enabled: valid });

  function update(key: Section, changes: Partial<SectionFilters>) {
    setFilters(current => ({ ...current, [key]: { ...current[key], ...changes, page: Object.keys(changes).every(field => field === "page") ? changes.page ?? current[key].page : 1 } }));
  }
  function changePeriod(next: Period) {
    setPeriod(next);
    setFilters(current => Object.fromEntries(Object.entries(current).map(([key, value]) => [key, { ...value, page: 1 }])) as Filters);
  }
  function reset() { setPeriod(lastWeek()); setFilters(initialFilters()); }
  const common = (key: Section) => ({
    id: `analytics-${key}`, filters: filters[key], defaultSort: sectionDefaults[key],
    view: views[key], onViewChange: (view: AnalyticsView) => setViews(current => ({ ...current, [key]: view })),
    onChange: (changes: Partial<SectionFilters>) => update(key, changes),
  });
  const overview = [
    { key: "logins", label: "Login attempts", query: logins, value: logins.data?.summary.totalAttempts },
    { key: "referrers", label: "Page views", query: referrers, value: referrers.data?.matchingPageViews },
    { key: "contacts", label: "Contact clicks", query: contacts, value: contacts.data?.totalClicks },
    { key: "redirects", label: "Redirect visits", query: redirects, value: redirects.data?.totalVisits },
    { key: "videos", label: "Video plays", query: videos, value: videos.data?.totalPlays },
    { key: "registrations", label: "Registrations", query: registrations, value: registrations.data?.summary.totalCreatedUsers },
    { key: "deletions", label: "Deletion attempts", query: deletions, value: deletions.data?.summary.totalAttempts },
  ];
  const loginData = logins.data;
  const referrerData = referrers.data;
  const contactData = contacts.data;
  const redirectData = redirects.data;
  const videoData = videos.data;
  const registrationData = registrations.data;
  const deletionData = deletions.data;

  return <section className="account-card analytics">
    <header className="account-card__header analytics-page-header">
      <div><p className="account-card__eyebrow">Analytics</p><h2>Site Activity</h2><p className="account-management__description">Explore recorded activity, compare sources and follow daily trends.</p></div>
      <button type="button" className="analytics-clear-button" onClick={reset}>Reset filters</button>
    </header>
    <div className="analytics-period" role="group" aria-label="Analytics period">
      <label><span>Period</span><select value={period.mode} onChange={event => {
        const mode = event.target.value as Period["mode"];
        changePeriod(mode === "week" ? lastWeek() : { ...period, mode });
      }}><option value="week">Last 7 days</option><option value="custom">Custom dates</option><option value="all">All time</option></select></label>
      {period.mode === "custom" && <>
        <label><span>From</span><input type="date" value={period.from} onChange={event => changePeriod({ ...period, from: event.target.value })} /></label>
        <label><span>To</span><input type="date" value={period.to} onChange={event => changePeriod({ ...period, to: event.target.value })} /></label>
      </>}
      <p>{period.mode === "all" ? "All recorded dates" : `${period.from} – ${period.to}`} · {Intl.DateTimeFormat().resolvedOptions().timeZone}</p>
    </div>
    {!valid && <p role="alert">Choose valid start and end dates, with the end date on or after the start date. No analytics request will be sent until the range is valid.</p>}
    <div hidden={!valid}>
      <nav className="analytics-overview" aria-label="Analytics overview">
        {overview.map(item => <a key={item.key} href={`#analytics-${item.key}`}><span>{item.label}</span><strong>{item.query.isError ? "Unavailable" : item.query.isPending ? "Loading…" : item.value?.toLocaleString() ?? "Unavailable"}</strong></a>)}
      </nav>
      <p className="analytics-overview-note">Figures reflect the selected period and each section’s active filters. These are recorded events, not unique people.</p>

      <AnalyticsPanel {...common("logins")} title="Login activity" query={logins} metric={loginData?.summary.totalAttempts} status
        sortOptions={[{ value: "createdAt", label: "Date" }, { value: "userId", label: "User" }]} paging={loginData}
        details={loginData && <LoginDetails data={loginData} />}
        breakdown={loginData && <BreakdownChart title="Login outcomes" series={["Attempts"]} rows={[{ label: "Successful", values: [loginData.summary.successfulLogins] }, { label: "Failed", values: [loginData.summary.failedLogins] }]} />}
        trend={loginData && <TrendChart title="Daily login attempts" series={["Successful", "Failed"]} points={outcomes(loginData.daily)} period={period} />} />

      <AnalyticsPanel {...common("referrers")} title="Referrers" query={referrers} metric={referrerData?.matchingPageViews}
        sortOptions={[{ value: "count", label: "Views" }, { value: "referrer", label: "Referrer" }]}
        details={referrerData && <ReferrerDetails data={referrerData} />}
        breakdown={referrerData && <BreakdownChart title="Page views by referring website" series={["Views"]} rows={referrerData.referrers.map(item => ({ label: item.referrer === "Direct" ? "Referrer unavailable" : item.referrer, values: [item.count] }))} />}
        trend={referrerData && <ReferrerTrends data={referrerData} period={period} />} />

      <AnalyticsPanel {...common("contacts")} title="Contact clicks" query={contacts} metric={contactData?.totalClicks}
        sortOptions={[{ value: "clicks", label: "Clicks" }, { value: "label", label: "Link" }]}
        details={contactData && <ContactDetails data={contactData} />}
        breakdown={contactData && <BreakdownChart title="Clicks by contact link" series={["Clicks"]} rows={contactData.links.map(item => ({ label: item.label, values: [item.clicks] }))} />}
        trend={contactData && <ContactTrends data={contactData} period={period} />} />

      <AnalyticsPanel {...common("redirects")} title="Redirect visits" query={redirects} metric={redirectData?.totalVisits}
        sortOptions={[{ value: "visits", label: "Visits" }, { value: "slug", label: "Link" }]}
        details={redirectData && <RedirectDetails data={redirectData} sortBy={filters.redirects.sortBy} descending={filters.redirects.descending} />}
        breakdown={redirectData && <><p>Recorded visits may include bots and link previews; they are not verified QR scans or unique people.</p><BreakdownChart title="Visits by named redirect link" series={["Visits"]} rows={groupRedirectVisits(redirectData.links, filters.redirects.sortBy, filters.redirects.descending).map(item => ({ label: item.slug, values: [item.visits] }))} /></>}
        trend={redirectData && <><p>Recorded visits may include bots and link previews; they are not verified QR scans or unique people.</p><RedirectTrends data={redirectData} period={period} /></>} />

      <AnalyticsPanel {...common("videos")} title="Featured videos" query={videos} metric={videoData?.totalPlays}
        sortOptions={[{ value: "watched", label: "Watched time" }, { value: "plays", label: "Plays" }, { value: "completions", label: "Completions" }, { value: "name", label: "File" }]}
        details={videoData && <VideoDetails data={videoData} />}
        breakdown={videoData && <><BreakdownChart title="Video plays and completions" series={["Plays", "Completions"]} rows={videoData.videos.map(item => ({ label: item.fileName, values: [item.plays, item.completions] }))} /><BreakdownChart title="Watched duration by video" series={["Watched duration"]} format={watchedTime} rows={videoData.videos.map(item => ({ label: item.fileName, values: [item.watchedSeconds] }))} /></>}
        trend={videoData && <VideoTrends data={videoData} period={period} />} />

      <AnalyticsPanel {...common("registrations")} title="Registrations" query={registrations} metric={registrationData?.summary.totalCreatedUsers}
        sortOptions={[{ value: "createdAt", label: "Created" }, { value: "name", label: "Name" }, { value: "email", label: "Email" }]} paging={registrationData}
        details={registrationData && <RegistrationDetails data={registrationData} />}
        breakdown={registrationData && <><p>Registration records have no category grouping; this bar shows the matching total.</p><BreakdownChart title="Total registrations" series={["Registrations"]} rows={registrationData.totalItems ? [{ label: "Registrations", values: [registrationData.totalItems] }] : []} /></>}
        trend={registrationData && <TrendChart title="Daily registrations" series={["Registrations"]} points={counts(registrationData.daily)} period={period} />} />

      <AnalyticsPanel {...common("deletions")} title="User deletion attempts" query={deletions} metric={deletionData?.summary.totalAttempts} status
        sortOptions={[{ value: "createdAt", label: "Date" }, { value: "userId", label: "Actor" }, { value: "targetUserId", label: "Target" }]} paging={deletionData}
        details={deletionData && <DeletionDetails data={deletionData} />}
        breakdown={deletionData && <BreakdownChart title="Deletion outcomes" series={["Attempts"]} rows={[{ label: "Successful", values: [deletionData.summary.successfulDeletes] }, { label: "Failed", values: [deletionData.summary.failedDeletes] }]} />}
        trend={deletionData && <TrendChart title="Daily deletion attempts" series={["Successful", "Failed"]} points={outcomes(deletionData.daily)} period={period} />} />
    </div>
  </section>;
}
