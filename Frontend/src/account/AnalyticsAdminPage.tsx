import { useState } from "react";
import { DeleteUserAnalyticsSection } from "../analytics/components/DeleteUserAnalyticsSection";
import { LoginAnalyticsSection } from "../analytics/components/LoginAnalyticsSection";
import { ReferrerAnalyticsSection } from "../analytics/components/ReferrerAnalyticsSection";
import { RegistrationAnalyticsSection } from "../analytics/components/RegistrationAnalyticsSection";

export function AnalyticsAdminPage() {
  const [resetKey, setResetKey] = useState(0);

  function clearAllFilters() {
    setResetKey((value) => value + 1);
  }

  return (
    <section className="account-card analytics">
      <header className="account-card__header analytics-page-header">
        <div>
          <p className="account-card__eyebrow">Analytics</p>

          <h2>Site Activity</h2>

          <p className="account-management__description">
            Monitor authentication activity, user registrations and website
            usage.
          </p>
        </div>

        <button
          type="button"
          className="analytics-clear-button"
          onClick={clearAllFilters}
        >
          Clear all filters
        </button>
      </header>

      <LoginAnalyticsSection resetKey={resetKey} />

      <ReferrerAnalyticsSection resetKey={resetKey} />

      <RegistrationAnalyticsSection resetKey={resetKey} />

      <DeleteUserAnalyticsSection resetKey={resetKey} />
    </section>
  );
}
