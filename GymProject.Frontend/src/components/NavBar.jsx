import { NavLink } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export default function NavBar() {
  const { auth, logout } = useAuth();
  const isSuperAdmin = auth?.roles?.includes('SuperAdmin');
  const isAdmin = auth?.roles?.includes('Admin');

  return (
    <nav className="top-nav">
      <div className="brand">GymProject</div>
      <div className="nav-links">
        {isSuperAdmin ? (
          <NavLink to="/users" className={({ isActive }) => (isActive ? 'active' : '')}>
            Users
          </NavLink>
        ) : isAdmin ? (
          <NavLink to="/admin" className={({ isActive }) => (isActive ? 'active' : '')}>
            Admin Dashboard
          </NavLink>
        ) : (
          <>
            <NavLink to="/" end className={({ isActive }) => (isActive ? 'active' : '')}>
              Home
            </NavLink>
            <NavLink to="/exercises" className={({ isActive }) => (isActive ? 'active' : '')}>
              Exercises
            </NavLink>
            <NavLink to="/fitness" className={({ isActive }) => (isActive ? 'active' : '')}>
              Fitness Plans
            </NavLink>
            <NavLink to="/existing-plans" className={({ isActive }) => (isActive ? 'active' : '')}>
              Existing Plans
            </NavLink>
            <NavLink to="/meals" className={({ isActive }) => (isActive ? 'active' : '')}>
              Meals
            </NavLink>
            <NavLink to="/profile" className={({ isActive }) => (isActive ? 'active' : '')}>
              Profile
            </NavLink>
          </>
        )}
      </div>
      <div className="auth-actions">
        {auth?.token ? (
          <>
            <span>{auth.displayName || auth.email}</span>
            <button type="button" onClick={logout}>Logout</button>
          </>
        ) : (
          <>
            <NavLink to="/login">Login</NavLink>
            <NavLink to="/register">Register</NavLink>
          </>
        )}
      </div>
    </nav>
  );
}
