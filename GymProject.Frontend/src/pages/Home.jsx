import { Link } from 'react-router-dom';

export default function Home() {
  return (
    <section className="page page-home">
      <h1>GymProject Frontend</h1>
      <p>Use this dashboard to manage authentication, fitness plans, exercises, and meal tracking.</p>
      <div className="card-grid">
        <Link className="card" to="/login">
          <strong>Login</strong>
          <p>Sign in to your account and access profile pages.</p>
        </Link>
        <Link className="card" to="/exercises">
          <strong>Exercises</strong>
          <p>Browse and create workout exercises.</p>
        </Link>
        <Link className="card" to="/fitness">
          <strong>Fitness Plans</strong>
          <p>View or add fitness plans for your users.</p>
        </Link>
        <Link className="card" to="/meals">
          <strong>Meals</strong>
          <p>Manage nutrition data and meal items.</p>
        </Link>
      </div>
    </section>
  );
}
