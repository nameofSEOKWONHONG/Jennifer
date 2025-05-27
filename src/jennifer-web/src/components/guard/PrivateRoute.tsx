// src/components/guard/PrivateRoute.tsx
import { Navigate } from 'react-router-dom';
import { useAuthStore } from '../../lib/useAuthStore';
import type { JSX } from 'react';

export default function PrivateRoute({ children }: { children: JSX.Element }) {
  const isAuthenticated = useAuthStore(state => state.isAuthenticated);
  return isAuthenticated ? children : <Navigate to="/signin" replace />;
}
