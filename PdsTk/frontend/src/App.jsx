import { useEffect, useMemo, useState } from "react";
import { api } from "./lib/api.js";
import { clearSession, loadSession, saveSession } from "./lib/session.js";
import Layout from "./components/Layout.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import DashboardPage from "./pages/DashboardPage.jsx";
import TicketsPage from "./pages/TicketsPage.jsx";
import AgentsPage from "./pages/AgentsPage.jsx";
import SlaPage from "./pages/SlaPage.jsx";
import { canViewAgents, canViewSla } from "./utils/permissions.js";

function App() {
  const [session, setSession] = useState(() => loadSession());
  const [activeView, setActiveView] = useState("dashboard");
  const [incidentes, setIncidentes] = useState([]);
  const [selectedIncident, setSelectedIncident] = useState(null);
  const [agentes, setAgentes] = useState([]);
  const [politicasSla, setPoliticasSla] = useState([]);
  const [loading, setLoading] = useState(false);
  const [feedback, setFeedback] = useState("");
  const [error, setError] = useState("");
  const [filters, setFilters] = useState({ estado: "", prioridad: "", q: "" });
  const [ticketForm, setTicketForm] = useState({
    titulo: "",
    descripcion: "",
    prioridad: "Media",
  });
  const [commentForm, setCommentForm] = useState({
    contenido: "",
    esInterno: false,
  });
  const [assignAgentId, setAssignAgentId] = useState("");
  const [statusDraft, setStatusDraft] = useState("EnProgreso");

  const token = session?.token;
  const user = session?.usuario;

  useEffect(() => {
    if (token) {
      refreshAppData();
    }
  }, [token]);

  async function refreshAppData(selectedIncidentId = selectedIncident?.id) {
    if (!token) {
      return;
    }

    setLoading(true);
    setError("");

    try {
      const [listaIncidentes, sla, agentesData] = await Promise.all([
        api.listarIncidentes(token),
        canViewSla(user?.rol) ? api.listarPoliticasSla(token) : Promise.resolve([]),
        canViewAgents(user?.rol) ? api.listarAgentes(token) : Promise.resolve([]),
      ]);

      setIncidentes(listaIncidentes);
      setPoliticasSla(sla);
      setAgentes(agentesData);

      if (selectedIncidentId) {
        const incidente = await api.obtenerIncidente(token, selectedIncidentId);
        applySelectedIncident(incidente);
      }
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setLoading(false);
    }
  }

  function applySelectedIncident(incidente) {
    setSelectedIncident(incidente);
    setAssignAgentId(incidente.agenteAsignadoId ? String(incidente.agenteAsignadoId) : "");
    setStatusDraft(incidente.estado === "En progreso" ? "EnProgreso" : incidente.estado);
    setCommentForm({
      contenido: "",
      esInterno: false,
    });
  }

  async function handleLogin(formData) {
    setError("");
    setFeedback("");

    try {
      const loginResult = await api.login(formData);
      saveSession(loginResult);
      setSession(loginResult);
      setActiveView("dashboard");
    } catch (loginError) {
      setError(loginError.message);
    }
  }

  function handleLogout() {
    clearSession();
    setSession(null);
    setIncidentes([]);
    setSelectedIncident(null);
    setAgentes([]);
    setPoliticasSla([]);
    setFeedback("");
    setError("");
  }

  async function handleCreateTicket(event) {
    event.preventDefault();
    setError("");
    setFeedback("");

    try {
      const created = await api.crearIncidente(token, ticketForm);
      setTicketForm({
        titulo: "",
        descripcion: "",
        prioridad: "Media",
      });
      setFeedback("Incidente creado correctamente.");
      await refreshAppData(created.id);
      setActiveView("tickets");
    } catch (requestError) {
      setError(requestError.message);
    }
  }

  async function handleSelectIncident(incidenteId) {
    setError("");
    setFeedback("");

    try {
      const incidente = await api.obtenerIncidente(token, incidenteId);
      applySelectedIncident(incidente);
      setActiveView("tickets");
    } catch (requestError) {
      setError(requestError.message);
    }
  }

  async function handleAssignIncident() {
    if (!selectedIncident || !assignAgentId) {
      return;
    }

    setError("");
    setFeedback("");

    try {
      const incidente = await api.asignarIncidente(token, selectedIncident.id, Number(assignAgentId));
      applySelectedIncident(incidente);
      setFeedback("Incidente asignado correctamente.");
      await refreshAppData(incidente.id);
    } catch (requestError) {
      setError(requestError.message);
    }
  }

  async function handleChangeStatus() {
    if (!selectedIncident || !statusDraft) {
      return;
    }

    setError("");
    setFeedback("");

    try {
      const incidente = await api.cambiarEstado(token, selectedIncident.id, statusDraft);
      applySelectedIncident(incidente);
      setFeedback("Estado actualizado correctamente.");
      await refreshAppData(incidente.id);
    } catch (requestError) {
      setError(requestError.message);
    }
  }

  async function handleAddComment(event) {
    event.preventDefault();
    if (!selectedIncident) {
      return;
    }

    setError("");
    setFeedback("");

    try {
      const incidente = await api.agregarComentario(token, selectedIncident.id, commentForm);
      applySelectedIncident(incidente);
      setFeedback("Comentario agregado correctamente.");
      await refreshAppData(incidente.id);
    } catch (requestError) {
      setError(requestError.message);
    }
  }

  async function handleResolveIncident() {
    if (!selectedIncident) {
      return;
    }

    setError("");
    setFeedback("");

    try {
      const incidente = await api.resolverIncidente(token, selectedIncident.id);
      applySelectedIncident(incidente);
      setFeedback("Incidente resuelto correctamente.");
      await refreshAppData(incidente.id);
    } catch (requestError) {
      setError(requestError.message);
    }
  }

  async function handleCloseIncident() {
    if (!selectedIncident) {
      return;
    }

    setError("");
    setFeedback("");

    try {
      const incidente = await api.cerrarIncidente(token, selectedIncident.id);
      applySelectedIncident(incidente);
      setFeedback("Incidente cerrado correctamente.");
      await refreshAppData(incidente.id);
    } catch (requestError) {
      setError(requestError.message);
    }
  }

  const filteredIncidentes = useMemo(() => {
    return incidentes.filter((incidente) => {
      const matchesEstado = !filters.estado || incidente.estado === filters.estado;
      const matchesPrioridad = !filters.prioridad || incidente.prioridad === filters.prioridad;
      const q = filters.q.trim().toLowerCase();
      const matchesSearch =
        !q ||
        incidente.codigo.toLowerCase().includes(q) ||
        incidente.titulo.toLowerCase().includes(q) ||
        incidente.solicitanteNombre.toLowerCase().includes(q) ||
        (incidente.agenteAsignadoNombre || "").toLowerCase().includes(q);

      return matchesEstado && matchesPrioridad && matchesSearch;
    });
  }, [filters, incidentes]);

  const stats = useMemo(() => {
    const total = incidentes.length;
    const resueltos = incidentes.filter((item) => item.estado === "Resuelto" || item.estado === "Cerrado").length;
    const pendientes = total - resueltos;
    const vencidos = incidentes.filter((item) => item.primeraRespuestaVencida || item.resolucionVencida).length;
    const porVencer = total - vencidos;

    return { total, resueltos, pendientes, porVencer, vencidos };
  }, [incidentes]);

  if (!session) {
    return <LoginPage onLogin={handleLogin} error={error} />;
  }

  return (
    <Layout
      user={user}
      activeView={activeView}
      onNavigate={setActiveView}
      onRefresh={() => refreshAppData()}
      onLogout={handleLogout}
    >
      {(error || feedback) && (
        <div className={`feedback ${error ? "error" : "success"}`}>{error || feedback}</div>
      )}

      {activeView === "dashboard" && (
        <DashboardPage
          stats={stats}
          incidentes={incidentes}
          agentes={agentes}
          politicasSla={politicasSla}
        />
      )}

      {activeView === "tickets" && (
        <TicketsPage
          user={user}
          loading={loading}
          incidentes={filteredIncidentes}
          selectedIncident={selectedIncident}
          agentes={agentes}
          filters={filters}
          setFilters={setFilters}
          ticketForm={ticketForm}
          setTicketForm={setTicketForm}
          commentForm={commentForm}
          setCommentForm={setCommentForm}
          assignAgentId={assignAgentId}
          setAssignAgentId={setAssignAgentId}
          statusDraft={statusDraft}
          setStatusDraft={setStatusDraft}
          onCreateTicket={handleCreateTicket}
          onSelectIncident={handleSelectIncident}
          onAssignIncident={handleAssignIncident}
          onChangeStatus={handleChangeStatus}
          onAddComment={handleAddComment}
          onResolveIncident={handleResolveIncident}
          onCloseIncident={handleCloseIncident}
        />
      )}

      {activeView === "agentes" && <AgentsPage agentes={agentes} />}
      {activeView === "sla" && <SlaPage politicasSla={politicasSla} />}
    </Layout>
  );
}

export default App;
