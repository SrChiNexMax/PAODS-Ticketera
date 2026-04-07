import EmptyState from "../components/EmptyState.jsx";

function AgentsPage({ agentes }) {
  return (
    <section className="card">
      <div className="card-header">
        <div>
          <h3>Agentes</h3>
          <p>Lista base para asignación desde el supervisor.</p>
        </div>
      </div>

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Correo</th>
              <th>Rol</th>
            </tr>
          </thead>
          <tbody>
            {agentes.map((agente) => (
              <tr key={agente.id}>
                <td>{agente.nombre}</td>
                <td>{agente.correo}</td>
                <td>{agente.rol}</td>
              </tr>
            ))}
            {agentes.length === 0 && (
              <tr>
                <td colSpan="3">
                  <EmptyState text="No hay agentes disponibles o tu rol no tiene acceso." />
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </section>
  );
}

export default AgentsPage;
