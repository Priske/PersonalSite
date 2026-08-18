type AnalyticsPaginationProps = {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
};

export function AnalyticsPagination({
  page,
  pageSize,
  totalItems,
  totalPages,
  onPageChange,
  onPageSizeChange,
}: AnalyticsPaginationProps) {
  return (
    <div className="analytics-pagination">
      <div className="analytics-pagination__summary">
        <span>
          Page {page} of {Math.max(totalPages, 1)}
        </span>

        <span>{totalItems} total</span>
      </div>

      <div className="analytics-pagination__controls">
        <label className="analytics-pagination__size">
          <span>Rows</span>

          <select
            value={pageSize}
            onChange={(event) => onPageSizeChange(Number(event.target.value))}
          >
            <option value={10}>10</option>
            <option value={20}>20</option>
            <option value={50}>50</option>
          </select>
        </label>

        <button
          type="button"
          className="analytics-pagination__button"
          disabled={page <= 1}
          onClick={() => onPageChange(page - 1)}
        >
          Previous
        </button>

        <button
          type="button"
          className="analytics-pagination__button"
          disabled={totalPages === 0 || page >= totalPages}
          onClick={() => onPageChange(page + 1)}
        >
          Next
        </button>
      </div>
    </div>
  );
}
