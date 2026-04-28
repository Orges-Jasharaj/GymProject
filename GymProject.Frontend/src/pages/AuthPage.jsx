import { useEffect, useState } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { login, register } from '../api';
import { useAuth } from '../hooks/useAuth';

function AuthForm({ mode }) {
  const navigate = useNavigate();
  const { setAuth } = useAuth();
  const [form, setForm] = useState({ email: '', password: '', firstName: '', lastName: '', dateOfBirth: '' });
  const [message, setMessage] = useState(null);

  useEffect(() => setMessage(null), [mode]);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    try {
      if (mode === 'register') {
        await register({
          firstName: form.firstName,
          lastName: form.lastName,
          dateOfBirth: form.dateOfBirth,
          email: form.email,
          password: form.password
        });
        setMessage('Registration successful. Please log in.');
        navigate('/login');
        return;
      }

      const result = await login({ email: form.email, password: form.password });
      setAuth({
        token: result.accessToken,
        email: result.email,
        displayName: result.displayName,
        roles: result.roles ?? []
      });
      navigate('/');
    } catch (error) {
      setMessage(error.message ?? 'Unable to authenticate.');
    }
  };

  return (
    <div className="auth-card">
      <h2>{mode === 'login' ? 'Login' : 'Register'}</h2>
      <form onSubmit={handleSubmit}>
        {mode === 'register' && (
          <>
            <label>
              First name
              <input name="firstName" value={form.firstName} onChange={handleChange} required />
            </label>
            <label>
              Last name
              <input name="lastName" value={form.lastName} onChange={handleChange} required />
            </label>
            <label>
              Date of birth
              <input name="dateOfBirth" type="date" value={form.dateOfBirth} onChange={handleChange} required />
            </label>
          </>
        )}
        <label>
          Email
          <input name="email" type="email" value={form.email} onChange={handleChange} required />
        </label>
        <label>
          Password
          <input name="password" type="password" value={form.password} onChange={handleChange} required />
        </label>
        {message && <p className="form-message">{message}</p>}
        <button type="submit">{mode === 'login' ? 'Sign In' : 'Create Account'}</button>
      </form>
      <div className="form-footer">
        {mode === 'login' ? (
          <span>
            Need an account? <Link to="/register">Register</Link>
          </span>
        ) : (
          <span>
            Already registered? <Link to="/login">Login</Link>
          </span>
        )}
      </div>
    </div>
  );
}

export default function AuthPage() {
  const location = useLocation();
  const mode = location.pathname === '/register' ? 'register' : 'login';
  return (
    <section className="page page-auth">
      <AuthForm mode={mode} />
    </section>
  );
}
