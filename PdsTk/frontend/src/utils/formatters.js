export function friendlyEstado(value) {
  const map = {
    EnProgreso: "En progreso",
    EnEspera: "En espera",
  };

  return map[value] || value;
}

export function friendlyPrioridad(value) {
  const map = {
    Critica: "Crítica",
  };

  return map[value] || value;
}

export function normalizeToken(value) {
  return String(value || "")
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/\s+/g, "")
    .toLowerCase();
}

export function formatDate(value) {
  if (!value) {
    return "-";
  }

  return new Date(value).toLocaleString("es-PE", {
    dateStyle: "short",
    timeStyle: "short",
  });
}

export function initials(value) {
  return String(value || "")
    .split(" ")
    .filter(Boolean)
    .slice(0, 2)
    .map((item) => item[0]?.toUpperCase())
    .join("");
}
