import { createContext, useContext, useEffect, useMemo, useState } from 'react';

const AuthContext = createContext(null);

function decodeJwtPayload(token) {
  if (!token) return null;
  try {
    const base64 = token.split('.')[1];
    const json = atob(base64.replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(json);
  } catch {
    return null;
  }
}

function getRolesFromToken(token) {
  const payload = decodeJwtPayload(token);
  if (!payload) return [];
  if (Array.isArray(payload.role)) return payload.role;
  if (typeof payload.role === 'string') return [payload.role];
  if (Array.isArray(payload.roles)) return payload.roles;
  if (typeof payload.roles === 'string') return [payload.roles];
  return [];
}

export function AuthProvider({ children }) {
  const [auth, setAuth] = useState(() => {
    const stored = localStorage.getItem('gymproject_auth');
    if (stored) {
      const parsed = JSON.parse(stored);
      return {
        token: parsed.token ?? null,
        email: parsed.email ?? null,
        displayName: parsed.displayName ?? null,
        roles: Array.isArray(parsed.roles) ? parsed.roles : getRolesFromToken(parsed.token)
      };
    }

    return { token: null, email: null, displayName: null, roles: [] };
  });

  useEffect(() => {
    localStorage.setItem('gymproject_auth', JSON.stringify(auth));
    if (auth?.token) {
      localStorage.setItem('gymproject_token', auth.token);
    } else {
      localStorage.removeItem('gymproject_token');
    }
  }, [auth]);

  const value = useMemo(
    () => ({ auth, setAuth, logout: () => setAuth({ token: null, email: null, displayName: null, roles: [] }) }),
    [auth]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  return useContext(AuthContext);
}
