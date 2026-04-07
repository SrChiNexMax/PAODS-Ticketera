import { friendlyEstado, friendlyPrioridad, normalizeToken } from "../utils/formatters.js";

export function StatusPill({ value }) {
  return <span className={`pill estado ${normalizeToken(value)}`}>{friendlyEstado(value)}</span>;
}

export function PriorityPill({ value }) {
  return <span className={`pill prioridad ${normalizeToken(value)}`}>{friendlyPrioridad(value)}</span>;
}

export function SubtlePill({ value }) {
  return <span className="subtle-pill">{value}</span>;
}
