import { buildNavigation, canCreateTicket } from "../utils/permissions.js";
import { initials } from "../utils/formatters.js";

function Layout({ user, activeView, onNavigate, onRefresh, onLogout, children }) {
  const navItems = buildNavigation(user?.rol);

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-icon">leaf</div>
          <div>
            <h1>PdsTk Ticketera</h1>
            <p>{user?.rol}</p>
          </div>
        </div>

        {canCreateTicket(user?.rol) && (
          <button className="primary-button sidebar-button" onClick={() => onNavigate("tickets")}>
            <span className="material-symbols-outlined">add</span>
            Nuevo incidente
          </button>
        )}

        <nav className="sidebar-nav">
          {navItems.map((item) => (
            <button
              key={item.id}
              className={`nav-link ${activeView === item.id ? "active" : ""}`}
              onClick={() => onNavigate(item.id)}
            >
              <span className="material-symbols-outlined">{item.icon}</span>
              <span>{item.label}</span>
            </button>
          ))}
        </nav>

        <div className="sidebar-footer">
          <button className="nav-link" onClick={onRefresh}>
            <span className="material-symbols-outlined">refresh</span>
            <span>Actualizar</span>
          </button>
          <button className="nav-link" onClick={onLogout}>
            <span className="material-symbols-outlined">logout</span>
            <span>Salir</span>
          </button>
        </div>
      </aside>

      <div className="content-shell">
        <header className="topbar">
          <div>
            <h2>{getViewTitle(activeView)}</h2>
            <p>Base Terra simplificada para cubrir los flujos principales de la ticketera.</p>
          </div>
          <div className="topbar-profile">
            <div className="avatar">{initials(user?.nombre)}</div>
            <div>
              <strong>{user?.nombre}</strong>
              <span>{user?.correo}</span>
            </div>
          </div>
        </header>

        <main className="content">{children}</main>
      </div>
    </div>
  );
}

function getViewTitle(view) {
  if (view === "dashboard") return "Panel de control";
  if (view === "tickets") return "Gestión de tickets";
  if (view === "agentes") return "Catálogo de agentes";
  if (view === "sla") return "Políticas SLA";
  return "Ticketera";
}

export default Layout;
