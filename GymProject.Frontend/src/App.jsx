import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './hooks/useAuth';
import NavBar from './components/NavBar';
import Home from './pages/Home';
import AuthPage from './pages/AuthPage';
import UsersPage from './pages/UsersPage';
import ProfilePage from './pages/ProfilePage';
import ExercisesPage from './pages/ExercisesPage';
import FitnessPlansPage from './pages/FitnessPlansPage';
import MealsPage from './pages/MealsPage';
import NotFound from './pages/NotFound';

function ProtectedRoute({ children }) {
  const { auth } = useAuth();
  if (!auth?.token) {
    return <Navigate to="/login" replace />;
  }
  return children;
}

function AppRoutes() {
  return (
    <>
      <NavBar />
      <main className="app-shell">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<AuthPage />} />
          <Route path="/register" element={<AuthPage />} />
          <Route path="/users" element={<ProtectedRoute><UsersPage /></ProtectedRoute>} />
          <Route path="/profile" element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />
          <Route path="/exercises" element={<ProtectedRoute><ExercisesPage /></ProtectedRoute>} />
          <Route path="/fitness" element={<ProtectedRoute><FitnessPlansPage /></ProtectedRoute>} />
          <Route path="/meals" element={<ProtectedRoute><MealsPage /></ProtectedRoute>} />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </main>
    </>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
}
