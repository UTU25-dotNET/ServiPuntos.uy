export const parseDecimal = (value) => {
  if (value === null || value === undefined) return 0;
  if (typeof value === "number") return value;

  const sanitized = value
    .toString()
    .trim()
    .replace(/[^0-9.,-]/g, "");

  const withoutThousands = sanitized.replace(/\.(?=\d{3}(?:\.|,|$))/g, "");
  const normalized = withoutThousands.replace(",", ".");

  const parsed = parseFloat(normalized);
  return isNaN(parsed) ? 0 : parsed;
};
