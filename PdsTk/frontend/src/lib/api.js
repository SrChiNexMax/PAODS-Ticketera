import { API_BASE_URL } from "../config.js";

async function request(path, { token, method = "GET", body } = {}) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: body ? JSON.stringify(body) : undefined,
  });

  if (response.status === 204) {
    return null;
  }

  const text = await response.text();
  const data = text ? JSON.parse(text) : null;

  if (!response.ok) {
    throw new Error(data?.mensaje || "No se pudo completar la solicitud.");
  }

  return data;
}

export const api = {
  login(credentials) {
    return request("/auth/login", { method: "POST", body: credentials });
  },
  listarIncidentes(token, filters = {}) {
    const params = new URLSearchParams();

    Object.entries(filters).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== "") {
        params.append(key, value);
      }
    });

    const query = params.toString();
    return request(`/incidentes${query ? `?${query}` : ""}`, { token });
  },
  obtenerIncidente(token, incidenteId) {
    return request(`/incidentes/${incidenteId}`, { token });
  },
  crearIncidente(token, payload) {
    return request("/incidentes", { token, method: "POST", body: payload });
  },
  asignarIncidente(token, incidenteId, agenteId) {
    return request(`/incidentes/${incidenteId}/asignaciones`, {
      token,
      method: "POST",
      body: { agenteId },
    });
  },
  cambiarEstado(token, incidenteId, nuevoEstado) {
    return request(`/incidentes/${incidenteId}/estado`, {
      token,
      method: "PATCH",
      body: { nuevoEstado },
    });
  },
  agregarComentario(token, incidenteId, payload) {
    return request(`/incidentes/${incidenteId}/comentarios`, {
      token,
      method: "POST",
      body: payload,
    });
  },
  resolverIncidente(token, incidenteId) {
    return request(`/incidentes/${incidenteId}/resolver`, {
      token,
      method: "POST",
    });
  },
  cerrarIncidente(token, incidenteId) {
    return request(`/incidentes/${incidenteId}/cerrar`, {
      token,
      method: "POST",
    });
  },
  listarAgentes(token) {
    return request("/catalogos/agentes", { token });
  },
  listarPoliticasSla(token) {
    return request("/catalogos/politicas-sla", { token });
  },
};
