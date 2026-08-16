import { useEffect } from "react";
import { trackPageView } from "./analytics";

export function SiteEntryTracker() {
  useEffect(() => {
    if (window.location.pathname === "/") {
      void trackPageView("homepage");
    }
  }, []);

  return null;
}
