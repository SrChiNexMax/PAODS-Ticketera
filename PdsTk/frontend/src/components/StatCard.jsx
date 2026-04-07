function StatCard({ label, value, icon, tone }) {
  return (
    <article className={`stat-card ${tone}`}>
      <div>
        <span>{label}</span>
        <strong>{value}</strong>
      </div>
      <span className="material-symbols-outlined">{icon}</span>
    </article>
  );
}

export default StatCard;
