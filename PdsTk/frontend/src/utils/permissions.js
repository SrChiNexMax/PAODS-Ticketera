import { ROLES } from "../config.js";

export function canCreateTicket(role) {
  return role === ROLES.solicitante;
}

export function canAssign(role) {
  return role === ROLES.supervisor || role === ROLES.administrador;
}

export function canChangeStatus(role) {
  return role === ROLES.supervisor || role === ROLES.administrador;
}

export function canResolve(role) {
  return [ROLES.supervisor, ROLES.administrador, ROLES.agente].includes(role);
}

export function canClose(role) {
  return [ROLES.supervisor, ROLES.administrador, ROLES.solicitante].includes(role);
}

export function canViewAgents(role) {
  return [ROLES.supervisor, ROLES.administrador].includes(role);
}

export function canViewSla(role) {
  return [ROLES.supervisor, ROLES.administrador].includes(role);
}

export function buildNavigation(role) {
  const items = [
    { id: "dashboard", label: "Dashboard", icon: "dashboard" },
    { id: "tickets", label: "Tickets", icon: "confirmation_number" },
  ];

  if (canViewAgents(role)) {
    items.push({ id: "agentes", label: "Agentes", icon: "group" });
  }

  if (canViewSla(role)) {
    items.push({ id: "sla", label: "SLA", icon: "schedule" });
  }

  return items;
}
