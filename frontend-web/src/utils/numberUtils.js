export const parseDecimal = (value) => {
  if (value === null || value === undefined) return 0;
  if (typeof value === 'number') return value;
  const normalized = value
    .toString()
    .trim()
    // remove currency symbols and spaces
    .replace(/[^0-9.,-]/g, '')
    // remove thousand separators
    .replace(/\./g, '')
    // unify decimal separator
    .replace(',', '.');
  const parsed = parseFloat(normalized);
  return isNaN(parsed) ? 0 : parsed;
};