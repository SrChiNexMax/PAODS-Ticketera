import { useState } from "react";

function LoginPage({ onLogin, error }) {
  const [form, setForm] = useState({ correo: "", clave: "" });

  function handleSubmit(event) {
    event.preventDefault();
    onLogin(form);
  }

  return (
    <div className="login-page">
      <div className="login-hero">
        <div className="hero-overlay" />
        <div className="hero-content">
          <h1>Terra Support</h1>
          <p>Accede al MVP de la ticketera con SLA, asignación y seguimiento por roles.</p>
        </div>
      </div>

      <div className="login-panel">
        <div className="panel-card">
          <h2>Bienvenido</h2>
          <p>Ingresa con un usuario sembrado en backend para probar los flujos.</p>

          {error && <div className="feedback error">{error}</div>}

          <form className="form-grid" onSubmit={handleSubmit}>
            <label>
              Correo
              <input
                type="email"
                value={form.correo}
                onChange={(event) => setForm((prev) => ({ ...prev, correo: event.target.value }))}
                placeholder="solicitante@pdstk.com"
              />
            </label>

            <label>
              Contraseña
              <input
                type="password"
                value={form.clave}
                onChange={(event) => setForm((prev) => ({ ...prev, clave: event.target.value }))}
                placeholder="********"
              />
            </label>

            <button className="primary-button" type="submit">
              Iniciar sesión
            </button>
          </form>

          <div className="demo-users">
            <strong>Usuarios demo</strong>
            <span>`supervisor@pdstk.com / Supervisor123*`</span>
            <span>`agente@pdstk.com / Agente123*`</span>
            <span>`solicitante@pdstk.com / Solicitante123*`</span>
          </div>
        </div>
      </div>
    </div>
  );
}

export default LoginPage;
