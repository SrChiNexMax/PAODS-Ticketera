import { ESTADOS_EDITABLES, PRIORIDADES } from "../config.js";
import EmptyState from "../components/EmptyState.jsx";
import { PriorityPill, StatusPill, SubtlePill } from "../components/Pills.jsx";
import { formatDate, friendlyEstado, friendlyPrioridad } from "../utils/formatters.js";
import {
  canAssign,
  canChangeStatus,
  canClose,
  canCreateTicket,
  canResolve,
} from "../utils/permissions.js";

function TicketsPage(props) {
  const {
    user,
    loading,
    incidentes,
    selectedIncident,
    agentes,
    filters,
    setFilters,
    ticketForm,
    setTicketForm,
    commentForm,
    setCommentForm,
    assignAgentId,
    setAssignAgentId,
    statusDraft,
    setStatusDraft,
    onCreateTicket,
    onSelectIncident,
    onAssignIncident,
    onChangeStatus,
    onAddComment,
    onResolveIncident,
    onCloseIncident,
  } = props;

  const estadoActual = selectedIncident?.estado;
  const incidenteBloqueado = estadoActual === "Cerrado" || estadoActual === "Cancelado";
  const puedeAsignarAgente = Boolean(selectedIncident) && canAssign(user?.rol) && !incidenteBloqueado;
  const puedeCambiarEstadoIncidente =
    Boolean(selectedIncident) &&
    canChangeStatus(user?.rol) &&
    !incidenteBloqueado &&
    estadoActual !== "Resuelto";
  const puedeResolverIncidente =
    Boolean(selectedIncident) &&
    canResolve(user?.rol) &&
    !incidenteBloqueado &&
    estadoActual !== "Resuelto";
  const puedeCerrarIncidente = Boolean(selectedIncident) && canClose(user?.rol) && estadoActual === "Resuelto";
  const mostrarAccionesRapidas = puedeResolverIncidente || puedeCerrarIncidente;

  return (
    <div className="tickets-layout">
      <section className="card incident-list-card">
        <div className="card-header">
          <div>
            <h3>Incidentes</h3>
            <p>Busca y filtra la cola de trabajo por estado y prioridad.</p>
          </div>
        </div>

        <div className="filters-bar">
          <input
            value={filters.q}
            onChange={(event) => setFilters((prev) => ({ ...prev, q: event.target.value }))}
            placeholder="Buscar por código, título o responsable"
          />
          <select
            value={filters.estado}
            onChange={(event) => setFilters((prev) => ({ ...prev, estado: event.target.value }))}
          >
            <option value="">Todos los estados</option>
            {["Nuevo", "Asignado", "EnProgreso", "EnEspera", "Resuelto", "Cerrado"].map((estado) => (
              <option key={estado} value={estado}>
                {friendlyEstado(estado)}
              </option>
            ))}
          </select>
          <select
            value={filters.prioridad}
            onChange={(event) => setFilters((prev) => ({ ...prev, prioridad: event.target.value }))}
          >
            <option value="">Todas las prioridades</option>
            {PRIORIDADES.map((prioridad) => (
              <option key={prioridad} value={prioridad}>
                {friendlyPrioridad(prioridad)}
              </option>
            ))}
          </select>
        </div>

        <div className="incident-list">
          {loading && <EmptyState text="Cargando incidentes..." />}

          {!loading &&
            incidentes.map((item) => (
              <button
                key={item.id}
                className={`incident-item ${selectedIncident?.id === item.id ? "selected" : ""}`}
                onClick={() => onSelectIncident(item.id)}
              >
                <div className="incident-item-top">
                  <strong>{item.codigo}</strong>
                  <PriorityPill value={item.prioridad} />
                </div>
                <h4>{item.titulo}</h4>
                <div className="incident-meta">
                  <StatusPill value={item.estado} />
                  {(item.primeraRespuestaVencida || item.resolucionVencida) && <SubtlePill value="SLA vencido" />}
                </div>
                <p>Solicitante: {item.solicitanteNombre}</p>
                <p>Agente: {item.agenteAsignadoNombre || "Sin asignar"}</p>
              </button>
            ))}

          {!loading && incidentes.length === 0 && <EmptyState text="No hay incidentes con esos filtros." />}
        </div>
      </section>

      <section className="ticket-workspace">
        {canCreateTicket(user?.rol) && (
          <article className="card">
            <div className="card-header">
              <h3>Registrar incidente</h3>
            </div>
            <form className="form-grid" onSubmit={onCreateTicket}>
              <label>
                Título
                <input
                  value={ticketForm.titulo}
                  onChange={(event) => setTicketForm((prev) => ({ ...prev, titulo: event.target.value }))}
                  placeholder="Describe el incidente"
                />
              </label>
              <label>
                Prioridad
                <select
                  value={ticketForm.prioridad}
                  onChange={(event) => setTicketForm((prev) => ({ ...prev, prioridad: event.target.value }))}
                >
                  {PRIORIDADES.map((prioridad) => (
                    <option key={prioridad} value={prioridad}>
                      {friendlyPrioridad(prioridad)}
                    </option>
                  ))}
                </select>
              </label>
              <label className="full-width">
                Descripción
                <textarea
                  rows="4"
                  value={ticketForm.descripcion}
                  onChange={(event) => setTicketForm((prev) => ({ ...prev, descripcion: event.target.value }))}
                  placeholder="Explica el problema, contexto y urgencia."
                />
              </label>
              <button className="primary-button" type="submit">
                Crear incidente
              </button>
            </form>
          </article>
        )}

        <article className="card detail-card">
          {!selectedIncident && <EmptyState text="Selecciona un incidente para ver el detalle." />}

          {selectedIncident && (
            <>
              <div className="card-header detail-header">
                <div>
                  <div className="detail-title-row">
                    <strong>{selectedIncident.codigo}</strong>
                    <StatusPill value={selectedIncident.estado} />
                    <PriorityPill value={selectedIncident.prioridad} />
                  </div>
                  <h3>{selectedIncident.titulo}</h3>
                  <p>{selectedIncident.descripcion}</p>
                </div>
              </div>

              <div className="detail-grid">
                <DetailItem label="Solicitante" value={selectedIncident.solicitanteNombre} />
                <DetailItem label="Agente" value={selectedIncident.agenteAsignadoNombre || "Sin asignar"} />
                <DetailItem label="Fecha de creación" value={formatDate(selectedIncident.fechaCreacion)} />
                <DetailItem label="Primera respuesta límite" value={formatDate(selectedIncident.fechaLimitePrimeraRespuesta)} />
                <DetailItem label="Resolución límite" value={formatDate(selectedIncident.fechaLimiteResolucion)} />
                <DetailItem label="Política SLA" value={selectedIncident.politicaSla.nombre} />
              </div>

              {puedeAsignarAgente && (
                <div className="actions-grid">
                  <label>
                    Asignar agente
                    <select value={assignAgentId} onChange={(event) => setAssignAgentId(event.target.value)}>
                      <option value="">Selecciona un agente</option>
                      {agentes.map((agente) => (
                        <option key={agente.id} value={agente.id}>
                          {agente.nombre}
                        </option>
                      ))}
                    </select>
                  </label>
                  <button className="secondary-button" onClick={onAssignIncident} disabled={!assignAgentId}>
                    Asignar
                  </button>
                </div>
              )}

              {puedeCambiarEstadoIncidente && (
                <div className="actions-grid">
                  <label>
                    Cambiar estado
                    <select value={statusDraft} onChange={(event) => setStatusDraft(event.target.value)}>
                      {ESTADOS_EDITABLES.map((estado) => (
                        <option key={estado} value={estado}>
                          {friendlyEstado(estado)}
                        </option>
                      ))}
                    </select>
                  </label>
                  <button className="secondary-button" onClick={onChangeStatus}>
                    Guardar estado
                  </button>
                </div>
              )}

              {mostrarAccionesRapidas && (
                <div className="actions-row">
                  {puedeResolverIncidente && (
                  <button className="primary-button" onClick={onResolveIncident}>
                    Resolver incidente
                  </button>
                  )}
                  {puedeCerrarIncidente && (
                    <button className="secondary-button" onClick={onCloseIncident}>
                      Cerrar incidente
                    </button>
                  )}
                </div>
              )}

              <div className="comments-section">
                <div className="card-header">
                  <h3>Comentarios</h3>
                </div>

                <div className="comment-list">
                  {selectedIncident.comentarios.map((comentario) => (
                    <div className="comment-card" key={comentario.id}>
                      <div className="comment-head">
                        <strong>{comentario.usuarioNombre}</strong>
                        <div className="comment-meta">
                          {comentario.esInterno && <SubtlePill value="Interno" />}
                          <span>{formatDate(comentario.fechaCreacion)}</span>
                        </div>
                      </div>
                      <p>{comentario.contenido}</p>
                    </div>
                  ))}
                  {selectedIncident.comentarios.length === 0 && <EmptyState text="Aún no hay comentarios." />}
                </div>

                {(selectedIncident.puedeAgregarComentarioPublico || selectedIncident.puedeAgregarComentarioInterno) && (
                  <form className="form-grid" onSubmit={onAddComment}>
                    <label className="full-width">
                      Nuevo comentario
                      <textarea
                        rows="3"
                        value={commentForm.contenido}
                        onChange={(event) => setCommentForm((prev) => ({ ...prev, contenido: event.target.value }))}
                        placeholder="Escribe una actualización o respuesta."
                      />
                    </label>
                    {selectedIncident.puedeAgregarComentarioInterno && (
                      <label className="checkbox-row">
                        <input
                          type="checkbox"
                          checked={commentForm.esInterno}
                          onChange={(event) => setCommentForm((prev) => ({ ...prev, esInterno: event.target.checked }))}
                        />
                        Marcar como comentario interno
                      </label>
                    )}
                    <button className="primary-button" type="submit">
                      Agregar comentario
                    </button>
                  </form>
                )}
              </div>
            </>
          )}
        </article>
      </section>
    </div>
  );
}

function DetailItem({ label, value }) {
  return (
    <div className="detail-item">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

export default TicketsPage;
