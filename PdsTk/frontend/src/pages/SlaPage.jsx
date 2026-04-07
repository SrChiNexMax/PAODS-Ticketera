import EmptyState from "../components/EmptyState.jsx";
import { PriorityPill } from "../components/Pills.jsx";

function SlaPage({ politicasSla }) {
  return (
    <section className="card">
      <div className="card-header">
        <div>
          <h3>Políticas SLA</h3>
          <p>Referencia rápida de tiempos por prioridad.</p>
        </div>
      </div>

      <div className="sla-grid">
        {politicasSla.map((politica) => (
          <article className="sla-card" key={politica.id}>
            <PriorityPill value={politica.prioridad} />
            <h4>{politica.nombre}</h4>
            <p>{politica.descripcion}</p>
            <div className="sla-metrics">
              <span>Primera respuesta: {politica.tiempoPrimeraRespuestaMinutos} min</span>
              <span>Resolución: {politica.tiempoResolucionMinutos} min</span>
            </div>
          </article>
        ))}
        {politicasSla.length === 0 && <EmptyState text="No hay políticas SLA visibles para este usuario." />}
      </div>
    </section>
  );
}

export default SlaPage;
