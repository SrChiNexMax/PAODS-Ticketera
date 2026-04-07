import EmptyState from "../components/EmptyState.jsx";
import { PriorityPill, StatusPill, SubtlePill } from "../components/Pills.jsx";
import StatCard from "../components/StatCard.jsx";

function DashboardPage({ stats, incidentes, agentes, politicasSla }) {
  return (
    <div className="page-grid">
      <section className="stats-grid">
        <StatCard label="Total" value={stats.total} icon="confirmation_number" tone="primary" />
        <StatCard label="Resueltos" value={stats.resueltos} icon="task_alt" tone="secondary" />
        <StatCard label="Pendientes" value={stats.pendientes} icon="hourglass_top" tone="neutral" />
        <StatCard label="Sin vencer" value={stats.porVencer} icon="schedule" tone="warning" />
        <StatCard label="Vencidos" value={stats.vencidos} icon="error" tone="danger" />
      </section>

      <section className="dashboard-columns">
        <article className="card">
          <div className="card-header">
            <h3>Incidentes recientes</h3>
          </div>
          <div className="list">
            {incidentes.slice(0, 5).map((item) => (
              <div className="list-row" key={item.id}>
                <div>
                  <strong>{item.codigo}</strong>
                  <p>{item.titulo}</p>
                </div>
                <StatusPill value={item.estado} />
              </div>
            ))}
            {incidentes.length === 0 && <EmptyState text="No hay incidentes para mostrar." />}
          </div>
        </article>

        <article className="card">
          <div className="card-header">
            <h3>Agentes disponibles</h3>
          </div>
          <div className="list">
            {agentes.map((agente) => (
              <div className="list-row" key={agente.id}>
                <div>
                  <strong>{agente.nombre}</strong>
                  <p>{agente.correo}</p>
                </div>
                <SubtlePill value={agente.rol} />
              </div>
            ))}
            {agentes.length === 0 && <EmptyState text="Tu rol no tiene acceso al catálogo de agentes." />}
          </div>
        </article>

        <article className="card">
          <div className="card-header">
            <h3>Políticas SLA</h3>
          </div>
          <div className="list">
            {politicasSla.map((politica) => (
              <div className="list-row" key={politica.id}>
                <div>
                  <strong>{politica.nombre}</strong>
                  <p>
                    {politica.tiempoPrimeraRespuestaMinutos} min primera respuesta · {politica.tiempoResolucionMinutos} min resolución
                  </p>
                </div>
                <PriorityPill value={politica.prioridad} />
              </div>
            ))}
            {politicasSla.length === 0 && <EmptyState text="No hay políticas SLA visibles para este usuario." />}
          </div>
        </article>
      </section>
    </div>
  );
}

export default DashboardPage;
