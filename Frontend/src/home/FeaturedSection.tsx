import { useCallback, useEffect, useRef, useState } from "react";
import { apiPath } from "../api";
import {
  trackVideoCompleted,
  trackVideoStarted,
  trackVideoWatched,
} from "../analytics/analytics";
import { useFeaturedContent } from "../featured/useFeaturedContent";
import type { FeaturedContentFile } from "../featured/types";

type FeaturedVideoProps = {
  featuredContentId: number;
  file: FeaturedContentFile;
};

function seconds(value: number) {
  return Number.isFinite(value)
    ? Number(Math.max(value, 0).toFixed(2))
    : 0;
}

function TrackedFeaturedVideo({
  featuredContentId,
  file,
}: FeaturedVideoProps) {
  const videoRef = useRef<HTMLVideoElement>(null);
  const playingSinceRef = useRef<number | null>(null);
  const startedRef = useRef(false);

  const getVideoActivity = useCallback(() => {
    const video = videoRef.current;

    return {
      featuredContentId,
      fileId: file.id,
      fileName: file.originalFileName,
      positionSeconds: seconds(video?.currentTime ?? 0),
      durationSeconds: seconds(video?.duration ?? 0),
    };
  }, [featuredContentId, file.id, file.originalFileName]);

  const flushWatchedTime = useCallback(
    (reason: string) => {
      if (playingSinceRef.current === null) {
        return;
      }

      const watchedSeconds =
        (performance.now() - playingSinceRef.current) / 1000;

      playingSinceRef.current = null;

      if (watchedSeconds < 0.25) {
        return;
      }

      void trackVideoWatched(
        getVideoActivity(),
        seconds(watchedSeconds),
        reason,
      );
    },
    [getVideoActivity],
  );

  useEffect(() => {
    function handlePageHide() {
      flushWatchedTime("page_hidden");
    }

    function handleVisibilityChange() {
      const video = videoRef.current;

      if (document.hidden) {
        flushWatchedTime("page_hidden");
      } else if (video && !video.paused && !video.ended) {
        playingSinceRef.current = performance.now();
      }
    }

    window.addEventListener("pagehide", handlePageHide);
    document.addEventListener(
      "visibilitychange",
      handleVisibilityChange,
    );

    return () => {
      flushWatchedTime("unmounted");
      window.removeEventListener("pagehide", handlePageHide);
      document.removeEventListener(
        "visibilitychange",
        handleVisibilityChange,
      );
    };
  }, [flushWatchedTime]);

  function handlePlay() {
    if (startedRef.current) {
      return;
    }

    startedRef.current = true;
    void trackVideoStarted(getVideoActivity());
  }

  function handlePlaying() {
    playingSinceRef.current ??= performance.now();
  }

  function handleEnded() {
    flushWatchedTime("ended");
    void trackVideoCompleted(getVideoActivity());
    startedRef.current = false;
  }

  return (
    <video
      className="featured-card__video"
      controls
      preload="metadata"
      ref={videoRef}
      onPlay={handlePlay}
      onPlaying={handlePlaying}
      onPause={() => flushWatchedTime("paused")}
      onWaiting={() => flushWatchedTime("buffering")}
      onEnded={handleEnded}
    >
      <source src={apiPath(`/files/${file.id}`)} type={file.contentType} />
    </video>
  );
}

function FeaturedFile({
  featuredContentId,
  file,
}: FeaturedVideoProps) {
  const source = apiPath(`/files/${file.id}`);

  if (file.contentType.startsWith("video/")) {
    return (
      <TrackedFeaturedVideo
        featuredContentId={featuredContentId}
        file={file}
      />
    );
  }

  if (file.contentType.startsWith("image/")) {
    return (
      <img
        className="featured-card__image"
        src={source}
        alt={file.originalFileName}
        loading="lazy"
      />
    );
  }

  return (
    <a
      className="button button--secondary featured-card__document"
      href={source}
      target="_blank"
      rel="noreferrer"
    >
      Open {file.originalFileName}
    </a>
  );
}

type FeaturedMediaCarouselProps = {
  featuredContentId: number;
  files: FeaturedContentFile[];
};

function FeaturedMediaCarousel({
  featuredContentId,
  files,
}: FeaturedMediaCarouselProps) {
  const [currentIndex, setCurrentIndex] = useState(0);
  const mediaRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    setCurrentIndex((index) =>
      Math.min(index, Math.max(files.length - 1, 0)),
    );
  }, [files.length]);

  if (files.length === 0) {
    return null;
  }

  function showFile(index: number) {
    mediaRef.current
      ?.querySelectorAll("video")
      .forEach((video) => video.pause());

    setCurrentIndex(index);
  }

  return (
    <div className="featured-carousel">
      <div className="featured-carousel__media" ref={mediaRef}>
        {files.map((file, index) => (
          <div key={file.id} hidden={index !== currentIndex}>
            <FeaturedFile
              featuredContentId={featuredContentId}
              file={file}
            />
          </div>
        ))}
      </div>

      {files.length > 1 && (
        <div className="featured-carousel__controls" aria-label="Choose media">
          {files.map((file, index) => (
            <button
              className={
                index === currentIndex
                  ? "featured-carousel__dot featured-carousel__dot--active"
                  : "featured-carousel__dot"
              }
              type="button"
              aria-label={`Show ${file.originalFileName}`}
              aria-current={index === currentIndex ? "true" : undefined}
              title={file.originalFileName}
              key={file.id}
              onClick={() => showFile(index)}
            />
          ))}
        </div>
      )}
    </div>
  );
}

type FeaturedSectionProps = {
  number: string;
};

export function FeaturedSection({ number }: FeaturedSectionProps) {
  const contentQuery = useFeaturedContent();

  return (
    <section className="home-section" id="featured-content">
      <div className="home-section__heading">
        <p className="home-section__number">{number}</p>

        <div>
          <p className="home-section__eyebrow">In focus</p>
          <h2>Featured</h2>
        </div>
      </div>

      <div className="home-section__content">
        <div className="home-section__connector" aria-hidden="true">
          <span className="home-section__connector-dot" />
          <span className="home-section__connector-line" />
        </div>

        <div className="featured-list">
          {contentQuery.isPending && (
            <article className="featured-card">
              <h3>Loading featured content...</h3>
            </article>
          )}

          {contentQuery.isError && (
            <article className="featured-card">
              <h3>Featured content unavailable</h3>
            </article>
          )}

          {contentQuery.isSuccess && contentQuery.data.items.length === 0 && (
            <article className="featured-card">
              <h3>Featured content coming soon</h3>
            </article>
          )}

          {contentQuery.data?.items.map((item) => (
            <article className="featured-card" key={item.id}>
              <div className="featured-card__copy">
                <h3>{item.title}</h3>
                <p>{item.description}</p>

                {item.tags.length > 0 && (
                  <ul className="featured-card__tags">
                    {item.tags.map((tag) => (
                      <li key={tag}>{tag}</li>
                    ))}
                  </ul>
                )}
              </div>

              {item.files.length > 0 && (
                <div className="featured-card__files">
                  <FeaturedMediaCarousel
                    featuredContentId={item.id}
                    files={item.files}
                  />
                </div>
              )}
            </article>
          ))}
        </div>
      </div>
    </section>
  );
}
