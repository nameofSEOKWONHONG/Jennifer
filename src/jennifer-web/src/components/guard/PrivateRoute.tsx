// src/components/guard/PrivateRoute.tsx
import { Navigate } from 'react-router-dom';
import { useAuthStore } from '../../lib/useAuthStore';
import type { JSX } from 'react';

export default function PrivateRoute({ children }: { children: JSX.Element }) {
  const {isAuthenticated, loading} = useAuthStore();
  if (loading) {
    return <div>Loading...</div>; // 또는 null, Skeleton 등
  }
  return isAuthenticated ? children : <Navigate to="/signin" replace />;
}
